namespace Store.Services
{
    public class OrderingService
    {
        private readonly HttpClient _httpClient;
        private readonly string remoteServiceBaseUrl = "/api/Orders/";

        // Inject HttpClient via constructor
        public OrderingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Get orders with their associated OrderItems
        public Task<OrderRecord[]> GetOrders()
        {
            // Fetch orders with OrderItems using the endpoint we defined previously
            return _httpClient.GetFromJsonAsync<OrderRecord[]>(remoteServiceBaseUrl)!;
        }

        public Task<OrderRecord[]> GetOrdersByUserId(Guid userId)
        {
            // Fetch orders with OrderItems using the endpoint we defined previously
            return _httpClient.GetFromJsonAsync<OrderRecord[]>($"/api/Orders/{userId}")!;
        }

        // Create a new order (includes OrderItems)
        public Task CreateOrder(OrderRecord request, Guid requestId)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Post, remoteServiceBaseUrl);
            requestMessage.Headers.Add("x-requestid", requestId.ToString());
            requestMessage.Content = JsonContent.Create(request); // Serialize the order record
            return _httpClient.SendAsync(requestMessage);
        }
    }

    // Refactor the OrderRecord to match the full structure, including OrderItems
    public record OrderRecord(
        Guid OrderNumber,
        DateTime Date,
        string Status,
        string City,     
        string Country, 
        string Street,
        string PostalCode,
        Guid ClientId,
        decimal Total,
        OrderItem[] OrderItems);  // Include the list of OrderItems

    // Refactor the OrderItem to match the response structure
    public record OrderItem(
        Guid Id,
        string ProductName,
        decimal UnitPrice,
        int Units,
        string PictureUrl);
}