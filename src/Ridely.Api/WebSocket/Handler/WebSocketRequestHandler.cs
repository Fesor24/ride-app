using Modules.ChatSystem.Models;
using Newtonsoft.Json.Linq;
using SolorideAPI.WebSocket.Attributes;
using SolorideAPI.WebSocket.Controller;
using System.Reflection;

namespace SolorideAPI.WebSocket.Handler;

public class WebSocketRequestHandler
{
    private Dictionary<string, object> _controllers;

    private IEnumerable<Type> _subClasses;

    public Dictionary<string, KeyValuePair<string, MethodInfo>> ControllerMappedMethods;

    private readonly ILogger<WebSocketRequestHandler> _logger;

    public WebSocketRequestHandler(IServiceProvider serviceProvider, string userKey)
    {
        _controllers = new();

        ControllerMappedMethods = new();

        _subClasses = Assembly
            .GetAssembly(typeof(WebSocketControllerBase))
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(WebSocketControllerBase)));

        _logger = serviceProvider.GetRequiredService<ILogger<WebSocketRequestHandler>>();

        foreach (var subClass in _subClasses)
        {
            WebSocketRouteAttribute? routeAttribute = subClass.GetCustomAttribute<WebSocketRouteAttribute>();

            if (routeAttribute is null)
                continue;

            object[] params_ = { };

            object controller = ActivatorUtilities.CreateInstance(serviceProvider, subClass, params_);

            ((WebSocketControllerBase)controller).UserIdentifier = userKey.Replace("/web-chat", "");

            _controllers.Add(routeAttribute.GetRouteName(), controller);
        }

        foreach (KeyValuePair<string, object> controller in _controllers)
        {
            var methods = controller.Value.GetType()
                .GetMethods()
                .Where(x => x.GetCustomAttribute<WebSocketEventNameAttribute>()?.GetRouteName() != null)
                .ToArray();

            foreach (var method in methods)
            {
                string? routeName = method.GetCustomAttribute<WebSocketEventNameAttribute>()?.GetRouteName();

                string key = controller.Key + "." + routeName;

                var value = new KeyValuePair<string, MethodInfo>(controller.Key, method);

                ControllerMappedMethods.Add(key, value);
            }
        }

    }

    public async Task<dynamic> InvokeAsync(GeneralEvent<JObject> generalEvent, string userKey)
    {
        if (generalEvent.EventName is null)
            return new GeneralEvent<string>
            {
                EventName = "Error",
                EventArgs = "Not Found"
            };


        KeyValuePair<string, MethodInfo> method;

        try
        {
            method = ControllerMappedMethods[generalEvent.EventName];
        }
        catch (Exception ex)
        {
            return new GeneralEvent<string>
            {
                EventName = "Error",
                EventArgs = "Not Found"
            };
        }

        ParameterInfo[] parameters = method.Value.GetParameters();

        try
        {
            object controller = _controllers[method.Key];

            if (controller == null)
            {
                return new GeneralEvent<string>
                {
                    EventName = "Error",
                    EventArgs = "404 Not Found"
                };
            }

            object result;

            if (parameters.Count() == 0)
            {
                if (generalEvent.EventArgs != null)
                {
                    return new GeneralEvent<string>
                    {
                        EventName = "Error",
                        EventArgs = "Too Many Parameters"
                    };
                }

                result = method.Value.Invoke(controller, null);
            }
            else
            {
                if (generalEvent.EventArgs == null)
                {
                    string params_ = "";

                    foreach (ParameterInfo param_ in parameters)
                    {
                        params_ += param_.Name + ":" + param_.ParameterType.Name + ",";
                    }

                    return new GeneralEvent<string>
                    {
                        EventName = "Error",
                        EventArgs = "Parameter Mismach Needs: " + params_.Substring(0, params_.Count() - 1)
                    };
                }

                Dictionary<string, object> namedParameters = new Dictionary<string, object>();

                foreach (var parameter in generalEvent.EventArgs)
                {
                    namedParameters.Add(parameter.Key, parameter.Value);
                }

                if (method.Value.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null)
                {
                    if (method.Value.ReturnType.IsGenericType)
                    {
                        Task<dynamic> a = (Task<dynamic>)method.Value.Invoke(controller,
                            MapParameters(method.Value, namedParameters));

                        await a;

                        result = a.Result;
                    }
                    else
                    {
                        Task a = (Task)method.Value.Invoke(controller, MapParameters(method.Value, namedParameters));

                        if (a != null)
                        {
                            await a;
                        }

                        result = null;
                    }
                }
                else
                {
                    result = method.Value.Invoke(controller, MapParameters(method.Value, namedParameters));
                }
            }

            return result;
        }
        catch (TargetParameterCountException ex)
        {
            string params_ = "";

            foreach (ParameterInfo param_ in parameters)
            {
                params_ += param_.Name + ", ";
            }

            return new GeneralEvent<string>
            {
                EventName = "Error",
                EventArgs = "Mismatching parameters are: " + params_
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred in web socket handler. Message: {ex.Message}. Details: {ex.StackTrace}");

            return new GeneralEvent<string>
            {
                EventName = "Error",
                EventArgs = ex.Message
            };
        }

    }

    private static object[] MapParameters(MethodBase method, Dictionary<string, object> namedParameters)
    {
        string[] paramNames = method.GetParameters().Select(p => p.Name).ToArray();

        object[] parameters = new object[paramNames.Length];

        for (int i = 0; i < parameters.Length; ++i)
        {
            parameters[i] = Type.Missing;
        }

        foreach (var item in namedParameters)
        {
            var paramName = item.Key;
            var paramIndex = Array.IndexOf(paramNames, paramName);
            if (paramIndex >= 0)
            {
                parameters[paramIndex] = Convert.ChangeType(item.Value,
                    method.GetParameters().First(p => p.Name == paramName).ParameterType);
            }
        }
        return parameters;
    }
}
