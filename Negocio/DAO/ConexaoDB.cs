using System.Data.SqlClient;

namespace Negocio.DAO
{
    public class ConexaoDB
    {
        public static SqlConnection GetDbConnection()
        {
            // String conexão Kauan
            // string connectionString = "data source=LOCALHOST/SQLEXPRESS; database=TrabalhoMultidisciplinar; integrated security = true";
            string connectionString = "data source=LOCALHOST; database=TrabalhoMultidisciplinar; user id=sa; password=123456";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }
    }
}
