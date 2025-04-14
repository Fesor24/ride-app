using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Ridely.Domain.Abstractions;
using Ridely.Domain.Rides;

namespace Ridely.Domain.Call
{
    public sealed class CallLog : Entity
    {
        private CallLog()
        {
        }

        public CallLog(long rideId, CallLogUser recipient, CallLogUser caller)
        {
            RideId = rideId;
            Recipient = recipient;
            Caller = caller;
            CallStartUtc = DateTime.UtcNow;
        }

        public long RideId { get; private set; }
        [ForeignKey(nameof(RideId))]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Ride Ride { get; }
        public CallLogUser Recipient { get; private set; }
        public CallLogUser Caller { get; private set; }
        public int DurationInSeconds { get; private set; }
        public DateTime CallStartUtc { get; private set; }
        public DateTime? CallEndUtc { get; private set; }

        public void EndCall()
        {
            CallEndUtc = DateTime.UtcNow;
            DurationInSeconds = (DateTime.UtcNow - CallStartUtc).Seconds;
        }
    }
}
