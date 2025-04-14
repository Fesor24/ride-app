namespace Ridely.Domain.Rides;
public enum PaymentFor
{
    EstimatedCharge = 1,
    RerouteCharge = 2,
    WaitTime = 3
}

// should we have a cumulative charge with reference
// I guess we could have a transaction in the riders transaction table
// this could have reference to the payment details references...
// we do it this way...