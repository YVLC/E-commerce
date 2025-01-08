using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntities
{
    public record OrderRecords(
        Guid OrderNumber,
        DateTime Date,
        string Status,
        string City,
        string Country,
        string Street,
        string PostalCode,
        Guid ClientId,
        decimal Total,
        OrderItem[] OrderItems);
}
