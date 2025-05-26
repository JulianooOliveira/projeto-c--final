using System.Net;
using System.Text;
using models;

namespace Rotas
{
    public static class GetPaciente
    {
        public static void Executar(HttpListenerResponse res, List<Paciente> pacientes)
        {
            StringBuilder sb = new StringBuilder();

            if (pacientes.Count == 0)
            {
                sb.AppendLine("Nenhum paciente cadastrado.");
            }
            else
            {
                foreach (var p in pacientes)
                {
                    sb.AppendLine($"Id: {p.Id}; Nome: {p.Nome}; DataNascimento: {p.DataNascimento:yyyy-MM-dd}; CPF: {p.CPF}");
                }
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            res.ContentType = "text/plain";
            res.OutputStream.Write(buffer, 0, buffer.Length);
            res.Close();
        }
    }
}
