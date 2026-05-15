using Facturas.Application.DTOs;
using Facturas.Application.Ports;
using Facturas.Domain.Aggregates;
using Facturas.Domain.Entities;
using Facturas.Domain.Exceptions;

namespace Facturas.Application.Services;

public class FacturaService : IFacturaService
{
    private readonly IFacturaRepository _repository;

    public FacturaService(
        IFacturaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<FacturaResponse>> GetAllAsync()
    {
        var facturas = await _repository.GetAllAsync();

        return facturas.Select(MapResponse);
    }

    public async Task<FacturaResponse?> GetByNumeroDocumentoAsync(
        long numeroDocumento)
    {
        var factura =
            await _repository.GetByNumeroDocumentoAsync(
                numeroDocumento);

        return factura is null
            ? null
            : MapResponse(factura);
    }

    public async Task<long> CreateAsync(
        FacturaRequest request)
    {
        var factura = MapDomain(request);

        return await _repository.CreateAsync(factura);
    }

    public async Task UpdateAsync(
        long numeroDocumento,
        FacturaRequest request)
    {
        var factura =
            await _repository.GetByNumeroDocumentoAsync(
                numeroDocumento);

        if (factura is null)
            throw new BusinessException(
                "Factura no encontrada.");

        var nuevaFactura = MapDomain(request);

        await _repository.UpdateAsync(nuevaFactura);
    }

    public async Task DeleteAsync(
        long numeroDocumento)
    {
        await _repository.DeleteAsync(numeroDocumento);
    }

    public async Task<PagedResult<FacturaResponse>> BuscarAsync(
        long? numeroDocumento,
        long? rutVendedor,
        int page,
        int size)
    {
        var result =
            await _repository.BuscarAsync(
                numeroDocumento,
                rutVendedor,
                page,
                size);

        return new PagedResult<FacturaResponse>
        {
            Content = result.Content.Select(MapResponse),
            Page = result.Page,
            Size = result.Size,
            TotalElements = result.TotalElements,
            TotalPages = result.TotalPages
        };
    }

    private static Factura MapDomain(
        FacturaRequest request)
    {
        var detalles =
            request.DetalleFactura.Select(d =>
                new DetalleFactura(
                    d.CantidadProducto,
                    new Producto(
                        d.Producto.Descripcion,
                        d.Producto.Valor)))
            .ToList();

        return new Factura(
            request.NumeroDocumento,
            request.RutVendedor,
            request.DvVendedor,
            request.RutComprador,
            request.DvComprador,
            request.DireccionComprador,
            request.ComunaComprador,
            request.RegionComprador,
            detalles);
    }

    private static FacturaResponse MapResponse(
        Factura factura)
    {
        return new FacturaResponse
        {
            NumeroDocumento = factura.NumeroDocumento,
            RutVendedor = factura.RutVendedor,
            TotalFactura = factura.TotalFactura,
            DireccionComprador = factura.DireccionComprador
        };
    }
}