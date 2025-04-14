using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Ridely.Infrastructure.WebSockets.Attributes;

namespace Ridely.Infrastructure.WebSockets;
public sealed class WebSocketEventHandler
{
    private readonly Dictionary<string, (Type handlerType, MethodInfo methodInfo)> _eventMapper;
    private readonly IServiceProvider _serviceProvider;

    public WebSocketEventHandler(IServiceProvider serviceProvider)
    {
        _eventMapper = new();

        var handlerTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsAssignableTo(typeof(IWebSocketEventHandlerMarker)) && !x.IsInterface && !x.IsAbstract)
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var webSocketClass = handlerType.GetCustomAttribute<WebSocketClassAttribute>();

            if (webSocketClass is null) continue;

            MethodInfo[]? methodInfos = handlerType.GetMethods();

            foreach (var methodInfo in methodInfos)
            {
                var webSocketMethod = methodInfo.GetCustomAttribute<WebSocketMethodAttribute>();

                if (webSocketMethod is null) continue;

                string fullName = $"{webSocketClass.Name}.{webSocketMethod.Name}";

                _eventMapper[fullName] = (handlerType, methodInfo);
            }
        }

        _serviceProvider = serviceProvider;
    }

    public async ValueTask DispatchAsync(WebSocketEvent webSocketEvent)
    {
        if (!_eventMapper.TryGetValue(webSocketEvent.EventName, out var value))
            throw new ArgumentNullException("Websocket event not found");

        using var scope = _serviceProvider.CreateScope();

        var handlerInstance = scope.ServiceProvider.GetService(value.handlerType);

        if (handlerInstance is null)
            throw new ApplicationException("Unable to resolve service scope of handler");

        ParameterInfo[] parameterInfos = value.methodInfo.GetParameters();

        object[] arguments = new object[parameterInfos.Length];

        for (int i = 0; i < parameterInfos.Length; i++)
        {
            var parameterInfo = parameterInfos[i];

            if (webSocketEvent.EventArgs.TryGetValue(parameterInfo.Name, out var parameterValue))
            {
                arguments[i] = JsonSerializer.Deserialize(JsonSerializer.Serialize(parameterValue), parameterInfo.ParameterType);
            }
            else
                throw new ApplicationException($"Parameter {parameterInfo.Name} not found");
        }

        object result = value.methodInfo.Invoke(handlerInstance, arguments);

        if (result is Task taskResult)
            await taskResult;
    }
}
