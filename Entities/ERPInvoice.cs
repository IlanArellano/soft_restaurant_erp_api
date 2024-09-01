using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ERPInvoice
    {
        public string customer { get; set; }
        public int docstatus { get; set; }
        public string posting_date { get; set; }
        public string posting_time { get; set; }
        public int is_pos { get; set; }
        public string company { get; set; }
        public string pos_profile { get; set; }
        public int update_stock { get; set; }
        public string set_warehouse { get; set; }
        public List<ERPInvoiceItem> items { get; set; } = [];
        public List<ERPInvoicePayment> payments { get; set; } = [];
    }
}
