using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Megazone.HyperSubtitleEditor.Presentation.Recently
{
    internal class RecentlyLoader
    {
        private List<RecentlyItem> _recentlyItems = new List<RecentlyItem>();
        public IEnumerable<RecentlyItem> RecentlyItems => _recentlyItems;
        

        public void Add(RecentlyItem item)
        {
            if(_recentlyItems==null)
                _recentlyItems= new List<RecentlyItem>();
            _recentlyItems.Add(item);

            Save();
        }

        public void Load()
        {
            var jsonString = "";
            _recentlyItems = JsonConvert.DeserializeObject<List<RecentlyItem>>(jsonString);
        }

        private void Save()
        {
            var jsonString = JsonConvert.SerializeObject(_recentlyItems);
            
        }

        private string GetSavePath()
        {
            var localAppdata = "";
            return $"{localAppdata}\\Recently.dat";
        }
    }
}
