using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Image
{
    public interface IImageService
    {
        public Task<string> Save(IFormFile file, string Foldername);
    }
}
