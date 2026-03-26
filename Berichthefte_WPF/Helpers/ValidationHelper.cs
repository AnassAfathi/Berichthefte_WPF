using System;
using System.Collections.Generic;

namespace Berichthefte_WPF.Helpers
{
    public class ValidationHelper
    {
        public static (bool IsValid, string ErrorMessage) ValidateBetriebActivity(string description, string hours)
        {
            var errors = new List<string>();

            // Validate Beschreibung (Description)
            if (string.IsNullOrWhiteSpace(description))
            {
                errors.Add("Beschreibung ist erforderlich");
            }
            else if (description.Length > 200)
            {
                errors.Add("Beschreibung darf maximal 200 Zeichen lang sein");
            }

            // Validate Stunden (Hours)
            if (string.IsNullOrWhiteSpace(hours))
            {
                errors.Add("Stunden sind erforderlich");
            }
            else if (!double.TryParse(hours, out double parsedHours))
            {
                errors.Add("Stunden müssen eine Zahl sein");
            }
            else if (parsedHours < 0)
            {
                errors.Add("Stunden dürfen nicht negativ sein");
            }
            else if (parsedHours > 24)
            {
                errors.Add("Stunden dürfen nicht mehr als 24 pro Tag sein");
            }

            if (errors.Count > 0)
            {
                return (false, string.Join("\n", errors));
            }

            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateSchoolActivity(string fach, string beschreibung)
        {
            var errors = new List<string>();

            // Validate Fach (Subject)
            if (string.IsNullOrWhiteSpace(fach))
            {
                errors.Add("Fach ist erforderlich");
            }
            else if (fach.Length > 100)
            {
                errors.Add("Fach darf maximal 100 Zeichen lang sein");
            }

            // Validate Beschreibung (Description) - optional but check length if provided
            if (!string.IsNullOrWhiteSpace(beschreibung) && beschreibung.Length > 300)
            {
                errors.Add("Beschreibung darf maximal 300 Zeichen lang sein");
            }

            if (errors.Count > 0)
            {
                return (false, string.Join("\n", errors));
            }

            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateTraineeInfo(string name, string firma, string abteilung)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(name))
                errors.Add("Name ist erforderlich");

            if (string.IsNullOrWhiteSpace(firma))
                errors.Add("Firma ist erforderlich");

            if (string.IsNullOrWhiteSpace(abteilung))
                errors.Add("Abteilung ist erforderlich");

            if (errors.Count > 0)
            {
                return (false, string.Join("\n", errors));
            }

            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateSchoolHours(string schoolHours)
        {
            if (string.IsNullOrWhiteSpace(schoolHours))
            {
                return (true, string.Empty); // Optional field
            }

            if (!double.TryParse(schoolHours, out double parsedHours))
            {
                return (false, "Schulstunden müssen eine Zahl sein");
            }

            if (parsedHours < 0)
            {
                return (false, "Schulstunden dürfen nicht negativ sein");
            }

            if (parsedHours > 168) // Maximum 168 hours per week
            {
                return (false, "Schulstunden dürfen nicht mehr als 168 pro Woche sein");
            }

            return (true, string.Empty);
        }
    }
}
