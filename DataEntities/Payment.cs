﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml;
using System.Text.Json.Serialization;

namespace DataEntities
{
       public class Payment
    {
                    //        PaymentID: Unique identifier for the payment
                    //OrderID: Reference to the associated order
                    //Amount: Payment amount
                    //PaymentMethod: Method of payment(e.g., credit card, PayPal)
                    //PaymentStatus: Status of the payment(e.g., completed, pending, failed)
                    //TransactionID: Identifier from the payment gateway
                    //PaymentDate: Date the payment was processe
        [JsonPropertyName("paymentid")]
        public Guid paymentId { get; set; }

        [JsonPropertyName("orderid")]
        public string? orderid { get; set; }

        [JsonPropertyName("amount")]
        public string? amount { get; set; }

        [JsonPropertyName("paymentmethod")]
        public string? paymentmethod { get; set; }

        [JsonPropertyName("paymentstatus")]
        public string? paymentstatus { get; set; }

        [JsonPropertyName("transactionid")]
        public string? transactionid { get; set; }

        [JsonPropertyName("paymentdate")]
        public string? date { get; set; }
    }


    [JsonSerializable(typeof(List<Payment>))]
    public sealed partial class SerializerContext : JsonSerializerContext
    {
    }
}
