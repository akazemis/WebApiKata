using AutoMapper;
using DataAccess.Interfaces;
using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WebApiKata.Exceptions;
using WebApiKata.ResourceModels;

namespace WebApiKata.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrolleyTotalController : ControllerBase
    {
        private readonly ITrolleyRepository _trolleyRepository;
        private readonly ILogger<TrolleyTotalController> _logger;
        private readonly IMapper _mapper;

        public TrolleyTotalController(ILogger<TrolleyTotalController> logger,
                                      IMapper mapper,
                                      ITrolleyRepository trolleyRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _trolleyRepository = trolleyRepository;
        }

        [HttpPost]
        public async Task<double> CalculateTrolleyTotal([FromBody] TrolleyInfoModel trolleyInfoModel)
        {
            if(trolleyInfoModel == null)
            {
                throw new BadApiRequestException($"{nameof(trolleyInfoModel)} is required.");
            }
            var trolleyInfo = _mapper.Map<TrolleyInfoModel, TrolleyInfo>(trolleyInfoModel);
            var total = await _trolleyRepository.CalculateTrolley(trolleyInfo);
            return total;
        }
    }
}
