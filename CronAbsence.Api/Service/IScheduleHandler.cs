using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CronAbsence.Api.Service
{
    public interface IScheduleHandler
    {
        Task ProcessAsync();
    }
}