namespace Modules.ChatSystem.Models
{
    public class GeneralEvent<T>
    {
        public string? EventName { get; set; }
        public T? EventArgs { get; set; }
    }
}
