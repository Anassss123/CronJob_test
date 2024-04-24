using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using CronAbsence.Domain.Models;
using Microsoft.Extensions.Configuration;

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

        public async Task InsertCatAbsencesAsync(IEnumerable<CatAbsence> newAbsences)
        {
            if (newAbsences == null || !newAbsences.Any())
                return;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                foreach (var absence in newAbsences)
                {
                    await connection.ExecuteAsync(
                        @"INSERT INTO Cat_absence (Matricule, Nom, Prenom, DateAbsence, AbsenceStatutId, Type, LastUpdate) 
                          VALUES (@Matricule, @Nom, @Prenom, @DateAbsence, @AbsenceStatutId, @Type, NULL)",
                        new
                        {
                            absence.Matricule,
                            absence.Nom,
                            absence.Prenom,
                            absence.DateAbsence,
                            absence.AbsenceStatutId,
                            absence.Type
                        });
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
                    await connection.ExecuteAsync(
                        @"UPDATE Cat_absence 
                          SET Nom = @Nom, 
                              Prenom = @Prenom, 
                              DateAbsence = @DateAbsence, 
                              AbsenceStatutId = @AbsenceStatutId, 
                              Type = @Type, 
                              LastUpdate = @dateNow
                          WHERE Matricule = @Matricule",
                        new
                        {
                            absence.Nom,
                            absence.Prenom,
                            absence.DateAbsence,
                            absence.AbsenceStatutId,
                            absence.Type,
                            dateNow = DateTime.Now,
                            absence.Matricule
                        });
                }
            }
        }

        public async Task DeleteCatAbsencesAsync(IEnumerable<CatAbsence> deletedAbsences)
        {
            if (deletedAbsences == null || !deletedAbsences.Any())
                return;

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                foreach (var absence in deletedAbsences)
                {
                    await connection.ExecuteAsync(
                        "DELETE FROM Cat_absence WHERE Matricule = @Matricule",
                        new { absence.Matricule });
                }
            }
        }
    }
}
