using System.Diagnostics;
using ExchangeRates.Common.Messaging;
using ExchangeRates.Services.Currency.Dto;
using ExchangeRates.Services.Currency.Queries;
using ExchangeRates.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeRates.Web.Controllers;

public class CurrencyController : Controller
{
    private readonly IMessageBroker _messageBroker;

    public CurrencyController(IMessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    public async Task<ActionResult<CurrencyDetailDto>> Index(
        [FromQuery] string sortBy, 
        [FromQuery] string sortOrder,
        CancellationToken ct = default)
    {
        ViewData["SortOrder"] = sortOrder;
        ViewData["SortBy"] = sortBy;
        
        var model = new CurrencyDetailDto();

        try
        {
            model = await _messageBroker.SendQueryAsync(new GetDefaultCurrency(sortBy, sortOrder), ct);
        }
        catch (Exception ex)
        {
            return View(null);
        }
        
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}