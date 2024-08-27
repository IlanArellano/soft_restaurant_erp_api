
namespace Entities
{
    public class Venta
    {
        public string Estacion { get; set; }
        public string Almacen { get; set; }
        public string FechaVenta { get; set; }
        public string NumeroOrden { get; set; }
        public string IdCliente { get; set; }
        public string IdUsuario { get; set; }
        public double Total { get; set; }
        public string Area { get; set; }
        public List<Concepto> Conceptos { get; set; }
        public List<Pago> Pagos { get; set; }
    }
}
