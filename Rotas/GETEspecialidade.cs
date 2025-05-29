using System.Net;
using System.Text;
using System;
using MySql.Data.MySqlClient;
using Database;

namespace Rotas
{
    public static class GETEspecialidade
    {
        public static void Executar(HttpListenerResponse res)
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                using (var conn = Database.GetConnection())
                {
                    string query = "SELECT id, nomeEspecialidade, descricao FROM especialidade";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            sb.AppendLine("Nenhuma especialidade cadastrada.");
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                string nome = reader.GetString("nomeEspecialidade");
                                string descricao = reader.GetString("descricao");

                                sb.AppendLine($"Id: {id}; Nome: {nome}; Descrição: {descricao}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.AppendLine("Erro ao consultar especialidades: " + ex.Message);
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            res.ContentType = "text/plain";
            res.OutputStream.Write(buffer, 0, buffer.Length);
            res.Close();
        }
    }
}
