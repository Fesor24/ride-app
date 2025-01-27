using System.Data;

namespace Ridely.Application.Abstractions.Data;
public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
