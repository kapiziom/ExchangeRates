using ExchangeRates.Common.Messaging.Messages;
using MediatR;

namespace ExchangeRates.Common.Messaging.Handlers;

public interface ICommandHandler<in TCommand, TRequestResult> : IRequestHandler<TCommand, TRequestResult>
    where TCommand : ICommand<TRequestResult>
{
    new Task<TRequestResult> Handle(TCommand command, CancellationToken ct = default);
}