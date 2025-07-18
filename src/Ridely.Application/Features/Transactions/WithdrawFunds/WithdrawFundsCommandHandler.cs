﻿using System.Text.Json;
using Microsoft.Extensions.Options;
using Ridely.Application.Abstractions.Messaging;
using Ridely.Application.Abstractions.Payment;
using Ridely.Application.Abstractions.Settings;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Drivers;
using Ridely.Domain.Services;
using Ridely.Domain.Transactions;
using Ridely.Shared.Helper.Keys;

namespace Ridely.Application.Features.Transactions.WithdrawFunds;
internal sealed class WithdrawFundsCommandHandler :
    ICommandHandler<WithdrawFundsCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaystackService _paymentService;
    private readonly ICacheService _cacheService;
    private readonly IDriverRepository _driverRepository;
    private readonly IDriverTransactionHistoryRepository _driverTransactionHistoryRepository;
    private readonly IDriverWalletRepository _driverWalletRepository;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly ApplicationSettings _applicationSettings;

    public WithdrawFundsCommandHandler(IUnitOfWork unitOfWork, IPaystackService paymentService,
        ICacheService cacheService, IDriverRepository driverRepository,
        IDriverTransactionHistoryRepository driverTransactionHistoryRepository,
        IDriverWalletRepository driverWalletRepository, 
        IBankAccountRepository bankAccountRepository,
        IOptions<ApplicationSettings> applicationSettings)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _cacheService = cacheService;
        _driverRepository = driverRepository;
        _driverTransactionHistoryRepository = driverTransactionHistoryRepository;
        _driverWalletRepository = driverWalletRepository;
        _bankAccountRepository = bankAccountRepository;
        _applicationSettings = applicationSettings.Value;
    }

    public async Task<Result<bool>> Handle(WithdrawFundsCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount < _applicationSettings.MinimumWithdrawAmount)
            return Error.BadRequest("invalid.amount", $"Minimum withdraw amount is {_applicationSettings.MinimumWithdrawAmount}");

        var driver = await _driverRepository
            .GetDetailsAsync(request.DriverId);

        if (driver is null) return Error.NotFound("driver.notfound", "Driver not found");

        string key = Cache.ProcessWithdrawal.Key(driver.PhoneNo!);

        string? code = await _cacheService.GetAsync(key);

        if (string.IsNullOrWhiteSpace(code) || code != request.Otp)
            return Error.BadRequest("invalid.otp", "Invalid otp");

        var wallet = await _driverWalletRepository
            .GetAsync(driver.Id);

        if (wallet is null) return Error.NotFound("wallet.notfound",
            "Wallet not found");

        if (wallet.AvailableBalance < request.Amount)
            return Error.BadRequest("insufficient.amount", "Insufficient amount in wallet");

        wallet.DeductBalance(request.Amount);

        var bankAccount = await _bankAccountRepository
            .GetByDriverAsync(request.DriverId, request.BankAccountId);

        if (bankAccount is null)
            return Error.NotFound("bankaccount.notfound", "Bank account not found");

        // todo: Based paystack doc, we could write a separate method for admin to initiate a transfer
        // using this same reference and amount...this is in situation where it fails the first time
        // so instead of creating a new transer initiate request, we call endpoint with same rerefence
        Ulid reference = Ulid.NewUlid();

        DriverTransactionHistory transactionHistory = new(
            driver.Id,
            request.Amount, 
            DriverTransactionType.Withdrawal, 
            reference, TransactionStatus.Pending);
      
        transactionHistory.SetBankAccountDetails(bankAccount.Bank.Name, bankAccount.AccountNo, bankAccount.AccountName);

        _driverWalletRepository.Update(wallet);

        var response = await _paymentService
            .InitiateTransfer(request.Amount, bankAccount.RecipientCode, reference.ToString());

        if (response.IsFailure)
        {
            // todo: if successful later on, update to success/completed
            // confirm if this should be retry or failed...
            // if retry, we need functionality to enable retry of transaction...
            transactionHistory.UpdateStatus(
                TransactionStatus.Retry,
                JsonSerializer.Serialize(response.Error));
        }

        await _driverTransactionHistoryRepository
            .AddAsync(transactionHistory);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}