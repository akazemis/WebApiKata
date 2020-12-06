﻿using AutoMapper;
using WebApiKata.Models;
using System.Threading.Tasks;
using WebApiKata.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiKata.Api.ResourceModels;

namespace WebApiKata.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UserController(ILogger<UserController> logger,
                              IMapper mapper,
                              IUserRepository userRepository)
        {
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get User and its token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<UserModel> Get()
        {
            var user = await _userRepository.GetUser();
            var userModel = _mapper.Map<User, UserModel>(user);
            return userModel;
        }
    }
}