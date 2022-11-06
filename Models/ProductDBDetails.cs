using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevVirtualBankingAssistant.Models
{
    public class ProductDBDetails
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        
        public string TextMessage { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
