using System.Net;
using System.Text;
using System.IO;
using System;
using MySql.Data.MySqlClient;

namespace Rotas
{
    public static class SetPaciente
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res)
        {
            using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            // Esperando dados simples: "Id;Nome;DataNascimento;CPF"
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

                    // Verifica se já existe paciente com o mesmo CPF para evitar duplicidade
                    string verificaQuery = "SELECT COUNT(*) FROM pacientes WHERE cpf = @cpf";
                    using (MySqlCommand verificaCmd = new MySqlCommand(verificaQuery, conn))
                    {
                        verificaCmd.Parameters.AddWithValue("@cpf", cpf);
                        int count = Convert.ToInt32(verificaCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            res.StatusCode = 409; // conflito
                            EscreverResposta(res, "Paciente com esse CPF já cadastrado.");
                            return;
                        }
                    }

                    string insertQuery = @"INSERT INTO pacientes (id, nome, dataNascimento, cpf) 
                                           VALUES (@id, @nome, @dataNascimento, @cpf)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@dataNascimento", dataNasc);
                        cmd.Parameters.AddWithValue("@cpf", cpf);

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                        {
                            EscreverResposta(res, "Paciente cadastrado com sucesso.");
                        }
                        else
                        {
                            res.StatusCode = 500;
                            EscreverResposta(res, "Falha ao cadastrar paciente.");
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
