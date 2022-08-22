using ExchangeRates.Common.Messaging.Messages;

namespace ExchangeRates.Common.Messaging;

public interface IMessageBroker
{
    Task<TResponse> SendCommandAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);

    Task<TResponse> SendQueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
}
