using System;

namespace CronAbsence.Domain.Models
{
    public class CatAbsence
    {
        public int Id { get; set; }
        public int Matricule { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime DateAbsence { get; set; }
        public int AbsenceStatutId { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool UpdateFlag { get; set; }
        public string Type { get; set; }
    }
}
