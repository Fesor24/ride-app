using Newtonsoft.Json.Linq;

namespace DotnetAuthBoilerPlate.Modules.PaystackModule
{
    public interface IPaystackCallback
    {
        public Task Callback(string Body);
    }
}
