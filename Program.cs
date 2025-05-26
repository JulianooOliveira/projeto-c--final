using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using models;
using MySql.Data.MySqlClient;
using Database;

class Program
{
    static List<Paciente> pacientes = new List<Paciente>();

    static void Main(string[] args)
    {
        bool rodando = true;

        while (rodando)
        {
            Console.Clear();
            Console.WriteLine("=== MENU PRINCIPAL ===");
            Console.WriteLine("1 - Cadastrar paciente");
            Console.WriteLine("2 - Listar pacientes");
            Console.WriteLine("3 - Atualizar paciente");
            Console.WriteLine("4 - Deletar paciente");
            Console.WriteLine("5 - Iniciar Servidor HTTP");
            Console.WriteLine("6 - Sair");
            Console.Write("Escolha uma opção: ");

            string escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    CadastrarPaciente();
                    break;
                case "2":
                    ListarPacientes();
                    break;
                case "3":
                    AtualizarPaciente();
                    break;
                case "4":
                    DeletarPaciente();
                    break;
                case "5":
                    IniciarServidor();
                    break;
                case "6":
                    rodando = false;
                    break;
                default:
                    Console.WriteLine("Opção inválida! Pressione qualquer tecla para continuar...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    static void CadastrarPaciente()
    {
        Console.Clear();
        Console.WriteLine("=== CADASTRAR PACIENTE ===");

        Console.Write("ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID inválido!");
            Console.ReadKey();
            return;
        }

        Console.Write("Nome: ");
        string nome = Console.ReadLine();

        Console.Write("CPF: ");
        string cpf = Console.ReadLine();

        Console.Write("Data de Nascimento (dd/MM/yyyy): ");
        string dataStr = Console.ReadLine();

        if (!DateTime.TryParseExact(dataStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dataNascimento))
        {
            Console.WriteLine("Data inválida!");
            Console.ReadKey();
            return;
        }

        string connectionString = "server=localhost;database=clinica;uid=root;pwd=;";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO pacientes (id, nome, cpf, DataNascimento) VALUES (@id, @nome, @cpf, @data)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@cpf", cpf);
                    cmd.Parameters.AddWithValue("@data", dataNascimento);
                    cmd.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Paciente cadastrado com sucesso no banco de dados!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao cadastrar paciente: {ex.Message}");
        }

        Console.ReadKey();
    }

    static void ListarPacientes()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA DE PACIENTES ===");

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
                        Console.WriteLine("Nenhum paciente encontrado.");
                    }
                    else
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id");
                            string nome = reader.GetString("nome");
                            string cpf = reader.GetString("cpf");
                            DateTime nascimento = reader.GetDateTime("DataNascimento");

                            Console.WriteLine($"ID: {id} | Nome: {nome} | CPF: {cpf} | Nasc: {nascimento:dd/MM/yyyy}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao listar pacientes: {ex.Message}");
        }

        Console.WriteLine("Pressione qualquer tecla para voltar...");
        Console.ReadKey();
    }

    static void AtualizarPaciente()
    {
        Console.Clear();
        Console.WriteLine("=== ATUALIZAR PACIENTE ===");
        Console.Write("Informe o CPF do paciente: ");
        string cpf = Console.ReadLine();

        string connectionString = "server=localhost;database=clinica;uid=root;pwd=;";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Verifica se existe
                string selectQuery = "SELECT * FROM pacientes WHERE cpf = @cpf";
                using (MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn))
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
                string dataStr = Console.ReadLine();

                if (!DateTime.TryParseExact(dataStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime novaData))
                {
                    Console.WriteLine("Data inválida!");
                    Console.ReadKey();
                    return;
                }

                // Atualiza
                string updateQuery = "UPDATE pacientes SET nome = @nome, DataNascimento = @data WHERE cpf = @cpf";
                using (MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn))
                {
                    updateCmd.Parameters.AddWithValue("@nome", novoNome);
                    updateCmd.Parameters.AddWithValue("@data", novaData);
                    updateCmd.Parameters.AddWithValue("@cpf", cpf);
                    updateCmd.ExecuteNonQuery();
                }

                Console.WriteLine("Paciente atualizado com sucesso!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao atualizar paciente: {ex.Message}");
        }

        Console.ReadKey();
    }

    static void DeletarPaciente()
    {
        Console.Clear();
        Console.WriteLine("=== DELETAR PACIENTE ===");
        Console.Write("Informe o CPF do paciente: ");
        string cpf = Console.ReadLine();

        string connectionString = "server=localhost;database=clinica;uid=root;pwd=;";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string deleteQuery = "DELETE FROM pacientes WHERE cpf = @cpf";
                using (MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn))
                {
                    deleteCmd.Parameters.AddWithValue("@cpf", cpf);
                    int linhasAfetadas = deleteCmd.ExecuteNonQuery();

                    if (linhasAfetadas > 0)
                        Console.WriteLine("Paciente deletado com sucesso!");
                    else
                        Console.WriteLine("Paciente não encontrado.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao deletar paciente: {ex.Message}");
        }

        Console.ReadKey();
    }

    static void IniciarServidor()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();

        Console.Clear();
        Console.WriteLine("Servidor rodando em http://localhost:8080/");
        Console.WriteLine("Pressione Ctrl+C para encerrar.\n");

        while (true)
        {
            var context = listener.GetContext();
            var req = context.Request;
            var res = context.Response;

            string path = req.Url.AbsolutePath.ToLower();

            if (path == "/getpaciente" && req.HttpMethod == "GET")
            {
                Rotas.GetPaciente.Executar(res);
            }
            else if (path == "/setpaciente" && req.HttpMethod == "POST")
            {
                Rotas.SetPaciente.Executar(req, res);
            }
            else if (path == "/putpaciente" && req.HttpMethod == "PUT")
            {
                Rotas.PutPaciente.Executar(req, res, pacientes);
            }
            else if (path == "/deletepaciente" && req.HttpMethod == "DELETE")
            {
                Rotas.DeletePaciente.Executar(req, res);
            }
            else
            {
                res.StatusCode = 404;
                byte[] msg = Encoding.UTF8.GetBytes("Rota ou método HTTP não encontrado.");
                res.OutputStream.Write(msg, 0, msg.Length);
                res.Close();
            }
        }
    }
}
