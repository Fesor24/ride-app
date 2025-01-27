using Soloride.Domain.Call;
using Soloride.Domain.Common;
using Soloride.Domain.Drivers;
using Soloride.Domain.Riders;
using Soloride.Domain.Rides;
using Soloride.Domain.Transactions;
using Soloride.Domain.Users;

namespace Soloride.Domain.Abstractions;
public interface IUnitOfWork : IDisposable
{
    //#region Users
    //IUserRepository UserRepository { get; }
    //IRoleRepository RoleRepository { get; }
    
    //#endregion

    //#region Driver
    //IDriverTransactionHistoryRepository DriverTransactionHistoryRepository { get; }
    //IDriverRepository DriverRepository { get; }
    //IDriverWalletRepository DriverWalletRepository { get; }
    //IDriverReferrersRepository DriverReferrersRepository { get; }
    //ICabRepository CabRepository { get; }
    //IBankAccountRepository BankAccountRepository { get; }
    //#endregion

    //#region Rider
    //IRiderTransactionHistoryRepository RiderTransactionHistoryRepository { get; }
    //IRiderReferrersRepository RiderReferrersRepository { get; }
    //IRiderRepository RiderRepository { get; }
    //IRiderWalletRepository RiderWalletRepository { get; }
    //ISavedLocationRepository SavedLocationRepository { get; }
    //#endregion

    //#region Rides
    //IRideRepository RideRepository { get; }
    //IRideLogRepository RideLogRepository { get; }
    //IPaymentRepository PaymentRepository { get; }
    //IRatingsRepository RatingsRepository { get; }
    //IChatRepository ChatRepository { get; }
    //ICallLogRepository CallLogRepository { get; }
    //#endregion

    //#region Common
    //ISettingsRepository SettingsRepository { get; }
    //IBankRepository BankRepository { get; }
    
    //#endregion

    //#region Transactions
    //ITransactionLogRepository TransactionLogRepository { get; }
    //ITransactionReferenceMapRepository TransactionReferenceMapRepository { get; }
    //IPaymentCardRepository PaymentCardRepository { get; }
    //#endregion

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
