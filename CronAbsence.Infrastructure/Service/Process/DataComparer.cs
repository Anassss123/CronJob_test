using CronAbsence.Domain.Models;
using CronAbsence.Infrastructure.Service.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CronAbsence.Infrastructure.Service.Process
{
    public class DataComparer : IDataComparer
    {
        private readonly IDatabaseReaderService _databaseReaderService;

        public DataComparer(IDatabaseReaderService databaseReaderService)
        {
            _databaseReaderService = databaseReaderService;
        }

        public async Task<IEnumerable<CatAbsence>> CompareDataAsync(DataTable excelData)
        {
            var dbCatAbsences = await _databaseReaderService.GetCatAbsencesAsync();
            var updatedDataList = new List<CatAbsence>();

            foreach (DataRow excelRow in excelData.Rows)
            {
                var matricule = Convert.ToInt32(excelRow["Matricule"]);
                var dbAbsence = dbCatAbsences.FirstOrDefault(a => a.Matricule == matricule);

                if (dbAbsence != null)
                {
                    dbAbsence.Nom = Convert.ToString(excelRow["Nom"]);
                    dbAbsence.Prenom = Convert.ToString(excelRow["Prenom"]);
                    dbAbsence.DateAbsence = Convert.ToDateTime(excelRow["DateAbsence"]);
                    dbAbsence.AbsenceStatutId = Convert.ToInt32(excelRow["AbsenceStatutId"]);
                    dbAbsence.Type = Convert.ToString(excelRow["Type"]);
                    updatedDataList.Add(dbAbsence);
                }
                else
                {
                    var newAbsence = new CatAbsence
                    {
                        Matricule = matricule,
                        Nom = Convert.ToString(excelRow["Nom"]),
                        Prenom = Convert.ToString(excelRow["Prenom"]),
                        DateAbsence = Convert.ToDateTime(excelRow["DateAbsence"]),
                        AbsenceStatutId = Convert.ToInt32(excelRow["AbsenceStatutId"]),
                        Type = Convert.ToString(excelRow["Type"])
                    };
                    updatedDataList.Add(newAbsence);
                }
            }

            return updatedDataList;
        }
    }
}
