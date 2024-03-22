using CronAbsence.Domain.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Service.Data
{
    public class DatabaseReaderService : IDatabaseReaderService
    {
        private readonly string _connectionString;

        public DatabaseReaderService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<CatAbsence>> GetCatAbsencesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<CatAbsence>("SELECT * FROM Cat_absence");
            }
        }
    }
}
