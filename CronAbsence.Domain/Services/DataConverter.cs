using System.Data;
using CronAbsence.Domain.Interfaces;
using CronAbsence.Domain.Models;


namespace CronAbsence.Domain.Service
{
    public class DataConverter : IDataConverter
    {
        private readonly IPilotageRepository _PilotageRepository;

        public DataConverter(IPilotageRepository PilotageRepository)
        {
            _PilotageRepository = PilotageRepository;
        }

        public List<CatAbsence> ConvertDataTableToCatAbsenceList(DataTable dataTable)
        {
            var catAbsences = new List<CatAbsence>();
            int maxId = _PilotageRepository.GetMaxIdFromDatabase();

            foreach (DataRow row in dataTable.Rows)
            {
                maxId++;
                var catAbsence = new CatAbsence
                {
                    ID = maxId,
                    Matricule = Convert.ToInt32(row["Matricule"]),
                    Nom = row["Nom"].ToString(),
                    Prenom = row["Prenom"].ToString(),
                    Date = DateTime.ParseExact(row["Date"].ToString(), "dd/MM/yyyy", null),
                    Type = ParseType(row["Nombre Heures"].ToString()),
                    Debut = row["Debut"].ToString(),
                    Fin = row["Fin"].ToString(),
                    Motif = row["Motif"].ToString().Equals("REPOS ASTREINTE", StringComparison.InvariantCultureIgnoreCase) ? "REPOS ASTREINTE" : "ABSENCE",
                    Flag = 0
                };

                catAbsences.Add(catAbsence);
            }
            return catAbsences;
        }

        private int ParseType(string nombreHeures)
        {
            if (nombreHeures.StartsWith("J"))
                return 3;
            else if (nombreHeures.StartsWith("M"))
                return 1;
            else if (nombreHeures.StartsWith("A"))
                return 2;
            else
                throw new ArgumentException("Invalid value for Nombre Heures");
        }
    }
}
