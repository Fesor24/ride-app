using Ridely.Domain.Abstractions;

namespace Ridely.Domain.Common;
public sealed class Bank : Entity
{
    private Bank()
    {
        
    }

    public Bank(string name, string code, string type = "")
    {
        Name = name;
        Code = code;
        Type = type;
    }

    public string Name { get; private set; }
    public string Code { get; private set; }
    public string Type { get; private set; }

}
