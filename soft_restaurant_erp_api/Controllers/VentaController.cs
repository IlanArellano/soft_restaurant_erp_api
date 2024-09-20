using ADO.Logic;
using Entities;
using Entities.Responses;
using Microsoft.AspNetCore.Mvc;

namespace soft_restaurant_erp_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VentaController : Controller
    {

        private readonly VentaLogic logic;
        private readonly string baseApiUrl;

        public VentaController(VentaLogic _logic, IConfiguration configuration)
        {
            this.logic = _logic;
            this.baseApiUrl = configuration["ExternalApiSettings:ERP_URL"]!;
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
            var response = await logic.ProcessVenta(body, ERPprops);
               
            var result = await response.Content.ReadAsStringAsync();
            Response.StatusCode = (int)response.StatusCode;
                return new()
                {
                    success = response.IsSuccessStatusCode,
                    response = response.IsSuccessStatusCode ? result : null,
                    error = !response.IsSuccessStatusCode ? result : null
                };
        }

        [HttpGet]
        public async Task<bool> Test()
        {
            return true;
        }
    }
}
