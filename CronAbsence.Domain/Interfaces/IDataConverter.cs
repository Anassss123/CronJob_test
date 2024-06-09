using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CronAbsence.Domain.Models;

namespace CronAbsence.Domain.Interfaces
{
   public interface IDataConverter
    {
        List<CatAbsence> ConvertDataTableToCatAbsenceList(DataTable dataTable);
    }
}