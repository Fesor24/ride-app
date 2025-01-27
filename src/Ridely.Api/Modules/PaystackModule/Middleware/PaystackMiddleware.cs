

using DotnetAuthBoilerPlate.Modules.PaystackModule.Config;
using DotnetAuthBoilerPlate.Modules.PaystackModule.Services;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace DotnetAuthBoilerPlate.Modules.PaystackModule.Middleware
{
    public class PaystackMiddleware
    {
        private readonly ILogger<PaystackMiddleware> logger;
        private readonly RequestDelegate next;

        public PaystackMiddleware(ILogger<PaystackMiddleware> logger,RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
            
        }
        public async Task Invoke(HttpContext context, IPaystackCallback PaystackBaseCallback, IOptions<PaystackConfig> paystackConfig) {

            if (context.Request.Path != "/PaystackCallback")
            {
                await next(context);
            }
            else
            {
                
                if(context.Request.Method.ToLower() != "post")
                {
                    await next(context);
                }
                string? signature;
                try
                {
                    signature = context.Request.Headers["X-Paystack-Signature"];
                    /*foreach (var header in context.Request.Headers)
                    {
                        Console.WriteLine(header.Key+":"+header.Value);
                    }*/
                    
                }
                catch {
                    await next(context);
                    return;
                }

                var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
                await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
                var requestContent = Encoding.UTF8.GetString(buffer);
                string? key = paystackConfig.Value.API_KEY;
                string result = GenerateHMACSHA512(key, requestContent);
                
                if (result == signature) {
                    context.Response.StatusCode = 200;
                    try
                    {
                        await PaystackBaseCallback?.Callback(requestContent);
                        return;
                    }
                    catch(Exception e){
                        logger.LogError(e.Message+"+\n"+e.Source+"\n"+e.StackTrace);
                    }
                
                }
                else
                {
                    await next(context);
                
                }
            }

        }

        private string GenerateHMACSHA512(string secret, string data)
        {
            byte[] secretBytes = Encoding.UTF8.GetBytes(secret);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            using (HMACSHA512 hmac = new HMACSHA512(secretBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

    }

    public static class PaystackExtentions
    {
        public static IApplicationBuilder UsePaystackGateway(this IApplicationBuilder app)
        {
            return app.UseMiddleware<PaystackMiddleware>();
        }

        public static void addPaystackGateway<T>(this IServiceCollection svcs, IConfiguration config) where T : PaystackBaseCallback
        {
            svcs.Configure<PaystackConfig>(config);
            svcs.AddScoped<IPaystackCallback,T>();
            svcs.AddScoped<IPaystackPaymentService, PaystackPaymentService>();
            svcs.AddMvc();
        }


    }

    
}
