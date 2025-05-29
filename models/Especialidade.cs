using System;

namespace models
{
    public class Especialidade
    {
        public int Id { get; set; }
        public string NomeEspecialidade { get; set; }
        public string Descricao { get; set; }
        public int IdMedico { get; set; }
    }
}
