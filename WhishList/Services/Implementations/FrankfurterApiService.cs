using WhishList.Services.Interfaces;

namespace WhishList.Services.Implementations;

public class FrankfurterApiService : ICurrencyConverter
{
    private readonly HttpClient _httpClient;
    // Base URL: https://api.frankfurter.app/latest

    public FrankfurterApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> ConvertAsync(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency)
            return amount;

        // API Endpoint: /latest?amount=100&from=EUR&to=DKK
        string url = $"/latest?amount={amount}&from={fromCurrency}&to={toCurrency}";
        var response = await _httpClient.GetFromJsonAsync<FrankfurterResponse>(url);

        // Check for conversion result
        if (response?.Rates?.TryGetValue(toCurrency, out decimal convertedAmount) == true)
        {
            return convertedAmount;
        }

        // Fallback or error handling
        return amount; // Or throw an exception
    }
}

// Simple DTO for the API response structure
public class FrankfurterResponse
{
    public string @Base { get; set; }
    public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    // ... other properties not needed for conversion
}