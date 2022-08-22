using MediatR;

namespace ExchangeRates.Common.Messaging.Messages;

public interface IQuery<out TRequestResult> : IRequest<TRequestResult>
{
}