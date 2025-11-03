using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Campaign.Watch.Application.Dtos.Campaign
{
    public class PaginatedResponse<T> where T : class
    {       
        public IEnumerable<T> Items { get; set; }

        public long TotalItems { get; set; }

        public PaginatedResponse(IEnumerable<T> items, long totalItems)
        {
            Items = items;
            TotalItems = totalItems;
        }
    }
}
