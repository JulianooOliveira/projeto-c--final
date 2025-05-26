using System;
using System.Net;
using System.Text;
using MySql.Data.MySqlClient;

namespace Rotas
{
    public static class DeletePaciente
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res)
        {
            // Extrai o parâmetro "id" da query string
            var query = req.Url.Query; // Exemplo: "?id=1"
            var queryParams = System.Web.HttpUtility.ParseQueryString(query);
            string idParam = queryParams["id"];

            // Valida o ID
            if (!int.TryParse(idParam, out int id))
            {
                res.StatusCode = 400;
                EscreverResposta(res, "ID inválido. Informe um número inteiro válido.");
                return;
            }

            string connectionString = "server=localhost;database=clinica;uid=root;pwd=;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Verifica se paciente existe no banco
                    string checkQuery = "SELECT COUNT(*) FROM pacientes WHERE id = @id";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            res.StatusCode = 404;
                            EscreverResposta(res, "Paciente não encontrado.");
                            return;
                        }
                    }

                    // Executa a exclusão do paciente
                    string deleteQuery = "DELETE FROM pacientes WHERE id = @id";
                    using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@id", id);
                        int linhasAfetadas = deleteCmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                        {
                            EscreverResposta(res, "Paciente deletado com sucesso.");
                        }
                        else
                        {
                            res.StatusCode = 500;
                            EscreverResposta(res, "Erro ao deletar paciente.");
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
