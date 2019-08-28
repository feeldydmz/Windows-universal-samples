using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Megazone.Cloud.Media.Domain
{
    public class ProjectListResponse
    {
        public ProjectListResponse(int totalCount, IEnumerable<Project> result)
        {
            TotalCount = totalCount;
            Results = result;
        }

        public int TotalCount { get; set; }
        
        public IEnumerable<Project> Results { get; set; }
    }
}
