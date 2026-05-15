namespace Facturas.Application.DTOs;

public class DetalleFacturaRequest
{
    public decimal CantidadProducto { get; set; }

    public ProductoRequest Producto { get; set; } = default!;
}