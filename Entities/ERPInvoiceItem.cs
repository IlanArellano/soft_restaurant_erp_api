using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ERPInvoiceItem
    {
        public string item_code { get; set; }
        public int qty { get; set; }
        public double rate { get; set; }
    }
}
