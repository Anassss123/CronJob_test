using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                        @"INSERT INTO Cat_absence (ID,Matricule, Nom, Prenom, Date, Type, Debut, Fin, Motif, Flag) 
                          VALUES (@ID,@Matricule, @Nom, @Prenom, @Date, @Type, @Debut, @Fin, @Motif, @Flag)",
                        new
                        {
                            absence.ID,
                            absence.Matricule,
                            absence.Nom,
                            absence.Prenom,
                            absence.Date,
                            absence.Type,
                            absence.Debut,
                            absence.Fin,
                            absence.Motif,
                            absence.Flag
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
                            Type = @Type, 
                            Debut = @Debut, 
                            Fin = @Fin, 
                            Motif = @Motif, 
                            Flag = @Flag,
                            LastUpdated = @dateNow
                        WHERE Matricule = @Matricule
                        AND Date = @Date
                        AND Motif = @Motif",
                        new
                        {
                            absence.Nom,
                            absence.Prenom,
                            absence.Type,
                            absence.Debut,
                            absence.Fin,
                            absence.Motif,
                            absence.Flag,
                            dateNow = DateTime.Now,
                            absence.Matricule,
                            absence.Date
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
