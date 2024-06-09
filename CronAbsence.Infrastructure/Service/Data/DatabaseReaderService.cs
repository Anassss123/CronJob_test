using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using CronAbsence.Domain.Models;
using Microsoft.Extensions.Configuration;
using CronAbsence.Domain.Interfaces;

namespace CronAbsence.Infrastructure.Service.Data
{
    public class DatabaseReaderService : IDatabaseReaderService
    {
        private readonly string _connectionString;

        public DatabaseReaderService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        public async Task<IEnumerable<CatAbsence>> GetCatAbsencesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<CatAbsence>("[dbo].[ps_cat_absence_s_by_date]", commandType: CommandType.StoredProcedure);
            }
        }

        public async Task InsertCatAbsencesAsync(IEnumerable<CatAbsence> newAbsences)
        {
            if (newAbsences == null || !newAbsences.Any())
                return;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                foreach (var absence in newAbsences)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@ID", absence.ID);
                    parameters.Add("@Matricule", absence.Matricule);
                    parameters.Add("@Nom", absence.Nom);
                    parameters.Add("@Prenom", absence.Prenom);
                    parameters.Add("@Date", absence.Date);
                    parameters.Add("@Type", absence.Type);
                    parameters.Add("@Debut", absence.Debut);
                    parameters.Add("@Fin", absence.Fin);
                    parameters.Add("@Motif", absence.Motif);
                    parameters.Add("@Flag", absence.Flag);

                    await connection.ExecuteAsync("[dbo].[ps_cat_absence_i]", parameters, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public async Task UpdateCatAbsencesAsync(IEnumerable<CatAbsence> updatedAbsences)
        {
            if (updatedAbsences == null || !updatedAbsences.Any())
                return;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                foreach (var absence in updatedAbsences)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Matricule", absence.Matricule);
                    parameters.Add("@Nom", absence.Nom);
                    parameters.Add("@Prenom", absence.Prenom);
                    parameters.Add("@Date", absence.Date);
                    parameters.Add("@Type", absence.Type);
                    parameters.Add("@Debut", absence.Debut);
                    parameters.Add("@Fin", absence.Fin);
                    parameters.Add("@Motif", absence.Motif);
                    parameters.Add("@Flag", absence.Flag);
                    parameters.Add("@LastUpdated", DateTime.Now);

                    await connection.ExecuteAsync("[dbo].[ps_cat_absence_u]", parameters, commandType: CommandType.StoredProcedure);
                }
            }
        }
        public int GetMaxIdFromDatabase()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT MAX(ID) FROM Cat_absence";
                int maxId = connection.ExecuteScalar<int>(query);
                return maxId;
            }
        }
    }
}
