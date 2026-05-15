using System.Text.Json;
using Dapper;
using Facturas.Domain.Aggregates;
using Facturas.Infrastructure.Persistence;

namespace Facturas.Infrastructure.Seeders;

public class DataSeeder
{
    private readonly SqliteConnectionFactory _factory;

    public DataSeeder(
        SqliteConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task SeedAsync(
        List<Factura> facturas)
    {
        using var connection = _factory.CreateConnection();

        var count =
            await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Facturas");

        if (count > 0)
            return;

        if (facturas is null)
            return;

        foreach (var factura in facturas)
        {
            factura.CalcularTotal();

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
                factura);

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
                        NumeroDocumento =
                            factura.NumeroDocumento,

                        detalle.CantidadProducto,

                        DescripcionProducto =
                            detalle.Producto.Descripcion,

                        ValorProducto =
                            detalle.Producto.Valor,

                        detalle.TotalProducto
                    });
            }
        }
    }
}