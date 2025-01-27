using Soloride.Domain.Call;
using Soloride.Domain.Common;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Domain.Transactions;
using Soloride.Domain.Users;
using Soloride.Infrastructure.Repositories;

namespace Soloride.Infrastructure;
internal sealed class UnitOfWork(ApplicationDbContext context)
{
    #region Users
    public IUserRepository UserRepository => new UserRepository(context);
    public IRoleRepository RoleRepository => new RoleRepository(context);
    
    #endregion

    #region Riders
    public IRiderTransactionHistoryRepository RiderTransactionHistoryRepository => new RiderTransactionHistoryRepository(context);
    public IRiderWalletRepository RiderWalletRepository => new RiderWalletRepository(context);
    public IRiderRepository RiderRepository => new RiderRepository(context);
    public IRiderReferrersRepository RiderReferrersRepository => new RiderReferrersRepository(context);
    #endregion

    #region Drivers
    public IDriverRepository DriverRepository => new DriverRepository(context);
    public IDriverTransactionHistoryRepository DriverTransactionHistoryRepository => new DriverTransactionHistoryRepository(context);
    public IDriverWalletRepository DriverWalletRepository => new DriverWalletRepository(context);
    public ICabRepository CabRepository => new CabRepository(context);
    public IDriverReferrersRepository DriverReferrersRepository => new DriverReferrersRepository(context);

    #endregion

    #region Rides
    public IRideRepository RideRepository => new RideRepository(context);
    public IPaymentRepository PaymentRepository => new PaymentRepository(context);
    public IRatingsRepository RatingsRepository => new RatingsRepository(context);
    public IChatRepository ChatRepository => new ChatRepository(context);
    public ICallLogRepository CallLogRepository => new CallLogRepository(context);
    public IRideLogRepository RideLogRepository => new RideLogRepository(context);
    #endregion

    #region Common
    public ISettingsRepository SettingsRepository => new SettingsRepository(context);
    public IBankRepository BankRepository => new BankRepository(context);
    public IBankAccountRepository BankAccountRepository => new BankAccountRepository(context);
    public ISavedLocationRepository SavedLocationRepository => new SavedLocationRepository(context);
    #endregion

    #region Transactions
    public ITransactionLogRepository TransactionLogRepository => new TransactionLogRepository(context);
    public IPaymentCardRepository PaymentCardRepository => new PaymentCardRepository(context);
    public ITransactionReferenceMapRepository TransactionReferenceMapRepository => new TransactionReferenceMapRepository(context);
    #endregion

    public async Task<int> Complete() =>
        await context.SaveChangesAsync();

    public void Dispose() =>
        context.Dispose();
}
