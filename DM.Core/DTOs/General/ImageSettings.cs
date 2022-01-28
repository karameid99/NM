using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.DTOs.General
{
    public class ImageSettings
    {
        public string RootPath { get; set; }
        public string WebSite { get; set; }
        public string[] ImageExtensions { get; set; }
        public string[] DocFilesExtensions { get; set; }

    }
}
