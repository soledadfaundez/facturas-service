namespace Facturas.Application.DTOs;

public class FacturaRequest
{
    public long NumeroDocumento { get; set; }

    public long RutVendedor { get; set; }

    public string DvVendedor { get; set; } = string.Empty;

    public long RutComprador { get; set; }

    public string DvComprador { get; set; } = string.Empty;

    public string DireccionComprador { get; set; } = string.Empty;

    public int ComunaComprador { get; set; }

    public int RegionComprador { get; set; }

    public List<DetalleFacturaRequest> DetalleFactura { get; set; } = [];
}