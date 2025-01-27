using System.Data;

namespace Soloride.Application.Abstractions.Data;
public interface ISqlConnectionFactory
{
    IDbConnection CreateConnection();
}
