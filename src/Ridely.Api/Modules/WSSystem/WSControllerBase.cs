namespace Modules.ChatSystem
{
    //public partial class  WSControllerBase{
    //    public string UserID { get; set; }

    //    public virtual void Disconnect(string UserId) {
    //        Console.WriteLine($"Disconnected UID>> :{UserId}");
    //    }
    //}

    //public class WebSocketRequestHandler{
    //    private Dictionary<string,object> controllers; 
    //    public Dictionary<string,KeyValuePair<string,MethodInfo>> ControllerMappedMethods;
    //    private IEnumerable<Type> subclasses;
    //    public WebSocketRequestHandler(IServiceProvider provider,string userID)
    //    {

    //        controllers = new  Dictionary<string, object>();
    //        ControllerMappedMethods = new Dictionary<string, KeyValuePair<string, MethodInfo>>();
    //        subclasses = Assembly
    //           .GetAssembly(typeof(WSControllerBase))
    //           .GetTypes()
    //           .Where(t => t.IsSubclassOf(typeof(WSControllerBase)));
    //        foreach (var subclass in subclasses)
    //        {
    //            WebsocketRouteAttribute? attr = subclass.GetCustomAttribute<WebsocketRouteAttribute>();
    //            if (attr == null)
    //            {
    //                continue;
    //            }
    //            object[] params_ = { 

    //            };
    //            object controller =
    //                ActivatorUtilities.CreateInstance(provider, subclass, params_);

    //            ((WSControllerBase) controller).UserID = userID.Replace("/web-chat","");

    //            controllers.Add(attr.GetRouteName(), controller);
    //        }

    //        foreach (KeyValuePair<string, object> controller in controllers)
    //        {
    //            var methods = controller.Value.GetType().GetMethods()
    //                  .Where(m => m.GetCustomAttribute<WebsocketEventNameAttribute>() != null)
    //                  .ToArray();
    //            foreach (var method in methods)
    //            {
    //                string? Ename = method.GetCustomAttribute<WebsocketEventNameAttribute>().GetRouteName();
    //                ControllerMappedMethods.Add(controller.Key+"."+ Ename,new KeyValuePair<string, MethodInfo>(
    //                    controller.Key, method));
    //            }
    //        }


    //    }

    //    public async Task<dynamic> InvokeEvent(GeneralEvent<JObject> Event,string UserId)
    //    {
    //        if (Event.EventName == null)
    //        {
    //            return new GeneralEvent<string>
    //            {
    //                EventName = "Error",
    //                EventArgs = "404 Not Found"
    //            };
    //        }

    //        KeyValuePair<string, MethodInfo> method;


    //        try
    //        {
    //            method = ControllerMappedMethods[Event.EventName];

    //        }
    //        catch (KeyNotFoundException E)
    //        {
    //            return new GeneralEvent<string>
    //            {
    //                EventName = "Error",
    //                EventArgs = "Not Found"
    //            };
    //        }
    //        ParameterInfo[] param = method.Value.GetParameters();
    //        try {

    //            object controller = controllers[method.Key];
    //            if (controller == null)
    //            {
    //                return new GeneralEvent<string>
    //                {
    //                    EventName = "Error",
    //                    EventArgs = "404 Not Found"
    //                };
    //            }
    //            object result;
    //            if (param.Count() == 0)
    //            {
    //                if(Event.EventArgs != null)
    //                {

    //                    return new GeneralEvent<string>
    //                    {
    //                        EventName = "Error",
    //                        EventArgs = "Too Many Parameters"
    //                    };
    //                }
    //                result = method.Value.Invoke(controller, null);
    //            }
    //            else
    //            {
    //                if (Event.EventArgs == null)
    //                {
    //                    string params_ = "";
    //                    foreach (ParameterInfo param_ in param)
    //                    {
    //                        params_ += param_.Name + ":" + param_.ParameterType.Name + ",";
    //                    }
    //                    return new GeneralEvent<string>
    //                    {
    //                        EventName = "Error",
    //                        EventArgs = "Parameter Mismach Needs: "+ params_.Substring(0,params_.Count()-1)
    //                    };
    //                }
    //                Dictionary<string, object> namedParameters = new Dictionary<string, object>();

    //                foreach (var parameter in Event.EventArgs)
    //                {
    //                    namedParameters.Add(parameter.Key, parameter.Value);
    //                }
    //                if(method.Value.ReturnType.GetMethod(nameof(Task.GetAwaiter)) != null)
    //                {
    //                    if ( method.Value.ReturnType.IsGenericType)
    //                    {
    //                        Task<dynamic> a = (Task<dynamic>) method.Value.Invoke(controller, MapParameters(method.Value, namedParameters));
    //                        await a;
    //                        result = a.Result;
    //                    }
    //                    else
    //                    {
    //                        Task a = (Task)method.Value.Invoke(controller, MapParameters(method.Value, namedParameters));
    //                        if(a != null)
    //                        {
    //                            await a;

    //                        }
    //                        result = null;

    //                    }
    //                }
    //                else
    //                {
    //                    result = method.Value.Invoke(controller, MapParameters(method.Value, namedParameters));
    //                }


    //            }
    //            return result;
    //        }catch(TargetParameterCountException e)
    //        {

    //            string params_ = "";
    //            foreach (ParameterInfo param_ in param)
    //            {
    //                params_ += param_.Name+", ";
    //            }
    //            return new GeneralEvent<string>
    //            {
    //                EventName = "Error",
    //                EventArgs = "Mismatching Parameters Parameters are :" + params_
    //            };
    //        }catch (Exception e) {
    //            Console.WriteLine(e.ToString());
    //            return new GeneralEvent<string>
    //            {
    //                EventName = "Error",
    //                EventArgs = e.Message
    //            };
    //        }

    //    }

    //    private object[] MapParameters(MethodBase method, Dictionary<string, object> namedParameters)
    //    {
    //        string[] paramNames = method.GetParameters().Select(p => p.Name).ToArray();
    //        object[] parameters = new object[paramNames.Length];
    //        for (int i = 0; i < parameters.Length; ++i)
    //        {
    //            parameters[i] = Type.Missing;
    //        }
    //        foreach (var item in namedParameters)
    //        {
    //            var paramName = item.Key;
    //            var paramIndex = Array.IndexOf(paramNames, paramName);
    //            if (paramIndex >= 0)
    //            {
    //                parameters[paramIndex] = Convert.ChangeType(item.Value, method.GetParameters().First(p=>p.Name == paramName).ParameterType);
    //            }
    //        }
    //        return parameters;
    //    }




    //}






    /// <summary>
    ///  Attribute Marks Routes for Events in The Web Socket
    /// </summary>
    //[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    //public class WebsocketRouteAttribute : System.Attribute
    //{
    //    private string EventName;

    //    public WebsocketRouteAttribute(string EventName)
    //    {
    //        this.EventName = EventName;
    //    }
    //    public string GetRouteName() {
    //        return EventName;
    //    }
    //    public bool MatchRoutName(string? RouteName)
    //    {
    //        return RouteName.StartsWith(EventName);
    //    }

    //}

    ///// <summary>
    ///// 
    ///// </summary>
    //[System.AttributeUsage(System.AttributeTargets.Method)]
    //public class WebsocketEventNameAttribute : System.Attribute
    //{
    //    private string EventName;

    //    public WebsocketEventNameAttribute(string EventName)
    //    {
    //        this.EventName = EventName;
    //    }
    //    public string GetRouteName()
    //    {
    //        return EventName;
    //    }
    //}

}
