using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace DataEntities
{
       public class Authentication
    {
        [Key]
        [JsonPropertyName("userid")]
        public Guid userid { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("password")]
        public string password { get; set; }

        [JsonPropertyName("username")]
        public string? username { get; set; }

        [JsonPropertyName("firstname")]
        public string? firstname { get; set; }

        [JsonPropertyName("lastname")]
        public string? lastname { get; set; }

        [JsonPropertyName("phone_number")]
        public string? phone_number{ get; set; }

        [JsonPropertyName("address")]
        public string address { get; set; }

        [JsonPropertyName("postcode")]
        public string postcode { get; set; }
    }


    [JsonSerializable(typeof(List<Authentication>))]
    public sealed partial class AuthSerializerContext : JsonSerializerContext
    {
    }
}
