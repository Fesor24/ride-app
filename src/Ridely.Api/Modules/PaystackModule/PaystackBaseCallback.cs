
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace DotnetAuthBoilerPlate.Modules.PaystackModule
{
    public partial class PaystackBaseCallback : IPaystackCallback 
    {
        protected readonly ILogger logger;
        protected readonly IServiceProvider serviceProvider;

        public PaystackBaseCallback(ILogger<PaystackBaseCallback> logger, IServiceProvider serviceProvider ) {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }
        public virtual async Task Callback(string Body) {
            
            JToken? Result =  JsonConvert.DeserializeObject<JToken>(Body);
            if(Result == null)
            {
                return;
            }
            object controller = ActivatorUtilities.CreateInstance(serviceProvider, this.GetType());
            string? methodName = Result["event"]?.ToString().Replace(".","_");
            MethodInfo[] methods = controller.GetType().GetMethods();
            foreach( MethodInfo method in methods )
            {
                if( method.Name == methodName )
                {
                    ParameterInfo[] parameterInfo = method.GetParameters();
                    if( parameterInfo.Length == 1 && Result["data"] != null) {
                        JToken? object_ =  Result["data"];
                        object[] params_ = { object_};
                        Task t = (Task)method.Invoke(controller, params_);
                        await t;
                    }
                    
                }                        
            }
        }

        public virtual async Task charge_success(JToken data){
            logger.LogInformation("Charge not Configured");
        }

    }
}
