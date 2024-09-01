using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ERPInvoicePayment
    {
        public int docstatus { get; set; }
        public int idx { get; set; }
        public int @default { get; set; }
        public string mode_of_payment { get; set; }
        public string account { get; set; }
        public double amount { get; set; }
        public double base_amount { get; set; }
        public string parentfield { get; set; }
        public string parenttype { get; set; }
        public string doctype { get; set; }
    }
}
