using System.Net;
using System.Text;
using System;
using MySql.Data.MySqlClient;
using Database;

namespace Rotas
{
    public static class DELETEMedico
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res)
        {
            var query = req.Url.Query;
            var queryParams = System.Web.HttpUtility.ParseQueryString(query);
            string idParam = queryParams["id"];

            if (!int.TryParse(idParam, out int id))
            {
                res.StatusCode = 400;
                EscreverResposta(res, "ID inválido. Informe um número inteiro válido.");
                return;
            }

            try
            {
                using (var conn = Database.GetConnection())
                {
                    string checkQuery = "SELECT COUNT(*) FROM medico WHERE id = @id";
                    using (var checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            res.StatusCode = 404;
                            EscreverResposta(res, "Médico não encontrado.");
                            return;
                        }
                    }

                    string deleteQuery = "DELETE FROM medico WHERE id = @id";
                    using (var deleteCmd = new MySqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@id", id);
                        int linhasAfetadas = deleteCmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                            EscreverResposta(res, "Médico deletado com sucesso.");
                        else
                            EscreverResposta(res, "Erro ao deletar médico.");
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
