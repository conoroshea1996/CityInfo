using CityInfo.Api.Context;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.Api.Controllers
{
    [ApiController]
    [Route("api/testdatabase")]
    public class DummyController : ControllerBase
    {
        
        private readonly CityInfoContext _context;
        //inject db context into class
        public DummyController(CityInfoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult TestDatabase()
        {
            return Ok();
        }

    }
}
