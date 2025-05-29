using MySql.Data.MySqlClient;

namespace Database
{
    public static class Database
    {
        private static readonly string connectionString = "Server=localhost;Database=Clinica;Uid=root;Pwd=;";

        public static MySqlConnection GetConnection()
        {
            var conn = new MySqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
