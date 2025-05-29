using System;
using MySql.Data.MySqlClient;
using Database;

namespace Services
{
    public static class EspecialidadeService
    {
        public static void CadastrarEspecialidade()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRAR ESPECIALIDADE ===");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Nome da Especialidade: ");
            string nome = Console.ReadLine();

            Console.Write("Descrição: ");
            string descricao = Console.ReadLine();

            Console.Write("ID do Médico responsável: ");
            int idMedico = int.Parse(Console.ReadLine());

            using (var conn = Database.GetConnection())
            {
                // Verifica se médico existe
                string verificaMedico = "SELECT COUNT(*) FROM medico WHERE id = @idMedico";
                using (var cmd = new MySqlCommand(verificaMedico, conn))
                {
                    cmd.Parameters.AddWithValue("@idMedico", idMedico);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        Console.WriteLine("Médico não encontrado.");
                        Console.ReadKey();
                        return;
                    }
                }

                // Verifica se já existe nome igual
                string verificaNome = "SELECT COUNT(*) FROM especialidade WHERE nomeEspecialidade = @nome";
                using (var cmd = new MySqlCommand(verificaNome, conn))
                {
                    cmd.Parameters.AddWithValue("@nome", nome);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        Console.WriteLine("Especialidade já cadastrada.");
                        Console.ReadKey();
                        return;
                    }
                }

                string insert = "INSERT INTO especialidade (id, nomeEspecialidade, descricao, idMedico) VALUES (@id, @nome, @desc, @idMedico)";
                using (var cmd = new MySqlCommand(insert, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@desc", descricao);
                    cmd.Parameters.AddWithValue("@idMedico", idMedico);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Especialidade cadastrada com sucesso!");
            }

            Console.ReadKey();
        }

        public static void ListarEspecialidades()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA DE ESPECIALIDADES ===");

            using (var conn = Database.GetConnection())
            {
                string query = "SELECT id, nomeEspecialidade, descricao, idMedico FROM especialidade";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Nenhuma especialidade encontrada.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string nome = reader.GetString("nomeEspecialidade");
                            string descricao = reader.GetString("descricao");
                            int idMedico = reader.GetInt32("idMedico");

                            Console.WriteLine($"ID: {id} | Nome: {nome} | Médico ID: {idMedico} | Descrição: {descricao}");
                        }
                    }
                }
            }

            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        public static void AtualizarEspecialidade()
        {
            Console.Clear();
            Console.WriteLine("=== ATUALIZAR ESPECIALIDADE ===");
            Console.Write("Informe o ID da especialidade: ");
            int id = int.Parse(Console.ReadLine());

            using (var conn = Database.GetConnection())
            {
                string select = "SELECT COUNT(*) FROM especialidade WHERE id = @id";
                using (var cmd = new MySqlCommand(select, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        Console.WriteLine("Especialidade não encontrada.");
                        Console.ReadKey();
                        return;
                    }
                }

                Console.Write("Novo nome: ");
                string nome = Console.ReadLine();

                Console.Write("Nova descrição: ");
                string desc = Console.ReadLine();

                Console.Write("Novo ID de médico: ");
                int idMedico = int.Parse(Console.ReadLine());

                string update = "UPDATE especialidade SET nomeEspecialidade = @nome, descricao = @desc, idMedico = @idMedico WHERE id = @id";
                using (var cmd = new MySqlCommand(update, conn))
                {
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@desc", desc);
                    cmd.Parameters.AddWithValue("@idMedico", idMedico);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Especialidade atualizada com sucesso!");
            }

            Console.ReadKey();
        }

        public static void DeletarEspecialidade()
        {
            Console.Clear();
            Console.WriteLine("=== DELETAR ESPECIALIDADE ===");
            Console.Write("Informe o ID da especialidade: ");
            int id = int.Parse(Console.ReadLine());

            using (var conn = Database.GetConnection())
            {
                string delete = "DELETE FROM especialidade WHERE id = @id";
                using (var cmd = new MySqlCommand(delete, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int linhasAfetadas = cmd.ExecuteNonQuery();

                    if (linhasAfetadas > 0)
                        Console.WriteLine("Especialidade deletada com sucesso!");
                    else
                        Console.WriteLine("Especialidade não encontrada.");
                }
            }

            Console.ReadKey();
        }
    }
}
