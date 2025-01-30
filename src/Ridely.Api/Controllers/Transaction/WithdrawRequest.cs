namespace Ridely.Api.Controllers.Transaction
{
    public sealed class WithdrawRequest
    {
        public int Amount { get; set; }
        public long BankAccountId { get; set; }
        public string Otp { get; set; }
    }
}
