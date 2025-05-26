using System;
using System.Collections.Generic;
using models;
using MySql.Data.MySqlClient;

namespace Database
{
    public static class PacienteDB
    {
        private static string connectionString = "Server=localhost;Database=Clinica;Uid=root;Pwd=;";

        public static void InserirPaciente(Paciente paciente)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "INSERT INTO Pacientes (Id, Nome, DataNascimento, CPF) VALUES (@id, @nome, @data, @cpf)";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", paciente.Id);
            cmd.Parameters.AddWithValue("@nome", paciente.Nome);
            cmd.Parameters.AddWithValue("@data", paciente.DataNascimento);
            cmd.Parameters.AddWithValue("@cpf", paciente.CPF);

            cmd.ExecuteNonQuery();
        }

        public static List<Paciente> ListarPacientes()
        {
            var pacientes = new List<Paciente>();

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "SELECT Id, Nome, DataNascimento, CPF FROM Pacientes";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                pacientes.Add(new Paciente
                {
                    Id = reader.GetInt32(0),
                    Nome = reader.GetString(1),
                    DataNascimento = reader.GetDateTime(2),
                    CPF = reader.GetString(3)
                });
            }

            return pacientes;
        }

        public static bool AtualizarPaciente(Paciente paciente)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "UPDATE Pacientes SET Nome=@nome, DataNascimento=@data WHERE CPF=@cpf";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@nome", paciente.Nome);
            cmd.Parameters.AddWithValue("@data", paciente.DataNascimento);
            cmd.Parameters.AddWithValue("@cpf", paciente.CPF);

            int linhasAfetadas = cmd.ExecuteNonQuery();
            return linhasAfetadas > 0;
        }

        public static bool DeletarPaciente(string cpf)
        {
            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = "DELETE FROM Pacientes WHERE CPF=@cpf";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@cpf", cpf);

            int linhasAfetadas = cmd.ExecuteNonQuery();
            return linhasAfetadas > 0;
        }
    }
}
