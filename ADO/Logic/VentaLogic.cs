using Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using System.Text;

namespace ADO.Logic
{
    public class VentaLogic
    {
        private static readonly string DEFAULT_SALES_COMPANY = "FiveAliance";
        private static readonly string DEFAULT_SALES_CUSTOMER = "ventasmitclan";
        private static ERPInvoice ToERPInvoice(VentaBody body)
        {
            string[] dateInfo = body.Ventas.Count > 0 ? body.Ventas[0].FechaVenta.Split('T', StringSplitOptions.TrimEntries) : ["2024-08-31", "13:00:49.276331"];
            ERPInvoice data = new()
            {
                company = DEFAULT_SALES_COMPANY,
                customer = DEFAULT_SALES_CUSTOMER,
                docstatus = 1,
                posting_date = dateInfo[0],
                posting_time = string.Concat(dateInfo[1].Where(c => !char.IsWhiteSpace(c))),
                is_pos = 1,
                pos_profile = "Mitclan",
                set_warehouse = "Productos M9 - ANM",
                update_stock = 1
            };

            foreach(Venta venta in body.Ventas)
            {
                foreach(Concepto concepto in venta.Conceptos)
                {
                    data.items.Add(new ERPInvoiceItem()
                    {
                        item_code = concepto.IdProducto,
                        qty = (int)concepto.Cantidad,
                        rate = concepto.PrecioUnitario
                    }); ;
                }
                for (int i = 0; i < venta.Pagos.Count; i++)
                {
                    Pago pago = venta.Pagos.ElementAt(i);
                    data.payments.Add(new ERPInvoicePayment()
                    {
                        account = "CAJA PRINCIPAL - FA",
                        amount = pago.Importe,
                        base_amount = pago.Importe,
                        @default = 1,
                        docstatus = 1,
                        doctype = "Sales Invoice Payment",
                        idx = i + 1,
                        mode_of_payment = "Efectivo", //pago.FormaPago,
                        parentfield = "payments",
                        parenttype = "Sales Invoice"
                    });
                }
               
            }

            return data;
        }
        
        public async Task<HttpResponseMessage> ProcessVenta(VentaBody body, string apiUrl, string apiToken)
        {
            // Crear una instancia de HttpClient
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", apiToken);

                ERPInvoice data = ToERPInvoice(body);

                var jsonData = JsonSerializer.Serialize(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
                
                return response;
            }
        }
    }
}
