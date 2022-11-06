using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevVirtualBankingAssistant.Models
{
    public class InvoiceData
    {
        public string InvoiceNumber { get; set; }

        public string CustomerName { get; set; }

        /*public Entity Invoice { get; set; }

        public List<Entity> InvoiceLines { get; set; }*/
    }
}
