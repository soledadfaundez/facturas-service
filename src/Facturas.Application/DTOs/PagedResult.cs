namespace Facturas.Application.DTOs;

public class PagedResult<T>
{
    public IEnumerable<T> Content { get; set; } = [];

    public int Page { get; set; }

    public int Size { get; set; }

    public long TotalElements { get; set; }

    public int TotalPages { get; set; }
}