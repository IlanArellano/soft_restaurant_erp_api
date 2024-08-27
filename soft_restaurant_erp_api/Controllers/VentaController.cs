using ADO.Logic;
using Entities;
using Microsoft.AspNetCore.Mvc;

namespace soft_restaurant_erp_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VentaController : Controller
    {

        private readonly VentaLogic logic;

        public VentaController(VentaLogic _logic)
        {
            logic = _logic;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<List<Venta>> ProcessVenta([FromBody] VentaBody body)
        {
            return logic.Test(body);
        }
    }
}
