using Facturas.Application.DTOs;

namespace Facturas.Application.Ports;

public interface IFacturaService
{
    Task<IEnumerable<FacturaResponse>> GetAllAsync();

    Task<FacturaResponse?> GetByNumeroDocumentoAsync(
        long numeroDocumento);

    Task<long> CreateAsync(
        FacturaRequest request);

    Task UpdateAsync(
        long numeroDocumento,
        FacturaRequest request);

    Task DeleteAsync(
        long numeroDocumento);

    Task<PagedResult<FacturaResponse>> BuscarAsync(
        long? numeroDocumento,
        long? rutVendedor,
        int page,
        int size);
}