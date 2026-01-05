using BancoChu.Application.Dtos.Accounts;
using BancoChu.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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


        /// <summary>
        /// Cria uma nova conta bancária.
        /// </summary>
        /// <param name="request">
        /// Dados necessários para a criação da conta bancária.
        /// </param>
        /// <returns>
        /// Retorna os dados da conta criada.
        /// </returns>
        /// <response code="200">
        /// Conta criada com sucesso.
        /// </response>
        /// <response code="500">
        /// Erro interno ao criar a conta.
        /// </response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAsync(CreateAccountsRequestDto request)
        {
            try
            {
                var accountId = await _accountsApplication.CreateAsync(request);

                return Created(string.Empty, new
                {
                    accountId,
                    message = "Conta criada com sucesso"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
        }


        /// <summary>
        /// Realiza uma transferência bancária a partir de uma conta de origem.
        /// </summary>
        /// <param name="accountId">
        /// Identificador da conta de origem (GUID).
        /// </param>
        /// <param name="request">
        /// Dados da transferência, incluindo conta de destino e valor.
        /// </param>
        /// <returns>
        /// Retorna o identificador da transferência criada.
        /// </returns>
        /// <response code="201">
        /// Transferência realizada com sucesso.
        /// </response>
        /// <response code="400">
        /// Dados inválidos ou regra de negócio violada (ex: saldo insuficiente, contas iguais).
        /// </response>
        /// <response code="500">
        /// Erro interno inesperado.
        /// </response>
        [HttpPost("{accountId:guid}/transfer")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TransferAsync(Guid accountId, [FromBody] TransferRequestDto request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                var transferId = await _accountsApplication.TransferAsync(userId, accountId, request);

                return Created(string.Empty, new
                {
                    transferId,
                    message = "Transferência realizada com sucesso"
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    error = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { error = "Erro interno ao realizar a transferência" }
                );
            }
        }

        /// <summary>
        /// Obtém o extrato bancário de uma conta em um determinado período.
        /// </summary>
        /// <param name="accountId">
        /// Identificador da conta bancária.
        /// </param>
        /// <param name="startDate">
        /// Data inicial do período do extrato (inclusive).
        /// Formato esperado: <c>yyyy/MM/dd</c>
        /// </param>
        /// <param name="endDate">
        /// Data final do período do extrato (inclusive).
        /// Formato esperado: <c>yyyy/MM/dd</c>
        /// </param>
        /// <returns>
        /// Retorna a lista de movimentações (créditos e débitos) da conta no período informado.
        /// </returns>
        /// <response code="200">
        /// Extrato retornado com sucesso.
        /// </response>
        /// <response code="400">
        /// A data inicial é maior que a data final ou o formato da data é inválido.
        /// </response>
        /// <response code="404">
        /// Conta não encontrada.
        /// </response>
        /// <response code="500">
        /// Erro interno ao processar a solicitação.
        /// </response>
        [HttpGet("{accountId}/statement")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStatementAsync([FromRoute] Guid accountId,
                                                    [FromQuery] DateTime startDate,
                                                    [FromQuery] DateTime endDate)

        {

            var query = new GetStatementRequestDto
            {
                AccountId = accountId,
                StartDate = startDate,
                EndDate = endDate
            };

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                var statement = await _accountsApplication.GetStatementAsync(userId, accountId, startDate, endDate);
                return Ok(statement);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }

        }
    }
}
