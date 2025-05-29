using System;
using MySql.Data.MySqlClient;
using Database;

namespace Services
{
    public static class PacienteService
    {
        public static void CadastrarPaciente()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRAR PACIENTE ===");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Nome: ");
            string nome = Console.ReadLine();

            Console.Write("CPF: ");
            string cpf = Console.ReadLine();

            Console.Write("Data de Nascimento (dd/MM/yyyy): ");
            DateTime dataNascimento = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);

            Console.Write("ID da Especialidade: ");
            int idEspecialidade = int.Parse(Console.ReadLine());

            using (var conn = Database.GetConnection())
            {
                string verificaQuery = "SELECT COUNT(*) FROM especialidade WHERE id = @id";
                using (var cmd = new MySqlCommand(verificaQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idEspecialidade);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        Console.WriteLine("Especialidade não encontrada.");
                        Console.ReadKey();
                        return;
                    }
                }

                string query = @"INSERT INTO pacientes (id, nome, cpf, dataNascimento, idEspecialidade) 
                                 VALUES (@id, @nome, @cpf, @data, @idEspecialidade)";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@cpf", cpf);
                    cmd.Parameters.AddWithValue("@data", dataNascimento);
                    cmd.Parameters.AddWithValue("@idEspecialidade", idEspecialidade);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Paciente cadastrado com sucesso!");
            }

            Console.ReadKey();
        }

        public static void ListarPacientes()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA DE PACIENTES ===");

            using (var conn = Database.GetConnection())
            {
                string query = "SELECT id, nome, cpf, dataNascimento, idEspecialidade FROM pacientes";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Nenhum paciente encontrado.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string nome = reader.GetString("nome");
                            string cpf = reader.GetString("cpf");
                            DateTime nascimento = reader.GetDateTime("dataNascimento");
                            int idEsp = reader.GetInt32("idEspecialidade");

                            Console.WriteLine($"ID: {id} | Nome: {nome} | CPF: {cpf} | Nasc: {nascimento:dd/MM/yyyy} | Especialidade ID: {idEsp}");
                        }
                    }
                }
            }

            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        public static void AtualizarPaciente()
        {
            Console.Clear();
            Console.WriteLine("=== ATUALIZAR PACIENTE ===");
            Console.Write("Informe o CPF do paciente: ");
            string cpf = Console.ReadLine();

            using (var conn = Database.GetConnection())
            {
                string selectQuery = "SELECT * FROM pacientes WHERE cpf = @cpf";
                using (var selectCmd = new MySqlCommand(selectQuery, conn))
                {
                    selectCmd.Parameters.AddWithValue("@cpf", cpf);
                    using (var reader = selectCmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Paciente não encontrado.");
                            Console.ReadKey();
                            return;
                        }
                    }
                }

                Console.Write("Novo nome: ");
                string novoNome = Console.ReadLine();

                Console.Write("Nova data de nascimento (dd/MM/yyyy): ");
                DateTime novaData = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);

                string updateQuery = "UPDATE pacientes SET nome = @nome, dataNascimento = @data WHERE cpf = @cpf";
                using (var updateCmd = new MySqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@nome", novoNome);
                    updateCmd.Parameters.AddWithValue("@data", novaData);
                    updateCmd.Parameters.AddWithValue("@cpf", cpf);
                    updateCmd.ExecuteNonQuery();
                }

                Console.WriteLine("Paciente atualizado com sucesso!");
            }

            Console.ReadKey();
        }

        public static void DeletarPaciente()
        {
            Console.Clear();
            Console.WriteLine("=== DELETAR PACIENTE ===");
            Console.Write("Informe o CPF do paciente: ");
            string cpf = Console.ReadLine();

            using (var conn = Database.GetConnection())
            {
                string deleteQuery = "DELETE FROM pacientes WHERE cpf = @cpf";
                using (var deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@cpf", cpf);
                    int linhasAfetadas = deleteCmd.ExecuteNonQuery();

                    if (linhasAfetadas > 0)
                        Console.WriteLine("Paciente deletado com sucesso!");
                    else
                        Console.WriteLine("Paciente não encontrado.");
                }
            }

            Console.ReadKey();
        }
    }
}
