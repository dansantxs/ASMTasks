namespace API.Entities
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
    }
}