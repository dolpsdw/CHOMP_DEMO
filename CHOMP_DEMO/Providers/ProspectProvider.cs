using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace CHOMP_DEMO.Providers
{
    public class ProspectProvider : IProspectProvider
    {
        private readonly string _connectionString;

        public ProspectProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ICollection<T> Procedure<T>(string procedureName, DynamicParameters parameters)
        {
            ICollection<T> result;

            using (var connection = new SqlConnection(_connectionString))
            {
                result = connection.Query<T>(procedureName, parameters, commandType: CommandType.StoredProcedure).AsList();
            }

            return result;
        }

    }
}