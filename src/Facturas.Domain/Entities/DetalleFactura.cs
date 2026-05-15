namespace Facturas.Domain.Entities;

public class DetalleFactura
{
    public decimal CantidadProducto { get; private set; }

    public Producto Producto { get; private set; }

    public decimal TotalProducto { get; private set; }

    private DetalleFactura()
    {
    }

    public DetalleFactura(
        decimal cantidadProducto,
        Producto producto)
    {
        if (cantidadProducto <= 0)
            throw new ArgumentException(
                "La cantidad debe ser mayor a cero.");

        Producto = producto
            ?? throw new ArgumentNullException(nameof(producto));

        CantidadProducto = cantidadProducto;

        CalcularTotal();
    }

    public void CalcularTotal()
    {
        TotalProducto = CantidadProducto * Producto.Valor;
    }
}