using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Core.Exceptions
{
    public class DMException : Exception
    {
        public DMException(string message) : base(message) { }
    }
}
