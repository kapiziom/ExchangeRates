using MediatR;

namespace ExchangeRates.Common.Messaging.Messages;

public interface ICommand<out TRequestResult> : IRequest<TRequestResult>
{
}