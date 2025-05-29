using System.Net;
using System.Text;
using System.IO;
using System;
using MySql.Data.MySqlClient;
using Database;

namespace Rotas
{
    public static class SETMedico
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
                    string verificaQuery = "SELECT COUNT(*) FROM medico WHERE crmMedico = @crm";
                    using (var verificaCmd = new MySqlCommand(verificaQuery, conn))
                    {
                        verificaCmd.Parameters.AddWithValue("@crm", crm);
                        int count = Convert.ToInt32(verificaCmd.ExecuteScalar());

                        if (count > 0)
                        {
                            res.StatusCode = 409;
                            EscreverResposta(res, "Médico com esse CRM já cadastrado.");
                            return;
                        }
                    }

                    string insertQuery = @"INSERT INTO medico (id, nomeMedico, crmMedico, dataNascimentoMedico)
                                           VALUES (@id, @nome, @crm, @dataNasc)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@crm", crm);
                        cmd.Parameters.AddWithValue("@dataNasc", dataNasc);

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                            EscreverResposta(res, "Médico cadastrado com sucesso.");
                        else
                            EscreverResposta(res, "Falha ao cadastrar médico.");
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
