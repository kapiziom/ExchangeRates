using ExchangeRates.Common.Messaging;
using ExchangeRates.Common.Messaging.Messages;
using MediatR;

namespace ExchangeRates.Web.Infrastructure.Messaging;

public class MediatR : IMessageBroker
{
    private readonly IMediator _mediator;

    public MediatR(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command,
        CancellationToken ct = default) =>
        await _mediator.Send(command, ct);

    public async Task<TResponse> SendQueryAsync<TResponse>(IQuery<TResponse> query,
        CancellationToken ct = default) =>
        await _mediator.Send(query, ct);
}