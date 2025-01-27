namespace SolorideAPI.Controllers.VoiceCall
{
    public sealed class StartCallRequest
    {
        public int RideId { get; set; }
        public CallUserRequest Caller { get; set; }
    }

    public enum CallUserRequest
    {
        Rider = 1,
        Driver = 2
    }
}
