using BancoChu.Application.Dtos.Accounts;
using BancoChu.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoChuApi.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsApplication _accountsApplication;
        public AccountsController(IAccountsApplication accountsApplication)
        {
            _accountsApplication = accountsApplication;
        }
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync(CreateAccountsRequestDto request)
        {
            var response = await _accountsApplication.CreateAsync(request);
            // _logger.LogInformation($"Iniciando a consulta referente a seguinte lista de places:{JsonSerializer.Serialize(request.Places)}");
            return Ok(response);
        }
    }
}
