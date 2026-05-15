using Dapper;
using Facturas.Application.DTOs;
using Facturas.Application.Ports;
using Facturas.Domain.Aggregates;
using Facturas.Infrastructure.Persistence;
using System.Data;

namespace Facturas.Infrastructure.Repositories;

public class FacturaRepository : IFacturaRepository
{
    private readonly SqliteConnectionFactory _factory;

    public FacturaRepository(
        SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Factura>> GetAllAsync()
    {
        using var connection = _factory.CreateConnection();

        var sql = @"
            SELECT *
            FROM Facturas";

        return await connection.QueryAsync<Factura>(sql);
    }

    public async Task<Factura?> GetByNumeroDocumentoAsync(
        long numeroDocumento)
    {
        using var connection = _factory.CreateConnection();

        var sql = @"
            SELECT *
            FROM Facturas
            WHERE NumeroDocumento = @NumeroDocumento";

        return await connection.QueryFirstOrDefaultAsync<Factura>(
            sql,
            new
            {
                NumeroDocumento = numeroDocumento
            });
    }

    public async Task<long> CreateAsync(
        Factura factura)
    {
        using var connection = _factory.CreateConnection();

        connection.Open();

        using var transaction = connection.BeginTransaction();

        var facturaSql = @"
            INSERT INTO Facturas
            (
                NumeroDocumento,
                RutVendedor,
                DvVendedor,
                RutComprador,
                DvComprador,
                DireccionComprador,
                ComunaComprador,
                RegionComprador,
                TotalFactura
            )
            VALUES
            (
                @NumeroDocumento,
                @RutVendedor,
                @DvVendedor,
                @RutComprador,
                @DvComprador,
                @DireccionComprador,
                @ComunaComprador,
                @RegionComprador,
                @TotalFactura
            )";

        await connection.ExecuteAsync(
            facturaSql,
            factura,
            transaction);

        foreach (var detalle in factura.DetalleFactura)
        {
            var detalleSql = @"
                INSERT INTO DetalleFactura
                (
                    NumeroDocumento,
                    CantidadProducto,
                    DescripcionProducto,
                    ValorProducto,
                    TotalProducto
                )
                VALUES
                (
                    @NumeroDocumento,
                    @CantidadProducto,
                    @DescripcionProducto,
                    @ValorProducto,
                    @TotalProducto
                )";

            await connection.ExecuteAsync(
                detalleSql,
                new
                {
                    NumeroDocumento = factura.NumeroDocumento,
                    detalle.CantidadProducto,
                    DescripcionProducto = detalle.Producto.Descripcion,
                    ValorProducto = detalle.Producto.Valor,
                    detalle.TotalProducto
                },
                transaction);
        }

        transaction.Commit();

        return factura.NumeroDocumento;
    }

    public async Task UpdateAsync(
        Factura factura)
    {
        using var connection = _factory.CreateConnection();

        var sql = @"
            UPDATE Facturas
            SET
                DireccionComprador = @DireccionComprador,
                TotalFactura = @TotalFactura
            WHERE NumeroDocumento = @NumeroDocumento";

        await connection.ExecuteAsync(sql, factura);
    }

    public async Task DeleteAsync(
        long numeroDocumento)
    {
        using var connection = _factory.CreateConnection();

        var detalleSql = @"
            DELETE FROM DetalleFactura
            WHERE NumeroDocumento = @NumeroDocumento";

        await connection.ExecuteAsync(
            detalleSql,
            new
            {
                NumeroDocumento = numeroDocumento
            });

        var facturaSql = @"
            DELETE FROM Facturas
            WHERE NumeroDocumento = @NumeroDocumento";

        await connection.ExecuteAsync(
            facturaSql,
            new
            {
                NumeroDocumento = numeroDocumento
            });
    }

    public async Task<PagedResult<Factura>> BuscarAsync(
        long? numeroDocumento,
        long? rutVendedor,
        int page,
        int size)
    {
        using var connection = _factory.CreateConnection();

        var offset = (page - 1) * size;

        var sql = @"
            SELECT *
            FROM Facturas
            WHERE
            (@NumeroDocumento IS NULL
                OR NumeroDocumento = @NumeroDocumento)
            AND
            (@RutVendedor IS NULL
                OR RutVendedor = @RutVendedor)
            ORDER BY NumeroDocumento
            LIMIT @Size OFFSET @Offset";

        var facturas =
            await connection.QueryAsync<Factura>(
                sql,
                new
                {
                    NumeroDocumento = numeroDocumento,
                    RutVendedor = rutVendedor,
                    Size = size,
                    Offset = offset
                });

        var countSql = @"
            SELECT COUNT(*)
            FROM Facturas
            WHERE
            (@NumeroDocumento IS NULL
                OR NumeroDocumento = @NumeroDocumento)
            AND
            (@RutVendedor IS NULL
                OR RutVendedor = @RutVendedor)";

        var total =
            await connection.ExecuteScalarAsync<long>(
                countSql,
                new
                {
                    NumeroDocumento = numeroDocumento,
                    RutVendedor = rutVendedor
                });

        return new PagedResult<Factura>
        {
            Content = facturas,
            Page = page,
            Size = size,
            TotalElements = total,
            TotalPages =
                (int)Math.Ceiling(total / (double)size)
        };
    }
}