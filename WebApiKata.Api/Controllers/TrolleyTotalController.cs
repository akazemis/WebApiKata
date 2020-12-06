using AutoMapper;
using WebApiKata.Interfaces;
using WebApiKata.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApiKata.Api.Exceptions;
using WebApiKata.Api.ResourceModels;

namespace WebApiKata.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrolleyTotalController : ControllerBase
    {
        private readonly ITrolleyCalculator _TrolleyCalculator;
        private readonly ILogger<TrolleyTotalController> _logger;
        private readonly IMapper _mapper;

        public TrolleyTotalController(ILogger<TrolleyTotalController> logger,
                                      IMapper mapper,
                                      ITrolleyCalculator TrolleyCalculator)
        {
            _logger = logger;
            _mapper = mapper;
            _TrolleyCalculator = TrolleyCalculator;
        }

        [HttpPost]
        public async Task<decimal> CalculateTrolleyTotal([FromBody] TrolleyInfoModel trolleyInfoModel)
        {
            if(trolleyInfoModel == null)
            {
                throw new BadApiRequestException($"{nameof(trolleyInfoModel)} is required.");
            }
            var trolleyInfo = _mapper.Map<TrolleyInfoModel, TrolleyInfo>(trolleyInfoModel);
            var total = await _TrolleyCalculator.CalculateTrolley(trolleyInfo);
            return total;
        }
    }
}
