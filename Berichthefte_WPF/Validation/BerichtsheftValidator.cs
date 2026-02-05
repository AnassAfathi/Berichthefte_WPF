using Berichthefte_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Berichthefte_WPF.Validation
{
    public class BerichtsheftValidator
    {
        public static bool ValidateTraineeInfo(TraineeInfo traineeInfo, out List<string> validationErrors)
        {
            validationErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(traineeInfo.Name))
            {
                validationErrors.Add("Name is required.");
            }
            if (string.IsNullOrWhiteSpace(traineeInfo.Firma))
            {
                validationErrors.Add("Firma is required.");
            }
            if (string.IsNullOrWhiteSpace(traineeInfo.Abteilung))
            {
                validationErrors.Add("Abteilung is required.");
            }
            if (traineeInfo.Ausbildungsjahr < 1 || traineeInfo.Ausbildungsjahr > 4)
            {
                validationErrors.Add("Ausbildungsjahr must be between 1 and 4.");
            }
            return validationErrors.Count == 0;
        }
    }
}
