using MySql.Data.MySqlClient;

namespace LibraryManagementSystem2
{
    public class Database
    {
        private string connectionString = "server=localhost;user id=root;password=1234;database=LibraryDB";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}
