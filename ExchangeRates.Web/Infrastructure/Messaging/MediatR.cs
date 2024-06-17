using ExchangeRates.Common.Messaging;
using ExchangeRates.Common.Messaging.Messages;
using MediatR;

namespace ExchangeRates.Web.Infrastructure.Messaging;

public class MediatR(IMediator mediator) : IMessageBroker
{
    public async Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command,
        CancellationToken ct = default) =>
        await mediator.Send(command, ct);

    public async Task<TResponse> SendQueryAsync<TResponse>(IQuery<TResponse> query,
        CancellationToken ct = default) =>
        await mediator.Send(query, ct);
}