using API.DAOs;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class Setor
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public bool Ativo { get; set; } = true;
        public int? ResponsavelId { get; set; }
        public Colaborador? Responsavel { get; set; }

        public async Task<int> CriarAsync(DbContext dbContext, SetoresDAO dao)
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new ValidationException("O nome do setor é obrigatório.");

            if (await dao.VerificarExistenciaPorNomeAsync(dbContext, Nome))
                throw new ValidationException("Já existe um setor com esse nome.");

            CriadoEm = DateTime.UtcNow;
            Ativo = true;

            return await dao.CriarAsync(dbContext, this);
        }

        public async Task AtualizarAsync(DbContext dbContext, SetoresDAO dao)
        {
            if (string.IsNullOrWhiteSpace(Nome))
                throw new ValidationException("O nome do setor é obrigatório.");

            if (await dao.VerificarExistenciaPorNomeAsync(dbContext, Nome, Id))
                throw new ValidationException("Já existe outro setor com esse nome.");

            var atualizado = await dao.AtualizarAsync(dbContext, this);
            if (!atualizado)
                throw new ValidationException("Setor não encontrado.");
        }

        public async Task InativarAsync(DbContext dbContext, SetoresDAO dao)
        {
            // verificar se existem tarefas em andamento

            var inativado = await dao.InativarAsync(dbContext, Id);
            if (!inativado)
                throw new ValidationException("Setor não encontrado.");
        }

        public async Task ReativarAsync(DbContext dbContext, SetoresDAO dao)
        {
            var reativado = await dao.ReativarAsync(dbContext, Id);
            if (!reativado)
                throw new ValidationException("Setor não encontrado.");
        }

        public static async Task<IEnumerable<Setor>> ObterTodosAsync(DbContext dbContext, SetoresDAO dao)
        {
            return await dao.ObterTodosAsync(dbContext);
        }

        public static async Task<Setor?> ObterPorIdAsync(DbContext dbContext, SetoresDAO dao, int id)
        {
            return await dao.ObterPorIdAsync(dbContext, id);
        }
    }
}