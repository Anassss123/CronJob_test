using System.Data;
using CronAbsence.Domain.Models;

namespace CronAbsence.Domain.Interfaces
{
    public interface IDataConverter
    {
        List<CatAbsence> ConvertDataTableToCatAbsenceList(DataTable dataTable);
    }
}