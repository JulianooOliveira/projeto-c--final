using System.Net;
using System.Text;
using System;
using MySql.Data.MySqlClient;
using Database;

namespace Rotas
{
    public static class GETMedico
    {
        public static void Executar(HttpListenerResponse res)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                using (var conn = Database.GetConnection())
                {
                    string query = "SELECT id, nomeMedico, crmMedico, dataNascimentoMedico FROM medico";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            sb.AppendLine("Nenhum médico cadastrado.");
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                string nome = reader.GetString("nomeMedico");
                                int crm = reader.GetInt32("crmMedico");
                                DateTime nascimento = reader.GetDateTime("dataNascimentoMedico");

                                sb.AppendLine($"Id: {id}; Nome: {nome}; CRM: {crm}; DataNascimento: {nascimento:yyyy-MM-dd}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.AppendLine("Erro ao consultar médicos: " + ex.Message);
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            res.ContentType = "text/plain";
            res.OutputStream.Write(buffer, 0, buffer.Length);
            res.Close();
        }
    }
}
