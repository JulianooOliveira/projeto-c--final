using System.Net;
using System.Text;
using System.IO;
using models;
using System.Linq;

namespace Rotas
{
    public static class PutPaciente
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res, List<Paciente> pacientes)
        {
            using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            // Espera formato: "Id;Nome;DataNascimento;CPF" para atualizar pelo CPF
            var partes = body.Split(';');
            if (partes.Length != 4)
            {
                res.StatusCode = 400;
                EscreverResposta(res, "Dados inválidos. Formato esperado: Id;Nome;DataNascimento;CPF");
                return;
            }

            if (!int.TryParse(partes[0], out int id) ||
                !DateTime.TryParse(partes[2], out DateTime dataNasc))
            {
                res.StatusCode = 400;
                EscreverResposta(res, "Dados inválidos. Id deve ser número, DataNascimento deve ser data válida.");
                return;
            }

            var cpf = partes[3];
            var paciente = pacientes.FirstOrDefault(p => p.CPF == cpf);

            if (paciente == null)
            {
                res.StatusCode = 404;
                EscreverResposta(res, "Paciente não encontrado.");
                return;
            }

            paciente.Id = id;
            paciente.Nome = partes[1];
            paciente.DataNascimento = dataNasc;

            EscreverResposta(res, "Paciente atualizado com sucesso.");
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
