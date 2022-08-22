using ExchangeRates.Common.Messaging.Messages;
using MediatR;

namespace ExchangeRates.Common.Messaging.Handlers;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    new Task<TResult> Handle(TQuery query, CancellationToken ct = default);
}