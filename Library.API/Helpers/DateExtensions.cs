using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public static class DateExtensions
    {
        public static int GetCurrentAge(this DateTimeOffset dateOfBirth)
        {
            return GetCurrentAge(dateOfBirth, DateTimeOffset.UtcNow);
        }
        public static int GetCurrentAge(this DateTimeOffset dateOfBirth, DateTimeOffset referenceDate)
        {
            int years = referenceDate.Year - dateOfBirth.Year;
            if (referenceDate.Month < dateOfBirth.Month || (referenceDate.Month == dateOfBirth.Month && referenceDate.Day < dateOfBirth.Day)) --years;
            return years;
        }
    }
}
