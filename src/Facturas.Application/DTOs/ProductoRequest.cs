namespace Facturas.Application.DTOs;

public class ProductoRequest
{
    public string Descripcion { get; set; } = string.Empty;

    public decimal Valor { get; set; }
}