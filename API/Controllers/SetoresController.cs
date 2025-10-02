using API.DAOs;
using API.DTOs.Setores;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SetoresController : ControllerBase
    {
        private readonly DbContext _dbContext;
        private readonly SetoresDAO _setoresDAO;

        public SetoresController(DbContext dbContext, SetoresDAO setoresDAO)
        {
            _dbContext = dbContext;
            _setoresDAO = setoresDAO;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var setores = await Setor.ObterTodosAsync(_dbContext, _setoresDAO);
            return Ok(setores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var setor = await Setor.ObterPorIdAsync(_dbContext, _setoresDAO, id);
            if (setor == null)
                return NotFound("Setor não encontrado.");

            return Ok(setor);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] SetorCriarRequest request)
        {
            try
            {
                var setor = new Setor
                {
                    Nome = request.Nome,
                    Descricao = request.Descricao,
                    ResponsavelId = request.ResponsavelId
                };

                var id = await setor.CriarAsync(_dbContext, _setoresDAO);
                return CreatedAtAction(nameof(ObterPorId), new { id }, id);
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] SetorAtualizarRequest request)
        {
            try
            {
                var setor = new Setor
                {
                    Id = id,
                    Nome = request.Nome,
                    Descricao = request.Descricao,
                    ResponsavelId = request.ResponsavelId
                };

                await setor.AtualizarAsync(_dbContext, _setoresDAO);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Inativar(int id)
        {
            try
            {
                var setor = new Setor { Id = id };
                await setor.InativarAsync(_dbContext, _setoresDAO);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}/reativar")]
        public async Task<IActionResult> Reativar(int id)
        {
            try
            {
                var setor = new Setor { Id = id };
                await setor.ReativarAsync(_dbContext, _setoresDAO);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
