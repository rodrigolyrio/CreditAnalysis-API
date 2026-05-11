using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CreditoApi.Services; 

namespace CreditoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditoController : ControllerBase
    {
        private readonly string _connectionString;
        
        public CreditoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>
        /// Recebe os dados do cliente e realiza a análise de crédito automática.
        /// </summary>
        /// <param name="proposta">Objeto contendo Nome, CPF, Renda e Valor solicitado.</param>
        /// <returns>Retorna se o crédito foi Aprovado ou Reprovado.</returns>
        [HttpPost]
        public async Task<IActionResult> CriarProposta(Proposta proposta)
        {
            if (string.IsNullOrEmpty(proposta.NomeCliente) || string.IsNullOrEmpty(proposta.CPF))
                return BadRequest("Nome e CPF são obrigatórios! ");

            try 
            {
                using var connection = new SqlConnection(_connectionString);
                
                var parametros = new { 
                    proposta.NomeCliente, 
                    proposta.CPF, 
                    proposta.ValorSolicitado, 
                    proposta.RendaMensal 
                };

                var resultado = await connection.QueryFirstOrDefaultAsync<string>(
                    "sp_CriarProposta", 
                    parametros, 
                    commandType: System.Data.CommandType.StoredProcedure
                );

                return Ok($"Análise via Procedure concluída! Resultado: {resultado} ");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro no banco: {ex.Message}");
            }
        }


        /// <summary>
        /// Recupera o histórico completo de todas as análises de crédito realizadas.
        /// </summary>
        /// <remarks>
        /// Ideal para gerar relatórios ou visualizar todas as tentativas de empréstimo registradas no banco.
        /// </remarks>
        /// <returns>Uma lista contendo todas as propostas ordenadas pela mais recente.</returns>
        [HttpGet]
        public async Task<IActionResult> ListarPropostas()
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var sql = "SELECT * FROM Propostas ORDER BY Id DESC";
                var lista = await connection.QueryAsync<Proposta>(sql);
                
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar dados: {ex.Message}");
            }
        }

        /// <summary>
        /// Realiza uma busca detalhada de uma única proposta através do seu identificador único (ID).
        /// </summary>
        /// <param name="id">O número de identificação da proposta no banco de dados.</param>
        /// <returns>Os dados detalhados da proposta ou um erro 404 caso não seja encontrada.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var sql = "SELECT * FROM Propostas WHERE Id = @id";
                var proposta = await connection.QueryFirstOrDefaultAsync<Proposta>(sql, new { id });

                if (proposta == null)
                {
                    return NotFound($"Proposta com ID {id} não encontrada! ");
                }

                return Ok(proposta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar proposta: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza dados cadastrais (Nome e CPF) de uma proposta existente.
        /// </summary>
        /// <param name="id">ID da proposta a ser editada.</param>
        /// <param name="proposta">Novos dados de Nome e CPF.</param>
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarCadastro(int id, [FromBody] Proposta proposta)
        {
            if (string.IsNullOrEmpty(proposta.NomeCliente) || string.IsNullOrEmpty(proposta.CPF))
                return BadRequest("Dados inválidos para atualização! ");

            try 
            {
                using var connection = new SqlConnection(_connectionString);
                
                var linhas = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_AtualizarCadastroProposta", 
                    new { Id = id, NovoNome = proposta.NomeCliente, NovoCPF = proposta.CPF }, 
                    commandType: System.Data.CommandType.StoredProcedure
                );

                if (linhas == 0) return NotFound("Proposta não encontrada para editar. ");

                return Ok("Cadastro atualizado com sucesso! ");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar: {ex.Message}");
            }
        }


        /// <summary>
        /// Remove permanentemente um registro de proposta do sistema.
        /// </summary>
        /// <param name="id">O ID da proposta que deseja excluir.</param>
        /// <remarks>
        /// Cuidado! Esta ação não pode ser desfeita. Use para limpar registros de teste ou duplicados.
        /// </remarks>
        /// <returns>Uma mensagem confirmando o sucesso da exclusão ou erro caso o ID não exista.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarProposta(int id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                var sql = "DELETE FROM Propostas WHERE Id = @id";
                
                var linhasAfetadas = await connection.ExecuteAsync(sql, new { id });

                if (linhasAfetadas == 0)
                {
                    return NotFound($"Ops! Não achei nenhuma proposta com o ID {id} para deletar. ");
                }

                return Ok($"Proposta {id} removida com sucesso! ");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao deletar: {ex.Message}");
            }
        }
    }
}