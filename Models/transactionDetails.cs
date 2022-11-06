using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevVirtualBankingAssistant.Models
{
    
    public class transactionDetails
    {
        public List<Amount> transactionList = new List<Amount>();
    }

    public class Amount
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string toUpiID { get; set; }
        public string toName { get; set; }
        public string time { get; set; }
        public string amount { get; set; }
        public string Remarks { get; set; }
        public string rewardPoint { get; set; }
    }
}
