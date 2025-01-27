using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Common;

namespace Ridely.Domain.Drivers;
public sealed class BankAccount : Entity
{
    private BankAccount()
    {

    }

    public BankAccount(long driverId, string accountNo, string accountName,
        long bankId, string recipientCode)
    {
        DriverId = driverId;
        AccountNo = accountNo;
        AccountName = accountName;
        BankId = bankId;
        RecipientCode = recipientCode;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public long DriverId { get; private set; }
    [ForeignKey(nameof(DriverId))]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Driver Driver { get; }
    public string AccountNo { get; private set; }
    public string AccountName { get; private set; }
    public long BankId { get; private set; }
    public Bank Bank { get; private set; }
    public string RecipientCode { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
}
