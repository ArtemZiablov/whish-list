namespace WhishList.Services.Interfaces;

public interface ICurrencyConverter
{
    // Converts an amount from one currency to another
    Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency);
}