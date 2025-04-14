using Ridely.Application.Abstractions.Messaging;

namespace Ridely.Application.Features.Admin.Driver.UpgradeCab;
public sealed record UpgradeCabCommand(long CabId) : ICommand;
