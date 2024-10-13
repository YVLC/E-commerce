using DataEntities;
using System.Text.Json;

namespace Store.Services;

public class PaymentService
{
    HttpClient httpClient;
    public PaymentService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
    public async Task<List<Payment>> GetPayments()
    {
        List<Payment>? payments = null;
        var response = await httpClient.GetAsync("/api/Payment");
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            payments = await response.Content.ReadFromJsonAsync(SerializerContext.Default.ListPayment);
        }

        return payments ?? new List<Payment>();
    }
    
}