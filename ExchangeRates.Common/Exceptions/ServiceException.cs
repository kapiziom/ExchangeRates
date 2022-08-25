using System.Collections;
using System.Reflection;

namespace ExchangeRates.Common.Exceptions;

public class ServiceException : Exception
{
    public readonly ExceptionEnum Type;

    protected ServiceException(string message, ExceptionEnum type = ExceptionEnum.BadRequest) : base(message)
    {
        Type = type;
    }
}
