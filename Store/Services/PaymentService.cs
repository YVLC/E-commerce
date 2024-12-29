using DataEntities;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;

namespace Store.Services
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;

        public PaymentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get a list of payments
        public async Task<List<Payment>> GetPayments()
        {
            List<Payment>? payments = null;
            var response = await _httpClient.GetAsync("/api/Payment");
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

        // Create a new payment
        public async Task<bool> CreatePayment(Payment payment)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Payment", payment);

            // Return true if the response is successful, otherwise false
            return response.IsSuccessStatusCode;
        }
    }
}