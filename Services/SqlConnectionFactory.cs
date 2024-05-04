using Microsoft.Data.SqlClient;

namespace Web.APi.Dapper.Services
{
    public class SqlConnectionFactory
    {
        private readonly string  _connectionFactory;
        public SqlConnectionFactory(string  connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public SqlConnection Create()
        {
            return new SqlConnection(_connectionFactory);
        }
    }
}
