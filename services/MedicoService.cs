using System;
using MySql.Data.MySqlClient;
using Database;

namespace Services
{
    public static class MedicoService
    {
        public static void CadastrarMedico()
        {
            Console.Clear();
            Console.WriteLine("=== CADASTRAR MÉDICO ===");

            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine());

            Console.Write("Nome: ");
            string nome = Console.ReadLine();

            Console.Write("CRM: ");
            int crm = int.Parse(Console.ReadLine());

            Console.Write("Data de Nascimento (dd/MM/yyyy): ");
            DateTime dataNascimento = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);

            using (var conn = Database.GetConnection())
            {
                string verificaQuery = "SELECT COUNT(*) FROM medico WHERE crmMedico = @crm";
                using (var cmd = new MySqlCommand(verificaQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@crm", crm);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0)
                    {
                        Console.WriteLine("Médico com esse CRM já cadastrado.");
                        Console.ReadKey();
                        return;
                    }
                }

                string insertQuery = @"INSERT INTO medico (id, nomeMedico, crmMedico, dataNascimentoMedico)
                                       VALUES (@id, @nome, @crm, @data)";
                using (var cmd = new MySqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@crm", crm);
                    cmd.Parameters.AddWithValue("@data", dataNascimento);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Médico cadastrado com sucesso!");
            }

            Console.ReadKey();
        }

        public static void ListarMedicos()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA DE MÉDICOS ===");

            using (var conn = Database.GetConnection())
            {
                string query = "SELECT id, nomeMedico, crmMedico, dataNascimentoMedico FROM medico";
                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Nenhum médico encontrado.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string nome = reader.GetString("nomeMedico");
                            int crm = reader.GetInt32("crmMedico");
                            DateTime nascimento = reader.GetDateTime("dataNascimentoMedico");

                            Console.WriteLine($"ID: {id} | Nome: {nome} | CRM: {crm} | Nasc: {nascimento:dd/MM/yyyy}");
                        }
                    }
                }
            }

            Console.WriteLine("Pressione qualquer tecla para voltar...");
            Console.ReadKey();
        }

        public static void AtualizarMedico()
        {
            Console.Clear();
            Console.WriteLine("=== ATUALIZAR MÉDICO ===");
            Console.Write("Informe o ID do médico: ");
            int id = int.Parse(Console.ReadLine());

            using (var conn = Database.GetConnection())
            {
                string selectQuery = "SELECT * FROM medico WHERE id = @id";
                using (var selectCmd = new MySqlCommand(selectQuery, conn))
                {
                    selectCmd.Parameters.AddWithValue("@id", id);
                    using (var reader = selectCmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("Médico não encontrado.");
                            Console.ReadKey();
                            return;
                        }
                    }
                }

                Console.Write("Novo nome: ");
                string nome = Console.ReadLine();

                Console.Write("Novo CRM: ");
                int crm = int.Parse(Console.ReadLine());

                Console.Write("Nova data de nascimento (dd/MM/yyyy): ");
                DateTime data = DateTime.ParseExact(Console.ReadLine(), "dd/MM/yyyy", null);

                string updateQuery = "UPDATE medico SET nomeMedico = @nome, crmMedico = @crm, dataNascimentoMedico = @data WHERE id = @id";
                using (var cmd = new MySqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@crm", crm);
                    cmd.Parameters.AddWithValue("@data", data);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("Médico atualizado com sucesso!");
            }

            Console.ReadKey();
        }

        public static void DeletarMedico()
        {
            Console.Clear();
            Console.WriteLine("=== DELETAR MÉDICO ===");
            Console.Write("Informe o ID do médico: ");
            int id = int.Parse(Console.ReadLine());

            using (var conn = Database.GetConnection())
            {
                string deleteQuery = "DELETE FROM medico WHERE id = @id";
                using (var cmd = new MySqlCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int linhasAfetadas = cmd.ExecuteNonQuery();

                    if (linhasAfetadas > 0)
                        Console.WriteLine("Médico deletado com sucesso!");
                    else
                        Console.WriteLine("Médico não encontrado.");
                }
            }

            Console.ReadKey();
        }
    }
}
