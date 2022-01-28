using DM.Core.Movments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Infrastructure.Modules.Movments
{
   public interface IMovmentService
    {
        Task SingleProductMovmant(SingleProductMovmantDto dto, string userId);
        Task MultipleProductMovmant(MultipleProductMovmantDto dto, string userId);
    }
}
