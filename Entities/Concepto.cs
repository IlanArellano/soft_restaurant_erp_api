
namespace Entities
{
    public class Concepto
    {
        public string IdProducto { get; set; }
        public string Descripcion { get; set; }
        public int Movimiento { get; set; }
        public double Cantidad { get; set; }
        public double PrecioUnitario { get; set; }
        public double ImporteSinImpuestos { get; set; }
        public double Descuento { get; set; }
        public List<VentaImpuesto> Impuestos { get; set; }
        public double? ImporteSinImpustos { get; set; }
    }
}
