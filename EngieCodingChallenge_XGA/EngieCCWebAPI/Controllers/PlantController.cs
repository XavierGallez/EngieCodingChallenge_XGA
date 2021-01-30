using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngieCCWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlantController : ControllerBase
    {

        private readonly ILogger<PlantController> _logger;

        public PlantController(ILogger<PlantController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/productionplan")]
        public IEnumerable<EngieCCComputer.LoadResponse> Optimize(EngieCCComputer.Payload payload)
        {
            try
            {
                var loadComputer = new EngieCCComputer.LoadComputer(payload);
                var plantsAndCost = loadComputer.Optimize();
                return plantsAndCost.Item1;
            }
            catch (Exception e)
            {
                _logger.LogError("Something went wrong. Details:\n" + e.Message);
                return new List<EngieCCComputer.LoadResponse>();
            }
        }
    }
}
