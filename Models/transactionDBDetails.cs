using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevVirtualBankingAssistant.Models
{
    public class transactionDBDetails
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Email { get; set; }
        /*public string StoreName { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        
         public string Email { get; set; }
        */
        
        public string Name { get; set; }
        //public string Location { get; set; }

        public string toUpiID { get; set; }
        public string toName { get; set; }

        public string time { get; set; }
        public string amount { get; set; }
        //public string Image { get; set; }
        public string Remarks { get; set; }
        public string rewardPoint { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}