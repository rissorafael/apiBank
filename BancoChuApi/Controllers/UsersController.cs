using BancoChu.Application.Dtos.Users;
using BancoChu.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoChuApi.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUsersApplication _usersApplication;

        public UsersController(IUsersApplication usersApplication)
        {
            _usersApplication = usersApplication;
        }

        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        /// <param name="request">
        /// Dados necessários para criar um novo usuário, incluindo nome, email, senha e outros campos obrigatórios.
        /// </param>
        /// <returns>
        /// Retorna os dados do usuário criado.
        /// </returns>
        /// <response code="200">
        /// Usuário criado com sucesso.
        /// </response>
        /// <response code="500">
        /// Erro interno ao criar o usuário.
        /// </response>
        [HttpPost]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequestDto request)
        {
            var user = await _usersApplication.CreateAsync(request);
            if (user is null)
                return Conflict(new { message = "Email já cadastrado." });

            return Ok(user);
        }
    }
}