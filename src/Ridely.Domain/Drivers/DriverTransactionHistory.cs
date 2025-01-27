using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Transactions;

namespace Ridely.Domain.Drivers;
public sealed class DriverTransactionHistory : Entity
{
    private DriverTransactionHistory()
    {
        
    }

    public DriverTransactionHistory(long driverId, decimal amount,
        DriverTransactionType transactionType, Ulid reference, 
        TransactionStatus status)
    {
        Reference = reference;
        DriverId = driverId;
        Amount = amount;
        Status = status;
        Type = transactionType;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Ulid Reference { get; private set; }
    public long DriverId { get; private set; }
    [ForeignKey(nameof(DriverId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Driver Driver { get; private set; }
    public decimal Amount { get; private set; }
    public string? Error { get; private set; }
    public string? BankAccountDetails { get; private set; }
    public TransactionStatus Status { get; private set; }
    public DriverTransactionType Type { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public void SetBankAccountDetails(string bankName, string accountNo, string accountName)
    {
        DriverBankAccount bankAccount = new(bankName, accountNo, accountName);

        BankAccountDetails = JsonSerializer.Serialize(bankAccount);
    }

    public void UpdateStatus(TransactionStatus status, string? error = null)
    {
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
        Error = error;
    }
}

internal sealed record DriverBankAccount(string Bank, string AccountNo, string AccountName);


