using MediatR;
using Ridely.Domain.Abstractions;

namespace Ridely.Application.Features.Common.Banks.Command.CreateMany;
public class CreateBanksCommand : IRequest<Result<bool>>
{
    public int UserId { get; set; }
    public List<CreateBank> Banks { get; set; }
}

public class CreateBank
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Type { get; set; }
}
