using ADO.Logic;
using Entities;
using Entities.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace soft_restaurant_erp_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VentaController : Controller
    {

        private readonly VentaLogic logic;
        private readonly ERPInternalContext context;
        private readonly string baseApiUrl;

        public VentaController(VentaLogic _logic, IConfiguration configuration, ERPInternalContext context)
        {
            this.logic = _logic;
            this.baseApiUrl = configuration["ExternalApiSettings:ERP_URL"]!;
            this.context = context;
        }

        [HttpPost]
        public async Task<RemoteReponse> ProcessVenta([FromBody] VentaBody body)
        {
            string token;
            if (Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
                token = authorizationHeader!;
            else throw new Exception("No Token has provided");
            HTTPProps ERPprops = new HTTPProps()
            {
                baseURL = this.baseApiUrl,
            };
            ERPprops.headers.Add("Authorization", authorizationHeader!);
            var StoredOrderNumbers = context.OrderNumbers.Select(x => x.Value).ToList();
            var response = await logic.ProcessVenta(body, ERPprops, StoredOrderNumbers);

            var result = await response.Content.ReadAsStringAsync();
            Response.StatusCode = (int)response.StatusCode;
            if (Response.StatusCode == 200 && result != null)
            {
                context.OrderNumbers.AddRange(body.Ventas.Select(x => new IdRecord()
                {
                    Value = x.NumeroOrden
                }));
                context.SaveChanges();
            }
            return new()
            {
                success = response.IsSuccessStatusCode,
                response = response.IsSuccessStatusCode ? result : null,
                error = !response.IsSuccessStatusCode ? result : null
            };
        }

    }
}
