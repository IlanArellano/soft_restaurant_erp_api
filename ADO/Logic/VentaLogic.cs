using Entities;
using System.Text.Json;
using System.Text;
using Entities.Responses;
using ADO.Extensions;
using System.Net;

namespace ADO.Logic
{
    public class VentaLogic
    {
        private static readonly string DEFAULT_SALES_COMPANY = "Mictlan9";
        private static readonly string DEFAULT_SALES_CUSTOMER = "VentasM9";

        private static List<string> StoredAvailableItemCodes = [];

        private async static Task processItem(VentaBody body, HTTPProps ERPprops)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.setHeadersFromDictionary(ERPprops.headers);

                foreach (var venta in body.Ventas)
                {
                    foreach (var item in venta.Conceptos)
                    {
                        string item_code = item.IdProducto.TrimStart('0');
                        bool isExistsInMemory = StoredAvailableItemCodes.Any(item => item.Equals(item_code));
                        if (isExistsInMemory) continue;
                        HttpResponseMessage getResponse = await httpClient.GetAsync($"{ERPprops.baseURL}/api/resource/Item/{item_code}");
                        if (getResponse.StatusCode == HttpStatusCode.Unauthorized || getResponse.StatusCode == HttpStatusCode.Forbidden) throw new Exception("El token de acceso al ERP No es válido");

                        if (getResponse.IsSuccessStatusCode)
                        {
                            StoredAvailableItemCodes.Add(item_code);
                            continue;
                        }

                        ERPCreateItem data = new()
                        {
                            item_code = item_code,
                            description = item.Descripcion,
                            item_group = "NuevosItems",
                            item_name = item.Descripcion,
                            stock_uom = "Unidad(es)",
                            valuation_rate = "0.01"
                        };
                        var jsonData = JsonSerializer.Serialize(data);
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        HttpResponseMessage createResponse = await httpClient.PostAsync($"{ERPprops.baseURL}/api/resource/Item", content);
                        if (createResponse.IsSuccessStatusCode)
                            StoredAvailableItemCodes.Add(item_code);
                        else
                            Console.WriteLine($"Ha ocurrido un error al añadir el item ${item_code}${Environment.NewLine}${await createResponse.Content.ReadAsStringAsync()}");
                    }
                }
            }
        }

        private static bool GenerateERPInvoice(VentaBody body, List<string> StoredOrderNumbers, out ERPInvoice? invoice)
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

            foreach (Venta venta in body.Ventas)
            {
                //Si tan solo una venta tiene un número de orden que ya se procesó se va a descartar esta petición
                if (StoredOrderNumbers.Any(x => x.Equals(venta.NumeroOrden)))
                {
                    invoice = null;
                    return false;
                }
                foreach (Concepto concepto in venta.Conceptos)
                {
                    data.items.Add(new ERPInvoiceItem()
                    {
                        item_code = concepto.IdProducto.TrimStart('0'),
                        qty = (int)concepto.Cantidad,
                        rate = concepto.PrecioUnitario
                    }); ;
                }
                for (int i = 0; i < venta.Pagos.Count; i++)
                {
                    Pago pago = venta.Pagos.ElementAt(i);
                    data.payments.Add(new ERPInvoicePayment()
                    {
                        account = "CAJA PRINCIPAL - ANM",
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

            invoice = data;
            return true;
        }

        public async Task<HttpResponseMessage> ProcessVenta(VentaBody body, HTTPProps ERPprops, List<string> StoredOrderNumbers)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.setHeadersFromDictionary(ERPprops.headers);
                bool canGenerateInvoice = GenerateERPInvoice(body, StoredOrderNumbers, out ERPInvoice? data);
                if (!canGenerateInvoice)
                {
                    return new HttpResponseMessage(HttpStatusCode.OK);
                }
                await processItem(body, ERPprops);
                var jsonData = JsonSerializer.Serialize(data);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync($"{ERPprops.baseURL}/api/resource/Sales Invoice", content);

                return response;
            }
        }
    }
}
