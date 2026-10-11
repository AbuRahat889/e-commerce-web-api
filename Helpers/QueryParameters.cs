using System;
using System.ComponentModel.DataAnnotations;

namespace e_commerce_web_api.Helpers;

public class QueryParameters
{

    public int MaxSize = 100;
    // [Range(1, int.MaxValue, ErrorMessage = "page mus be at least 1.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public string? SortOrder { get; set; }

    public QueryParameters Validation()
    {
        if (PageNumber < 1)
        {
            PageNumber = 1;
        }
        if (PageSize < 1)
        {
            PageSize = 10;
        }
        if (PageSize > MaxSize)
        {
            PageSize = MaxSize;
        }
        return this;
    }
}
