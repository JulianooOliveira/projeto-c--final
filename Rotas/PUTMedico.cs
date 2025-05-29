using System.Net;
using System.Text;
using System.IO;
using System;
using MySql.Data.MySqlClient;
using Database;

namespace Rotas
{
    public static class PUTMedico
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res)
        {
            using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            // Formato esperado: "Id;NomeMedico;CrmMedico;DataNascimentoMedico"
            var partes = body.Split(';');
            if (partes.Length != 4 ||
                !int.TryParse(partes[0], out int id) ||
                !int.TryParse(partes[2], out int crm) ||
                !DateTime.TryParse(partes[3], out DateTime dataNasc))
            {
                res.StatusCode = 400;
                EscreverResposta(res, "Dados inválidos. Formato esperado: Id;NomeMedico;CrmMedico;DataNascimentoMedico");
                return;
            }

            string nome = partes[1];

            try
            {
                using (var conn = Database.GetConnection())
                {
                    string selectQuery = "SELECT COUNT(*) FROM medico WHERE id = @id";
                    using (var selectCmd = new MySqlCommand(selectQuery, conn))
                    {
                        selectCmd.Parameters.AddWithValue("@id", id);
                        var count = Convert.ToInt32(selectCmd.ExecuteScalar());

                        if (count == 0)
                        {
                            res.StatusCode = 404;
                            EscreverResposta(res, "Médico não encontrado.");
                            return;
                        }
                    }

                    string updateQuery = @"UPDATE medico
                                           SET nomeMedico = @nome, crmMedico = @crm, dataNascimentoMedico = @dataNasc
                                           WHERE id = @id";
                    using (var updateCmd = new MySqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@nome", nome);
                        updateCmd.Parameters.AddWithValue("@crm", crm);
                        updateCmd.Parameters.AddWithValue("@dataNasc", dataNasc);
                        updateCmd.Parameters.AddWithValue("@id", id);

                        int linhasAfetadas = updateCmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                            EscreverResposta(res, "Médico atualizado com sucesso.");
                        else
                            EscreverResposta(res, "Falha ao atualizar médico.");
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
