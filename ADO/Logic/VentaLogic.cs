using Entities;

namespace ADO.Logic
{
    public class VentaLogic
    {
        public List<Venta> Test(VentaBody body)
        {
            return body.Ventas;
        }
    }
}
