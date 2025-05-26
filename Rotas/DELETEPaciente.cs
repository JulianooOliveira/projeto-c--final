using System.Net;
using System.Text;
using models;
using System.Linq;

namespace Rotas
{
    public static class DeletePaciente
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res, List<Paciente> pacientes)
        {
            var cpf = req.QueryString["cpf"];

            if (string.IsNullOrEmpty(cpf))
            {
                res.StatusCode = 400;
                EscreverResposta(res, "CPF não fornecido na query string.");
                return;
            }

            var paciente = pacientes.FirstOrDefault(p => p.CPF == cpf);

            if (paciente == null)
            {
                res.StatusCode = 404;
                EscreverResposta(res, "Paciente não encontrado.");
                return;
            }

            pacientes.Remove(paciente);
            EscreverResposta(res, "Paciente removido com sucesso.");
        }

        private static void EscreverResposta(HttpListenerResponse res, string mensagem)
        {
            byte[] msg = Encoding.UTF8.GetBytes(mensagem);
            res.ContentType = "text/plain";
            res.OutputStream.Write(msg, 0, msg.Length);
            res.Close();
        }
    }
}
