using API.Entities;
using API.Repositories;

namespace API.Services
{
    public class SetoresService
    {
        private readonly SetoresRepository _setoresRepository;

        public SetoresService(SetoresRepository setoresRepository)
        {
            _setoresRepository = setoresRepository;
        }

        public async Task<IEnumerable<Setor>> ObterTodosAsync()
        {
            return await _setoresRepository.ObterTodosAsync();
        }

        public async Task<Setor?> ObterPorIdAsync(int id)
        {
            return await _setoresRepository.ObterPorIdAsync(id);
        }

        public async Task<int> CriarAsync(Setor setor)
        {
            if (string.IsNullOrWhiteSpace(setor.Nome))
                throw new ArgumentException("O nome do setor é obrigatório.");

            if (await _setoresRepository.VerificarExistenciaPorNomeAsync(setor.Nome))
                throw new ArgumentException("Já existe um setor com esse nome.");

            setor.CriadoEm = DateTime.UtcNow;
            setor.Ativo = true;

            return await _setoresRepository.CriarAsync(setor);
        }

        public async Task AtualizarAsync(Setor setor)
        {
            if (string.IsNullOrWhiteSpace(setor.Nome))
                throw new ArgumentException("O nome do setor é obrigatório.");

            if (await _setoresRepository.VerificarExistenciaPorNomeAsync(setor.Nome, setor.Id))
                throw new ArgumentException("Já existe outro setor com esse nome.");

            var atualizado = await _setoresRepository.AtualizarAsync(setor);
            if (!atualizado)
                throw new ArgumentException("Setor não encontrado.");
        }

        public async Task InativarAsync(int id)
        {
            //Verificar se existem tarefas em andamento naquele setor

            var inativado = await _setoresRepository.InativarAsync(id);
            if (!inativado)
                throw new ArgumentException("Setor não encontrado.");
        }
    }
}