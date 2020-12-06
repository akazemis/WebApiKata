using AutoMapper;
using WebApiKata.Models;
using System.Threading.Tasks;
using WebApiKata.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiKata.Api.ResourceModels;
using System;
using System.Collections.Generic;
using WebApiKata.Api.Exceptions;

namespace WebApiKata.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SortController : ControllerBase
    {
        private readonly ILogger<SortController> _logger;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public SortController(ILogger<SortController> logger,
                              IMapper mapper,
                              IProductService productService)
        {
            _logger = logger;
            _mapper = mapper;
            _productService = productService;
        }

        /// <summary>
        /// Get User and its token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<ProductModel>> GetSortedShopperHistory([FromQuery] string sortOption)
        {
            if(sortOption == null)
            {
                throw new BadApiRequestException($"SortOption query parameter is required.");
            }
            SortType sortType;
            if (!Enum.TryParse<SortType>(sortOption, out sortType))
            {
                throw new BadApiRequestException($"SortOption '{sortOption}' is not supported.");
            }
            var sortedProducts = await _productService.GetSortedProducts(sortType);
            var sortedProductsModel = _mapper.Map<List<Product>, List<ProductModel>>(sortedProducts);
            return sortedProductsModel;
        }
    }
}
