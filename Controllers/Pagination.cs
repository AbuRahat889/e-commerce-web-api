
public class Pagination<T>
{
    public PaginationMeta Meta { get; set; } = new();
    public IEnumerable<T> Data { get; set; } = [];
}

public class PaginationMeta
{

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages =>
        (int)Math.Ceiling((double)TotalCount / PageSize);

}