using Facturas.Domain.Entities;
using Facturas.Domain.Exceptions;

namespace Facturas.Domain.Aggregates;

public class Factura
{
    public long NumeroDocumento { get; private set; }

    public long RutVendedor { get; private set; }

    public string DvVendedor { get; private set; }

    public long RutComprador { get; private set; }

    public string DvComprador { get; private set; }

    public string DireccionComprador { get; private set; }

    public int ComunaComprador { get; private set; }

    public int RegionComprador { get; private set; }

    public decimal TotalFactura { get; private set; }

    public List<DetalleFactura> DetalleFactura { get; private set; }

    private Factura()
    {
        DetalleFactura = [];
    }

    public Factura(
        long numeroDocumento,
        long rutVendedor,
        string dvVendedor,
        long rutComprador,
        string dvComprador,
        string direccionComprador,
        int comunaComprador,
        int regionComprador,
        List<DetalleFactura> detalleFactura)
    {
        if (numeroDocumento <= 0)
            throw new BusinessException(
                "Número documento inválido.");

        if (rutVendedor <= 0)
            throw new BusinessException(
                "Rut vendedor inválido. " + rutVendedor);

        if (rutComprador <= 0)
            throw new BusinessException(
                "Rut comprador inválido. " + rutComprador);

        if (detalleFactura is null || !detalleFactura.Any())
            throw new BusinessException(
                "La factura debe tener detalles.");

        NumeroDocumento = numeroDocumento;
        RutVendedor = rutVendedor;
        DvVendedor = dvVendedor;
        RutComprador = rutComprador;
        DvComprador = dvComprador;
        DireccionComprador = direccionComprador;
        ComunaComprador = comunaComprador;
        RegionComprador = regionComprador;
        DetalleFactura = detalleFactura;

        CalcularTotalFactura();
    }

    public void CalcularTotal()
    {
        TotalFactura =
            DetalleFactura.Sum(
                x => x.TotalProducto);
    }

    public void CalcularTotalFactura()
    {
        foreach (var detalle in DetalleFactura)
        {
            detalle.CalcularTotal();
        }

        TotalFactura = DetalleFactura.Sum(x => x.TotalProducto);
    }

    public void ActualizarDireccion(
        string nuevaDireccion)
    {
        if (string.IsNullOrWhiteSpace(nuevaDireccion))
            throw new BusinessException(
                "La dirección es requerida.");

        DireccionComprador = nuevaDireccion;
    }
}