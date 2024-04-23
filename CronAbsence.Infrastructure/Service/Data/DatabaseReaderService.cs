using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using CronAbsence.Domain.Models;

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
                return await connection.QueryAsync<CatAbsence>("SELECT * FROM Cat_absence");
            }
        }

        public async Task UpdateCatAbsencesAsync(IEnumerable<CatAbsence> catAbsences)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                foreach (var catAbsence in catAbsences)
                {
                    await connection.ExecuteAsync(
                        "UPDATE Cat_absence SET Nom = @Nom, Prenom = @Prenom, DateAbsence = @DateAbsence, AbsenceStatutId = @AbsenceStatutId, Type = @Type, LastUpdate = @LastUpdate WHERE Matricule = @Matricule",
                        new
                        {
                            catAbsence.Nom,
                            catAbsence.Prenom,
                            catAbsence.DateAbsence,
                            catAbsence.AbsenceStatutId,
                            catAbsence.Type,
                            catAbsence.LastUpdate,
                            catAbsence.Matricule
                        });
                }
            }
        }
    }
}
