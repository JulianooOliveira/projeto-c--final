using System.Net;
using System.Text;
using System.IO;
using System;
using MySql.Data.MySqlClient;

namespace Rotas
{
    public static class PutPaciente
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res, List<models.Paciente> pacientes)
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

            string nome = partes[1];
            string cpf = partes[3];

            string connectionString = "server=localhost;database=clinica;uid=root;pwd=;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Primeiro, verifica se paciente existe pelo CPF
                    string selectQuery = "SELECT COUNT(*) FROM pacientes WHERE cpf = @cpf";
                    using (MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@cpf", cpf);
                        var count = Convert.ToInt32(selectCmd.ExecuteScalar());
                        if (count == 0)
                        {
                            res.StatusCode = 404;
                            EscreverResposta(res, "Paciente não encontrado.");
                            return;
                        }
                    }

                    // Atualiza paciente pelo CPF
                    string updateQuery = @"
                        UPDATE pacientes
                        SET id = @id, nome = @nome, DataNascimento = @dataNasc
                        WHERE cpf = @cpf";

                    using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@id", id);
                        updateCmd.Parameters.AddWithValue("@nome", nome);
                        updateCmd.Parameters.AddWithValue("@dataNasc", dataNasc);
                        updateCmd.Parameters.AddWithValue("@cpf", cpf);

                        int linhasAfetadas = updateCmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                        {
                            EscreverResposta(res, "Paciente atualizado com sucesso.");
                        }
                        else
                        {
                            res.StatusCode = 500;
                            EscreverResposta(res, "Falha ao atualizar paciente.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res.StatusCode = 500;
                EscreverResposta(res, "Erro no servidor: " + ex.Message);
            }
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
