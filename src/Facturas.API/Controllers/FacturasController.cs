using Facturas.Application.DTOs;
using Facturas.Application.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Facturas.API.Controllers;

[ApiController]
[Route("api/facturas")]
[Authorize]
public class FacturasController : ControllerBase
{
    private readonly IFacturaService _service;

    public FacturasController(
        IFacturaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("{numeroDocumento}")]
    public async Task<IActionResult> GetByNumeroDocumento(
        long numeroDocumento)
    {
        var result =
            await _service.GetByNumeroDocumentoAsync(
                numeroDocumento);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] FacturaRequest request)
    {
        var id = await _service.CreateAsync(request);

        return Created(
            $"/api/facturas/{id}",
            new
            {
                NumeroDocumento = id
            });
    }

    [HttpPut("{numeroDocumento}")]
    public async Task<IActionResult> Update(
        long numeroDocumento,
        [FromBody] FacturaRequest request)
    {
        await _service.UpdateAsync(
            numeroDocumento,
            request);

        return NoContent();
    }

    [HttpDelete("{numeroDocumento}")]
    public async Task<IActionResult> Delete(
        long numeroDocumento)
    {
        await _service.DeleteAsync(numeroDocumento);

        return NoContent();
    }

    [Authorize(Roles = "AUDITOR")]
    [HttpGet("busqueda")]
    public async Task<IActionResult> Buscar(
        [FromQuery] long? numeroDocumento,
        [FromQuery] long? rutVendedor,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10)
    {
        var result =
            await _service.BuscarAsync(
                numeroDocumento,
                rutVendedor,
                page,
                size);

        return Ok(result);
    }
}