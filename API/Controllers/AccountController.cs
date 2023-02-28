using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities.Domain;
using API.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;
        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;

        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(string username, string password)
        {
            var user = await _accountRepository.Register(username, password);

            return user;
        }
    }
}