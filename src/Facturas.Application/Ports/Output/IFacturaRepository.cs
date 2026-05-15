using Facturas.Application.DTOs;
using Facturas.Domain.Aggregates;

namespace Facturas.Application.Ports;

public interface IFacturaRepository
{
    Task<IEnumerable<Factura>> GetAllAsync();

    Task<Factura?> GetByNumeroDocumentoAsync(
        long numeroDocumento);

    Task<long> CreateAsync(
        Factura factura);

    Task UpdateAsync(
        Factura factura);

    Task DeleteAsync(
        long numeroDocumento);

    Task<PagedResult<Factura>> BuscarAsync(
        long? numeroDocumento,
        long? rutVendedor,
        int page,
        int size);
}