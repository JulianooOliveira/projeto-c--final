using System.Net;
using System.Text;
using System.IO;
using System;
using MySql.Data.MySqlClient;
using Database;

namespace Rotas
{
    public static class SETEspecialidade
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res)
        {
            using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            // Formato esperado: "Id;NomeEspecialidade;Descricao;IdMedico"
            var partes = body.Split(';');
            if (partes.Length != 4 ||
                !int.TryParse(partes[0], out int id) ||
                !int.TryParse(partes[3], out int idMedico))
            {
                res.StatusCode = 400;
                EscreverResposta(res, "Dados inválidos. Formato esperado: Id;NomeEspecialidade;Descricao;IdMedico");
                return;
            }

            string nome = partes[1];
            string descricao = partes[2];

            try
            {
                using (var conn = Database.GetConnection())
                {
                    // Verifica se o médico existe
                    string verificaMedicoQuery = "SELECT COUNT(*) FROM medico WHERE id = @idMedico";
                    using (var verificaCmd = new MySqlCommand(verificaMedicoQuery, conn))
                    {
                        verificaCmd.Parameters.AddWithValue("@idMedico", idMedico);
                        int count = Convert.ToInt32(verificaCmd.ExecuteScalar());
                        if (count == 0)
                        {
                            res.StatusCode = 404;
                            EscreverResposta(res, "Médico não encontrado.");
                            return;
                        }
                    }

                    // Verifica se a especialidade já existe
                    string verificaEspecialidadeQuery = "SELECT COUNT(*) FROM especialidade WHERE nomeEspecialidade = @nome";
                    using (var verificaCmd = new MySqlCommand(verificaEspecialidadeQuery, conn))
                    {
                        verificaCmd.Parameters.AddWithValue("@nome", nome);
                        int count = Convert.ToInt32(verificaCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            res.StatusCode = 409;
                            EscreverResposta(res, "Especialidade com esse nome já cadastrada.");
                            return;
                        }
                    }

                    // Insere nova especialidade
                    string insertQuery = @"INSERT INTO especialidade (id, nomeEspecialidade, descricao, idMedico)
                                           VALUES (@id, @nome, @descricao, @idMedico)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@descricao", descricao);
                        cmd.Parameters.AddWithValue("@idMedico", idMedico);

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                            EscreverResposta(res, "Especialidade cadastrada com sucesso.");
                        else
                            EscreverResposta(res, "Falha ao cadastrar especialidade.");
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
