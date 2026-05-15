namespace Facturas.Domain.Entities;

public class Producto
{
    public string Descripcion { get; private set; }

    public decimal Valor { get; private set; }

    private Producto()
    {
    }

    public Producto(
        string descripcion,
        decimal valor)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción es requerida.");

        if (valor <= 0)
            throw new ArgumentException("El valor debe ser mayor a cero.");

        Descripcion = descripcion;
        Valor = valor;
    }
}