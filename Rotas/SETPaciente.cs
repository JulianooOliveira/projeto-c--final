using System.Net;
using System.Text;
using System.IO;
using models;

namespace Rotas
{
    public static class SetPaciente
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res, List<Paciente> pacientes)
        {
            using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            // Esperando dados simples: "Id;Nome;DataNascimento;CPF"
            // Exemplo: "1;Juliano;1990-05-25;12345678900"
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

            var paciente = new Paciente
            {
                Id = id,
                Nome = partes[1],
                DataNascimento = dataNasc,
                CPF = partes[3]
            };

            pacientes.Add(paciente);
            EscreverResposta(res, "Paciente cadastrado.");
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
