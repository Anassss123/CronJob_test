using System;

namespace CronAbsence.Domain.Models
{
    public class CatAbsence
    {
        public int ID { get; set; }
        public int Matricule { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime Date { get; set; }
        public int Type { get; set; }
        public string Debut { get; set; }
        public string Fin { get; set; }
        public string Motif { get; set; }
        public int Flag { get; set; }
        public DateTime LastUpdated {get; set;}
    }
}
