using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace project1.Application.Helpers
{
    public static class PaginationHelper
    {
        public static void AddPaginationHeader(HttpResponse response, object pagedResult)
        {
            if (pagedResult == null) return;
            var type = pagedResult.GetType();
            var pageProp = type.GetProperty("Page");
            var pageSizeProp = type.GetProperty("PageSize");
            var totalProp = type.GetProperty("Total");
            if (pageProp == null || pageSizeProp == null || totalProp == null) return;

            var meta = new
            {
                page = pageProp.GetValue(pagedResult),
                pageSize = pageSizeProp.GetValue(pagedResult),
                total = totalProp.GetValue(pagedResult)
            };

            var json = JsonSerializer.Serialize(meta);
            response.Headers["X-Pagination"] = json;
        }
    }
}
