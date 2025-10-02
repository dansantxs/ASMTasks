using API.Models;
using Dapper;

namespace API.DAOs
{
    public class SetoresDAO
    {
        public async Task<IEnumerable<Setor>> ObterTodosAsync(DbContext dbContext)
        {
            var sql = "SELECT Id, Nome, Descricao, CriadoEm, Ativo, ResponsavelId FROM Setor";
            using var con = await dbContext.GetConnectionAsync();
            return await con.QueryAsync<Setor>(sql);
        }

        public async Task<Setor?> ObterPorIdAsync(DbContext dbContext, int id)
        {
            var sql = "SELECT Id, Nome, Descricao, CriadoEm, Ativo, ResponsavelId FROM Setor WHERE Id = @Id";
            using var con = await dbContext.GetConnectionAsync();
            return await con.QueryFirstOrDefaultAsync<Setor>(sql, new { Id = id });
        }

        public async Task<int> CriarAsync(DbContext dbContext, Setor setor)
        {
            var sql = @"INSERT INTO Setor (Nome, Descricao, CriadoEm, Ativo, ResponsavelId)
                        VALUES (@Nome, @Descricao, @CriadoEm, @Ativo, @ResponsavelId);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            using var con = await dbContext.GetConnectionAsync();
            return await con.ExecuteScalarAsync<int>(sql, setor);
        }

        public async Task<bool> AtualizarAsync(DbContext dbContext, Setor setor)
        {
            var sql = @"UPDATE Setor SET Nome = @Nome, Descricao = @Descricao, ResponsavelId = @ResponsavelId
                        WHERE Id = @Id AND Ativo = 1";
            using var con = await dbContext.GetConnectionAsync();
            return await con.ExecuteAsync(sql, setor) > 0;
        }

        public async Task<bool> InativarAsync(DbContext dbContext, int id)
        {
            var sql = "UPDATE Setor SET Ativo = 0 WHERE Id = @Id";
            using var con = await dbContext.GetConnectionAsync();
            return await con.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<bool> ReativarAsync(DbContext dbContext, int id)
        {
            var sql = "UPDATE Setor SET Ativo = 1 WHERE Id = @Id";
            using var con = await dbContext.GetConnectionAsync();
            return await con.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<bool> VerificarExistenciaPorNomeAsync(DbContext dbContext, string nome, int? id = null)
        {
            var sql = @"SELECT COUNT(1) FROM Setor 
                        WHERE Nome = @Nome AND (Id <> @Id OR @Id IS NULL)";
            using var con = await dbContext.GetConnectionAsync();
            var count = await con.ExecuteScalarAsync<int>(sql, new { Nome = nome, Id = id });
            return count > 0;
        }

        //Verificar se existem tarefas em andamento naquele setor
    }
}