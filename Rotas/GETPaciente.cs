using System.Net;
using System.Text;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using models;

namespace Rotas
{
    public static class GetPaciente
    {
        public static void Executar(HttpListenerResponse res)
        {
            StringBuilder sb = new StringBuilder();

            string connectionString = "server=localhost;database=clinica;uid=root;pwd=;";

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT id, nome, cpf, DataNascimento FROM pacientes";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            sb.AppendLine("Nenhum paciente cadastrado.");
                        }
                        else
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32("id");
                                string nome = reader.GetString("nome");
                                string cpf = reader.GetString("cpf");
                                DateTime nascimento = reader.GetDateTime("DataNascimento");

                                sb.AppendLine($"Id: {id}; Nome: {nome}; DataNascimento: {nascimento:yyyy-MM-dd}; CPF: {cpf}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.AppendLine("Erro ao consultar pacientes: " + ex.Message);
            }

            byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
            res.ContentType = "text/plain";
            res.OutputStream.Write(buffer, 0, buffer.Length);
            res.Close();
        }
    }
}
