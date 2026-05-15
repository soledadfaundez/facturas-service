namespace Facturas.Application.DTOs;

public class FacturaResponse
{
    public long NumeroDocumento { get; set; }

    public long RutVendedor { get; set; }

    public decimal TotalFactura { get; set; }

    public string DireccionComprador { get; set; } = string.Empty;
}