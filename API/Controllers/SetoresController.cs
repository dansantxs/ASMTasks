using API.DTOs.Setores;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class SetoresController : ControllerBase
    {
        private readonly SetoresService _setoresService;

        public SetoresController(SetoresService setoresService)
        {
            _setoresService = setoresService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var setores = await _setoresService.ObterTodosAsync();
            return Ok(setores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var setor = await _setoresService.ObterPorIdAsync(id);
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

                var id = await _setoresService.CriarAsync(setor);
                return CreatedAtAction(nameof(ObterPorId), new { id }, id);
            }
            catch (ArgumentException ex)
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

                await _setoresService.AtualizarAsync(setor);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Inativar(int id)
        {
            try
            {
                await _setoresService.InativarAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}