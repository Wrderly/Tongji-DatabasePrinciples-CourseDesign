using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("baseapi/[controller]")]
    [ApiController]
    internal class BaseApiController : ControllerBase
    {
        [HttpGet("basefunc/{str}")]
        public string BaseFunc(string str)
        {
            return str;
        }
    }
}
