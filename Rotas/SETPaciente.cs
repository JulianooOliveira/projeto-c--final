using System.Net;
using System.Text;
using System.IO;
using System;
using MySql.Data.MySqlClient;
using Database;

namespace Rotas
{
    public static class SETPaciente
    {
        public static void Executar(HttpListenerRequest req, HttpListenerResponse res)
        {
            using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
            string body = reader.ReadToEnd();

            // Formato esperado: "Id;Nome;DataNascimento;CPF;IdEspecialidade"
            var partes = body.Split(';');
            if (partes.Length != 5 || 
                !int.TryParse(partes[0], out int id) ||
                !DateTime.TryParse(partes[2], out DateTime dataNasc) ||
                !int.TryParse(partes[4], out int idEspecialidade))
            {
                res.StatusCode = 400;
                EscreverResposta(res, "Dados inválidos. Formato esperado: Id;Nome;DataNascimento;CPF;IdEspecialidade");
                return;
            }

            string nome = partes[1];
            string cpf = partes[3];

            try
            {
                using (var conn = Database.GetConnection())
                {
                    // Verifica se a especialidade existe
                    string verificaEspecialidadeQuery = "SELECT COUNT(*) FROM especialidade WHERE id = @idEspecialidade";
                    using (var verificaCmd = new MySqlCommand(verificaEspecialidadeQuery, conn))
                    {
                        verificaCmd.Parameters.AddWithValue("@idEspecialidade", idEspecialidade);
                        int count = Convert.ToInt32(verificaCmd.ExecuteScalar());
                        if (count == 0)
                        {
                            res.StatusCode = 404;
                            EscreverResposta(res, "Especialidade não encontrada.");
                            return;
                        }
                    }

                    // Verifica se já existe paciente com o mesmo CPF
                    string verificaPacienteQuery = "SELECT COUNT(*) FROM pacientes WHERE cpf = @cpf";
                    using (var verificaCmd = new MySqlCommand(verificaPacienteQuery, conn))
                    {
                        verificaCmd.Parameters.AddWithValue("@cpf", cpf);
                        int count = Convert.ToInt32(verificaCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            res.StatusCode = 409;
                            EscreverResposta(res, "Paciente com esse CPF já cadastrado.");
                            return;
                        }
                    }

                    // Insere novo paciente
                    string insertQuery = @"INSERT INTO pacientes (id, nome, dataNascimento, cpf, idEspecialidade)
                                           VALUES (@id, @nome, @dataNascimento, @cpf, @idEspecialidade)";
                    using (var cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@nome", nome);
                        cmd.Parameters.AddWithValue("@dataNascimento", dataNasc);
                        cmd.Parameters.AddWithValue("@cpf", cpf);
                        cmd.Parameters.AddWithValue("@idEspecialidade", idEspecialidade);

                        int linhasAfetadas = cmd.ExecuteNonQuery();

                        if (linhasAfetadas > 0)
                            EscreverResposta(res, "Paciente cadastrado com sucesso.");
                        else
                            EscreverResposta(res, "Falha ao cadastrar paciente.");
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
