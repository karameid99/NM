using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DM.Core.DTOs.General
{
	public class APIResponseError : APIResponse<object>
	{
		public object Error { get; set; }
	}
}
