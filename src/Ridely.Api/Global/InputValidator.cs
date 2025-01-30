using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Ridely.Api.Global
{
    public static class InputValidator
    {
        public static bool ValidateName(string? lastName)
        {
            //if (lastName.IsNullOrEmpty())
            //{
            //    return false;
            //}
            string pattern = @"^[a-zA-Z]+$";
            return Regex.IsMatch(lastName, pattern);
        }


        public static bool ValidateEmail(string? email)
        {
            //if (email.IsNullOrEmpty())
            //{
            //    return false;
            //}
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool ValidateUserName(string? userName)
        {
            //if (userName.IsNullOrEmpty())
            //{
            //    return false;
            //}
            string pattern = @"^[a-zA-Z0-9_]{4,}$";
            return Regex.IsMatch(userName, pattern);
        }

        public static bool ValidatePhone(string? phone)
        {
            //if (phone.IsNullOrEmpty())
            //{
            //    return false;
            //}
            string pattern = @"^\+(?:[0-9] ?){6,14}[0-9]$";
            return Regex.IsMatch(phone, pattern);
        }

        public static bool ValidateDOB(DateTime? dob, int MinAge = 19)
        {
            if (dob == null)
                return false;

            int age = DateTime.Today.Year - dob.Value.Year;
            if (dob.Value.Date > DateTime.Today.AddYears(-age)) // Adjust for leap years
                age--;

            return age >= MinAge;
        }

        public static bool ValidatePassword(string? password)
        {
            //if (password.IsNullOrEmpty())
            //{
            //    return false;
            //}
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
            return Regex.IsMatch(password, pattern);
        }
    }
}
