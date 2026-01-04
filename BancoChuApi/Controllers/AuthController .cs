using BancoChu.Application.Dtos.Auth;
using BancoChu.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BancoChuApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthApplication _authApplication;

        public AuthController(IAuthApplication authApplication)
        {
            _authApplication = authApplication;
        }

        /// <summary>
        /// Realiza a autenticação do usuário.
        /// </summary>
        /// <remarks>
        /// Endpoint responsável por validar as credenciais do usuário
        /// e retornar um token JWT em caso de sucesso.
        /// </remarks>
        /// <param name="request">Dados de login (email e senha).</param>
        /// <response code="200">Usuário autenticado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="401">Credenciais inválidas.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto request)
        {
            try
            {
                var result = await _authApplication.LoginAsync(request);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ProblemDetails
                {
                    Title = "Credenciais inválidas",
                    Detail = ex.Message,
                    Status = StatusCodes.Status401Unauthorized
                });
            }
        }

    }
}

