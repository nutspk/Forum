using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCrypt.Net;

namespace Selectcon
{
    public class Encryptions
    {
        const int cost = 12;

        public string GetRandomSalt() {
            return BCrypt.Net.BCrypt.GenerateSalt(cost);
        }



        public string HashPassword(string Password)
        {
            return BCrypt.Net.BCrypt.HashPassword(Password + "Nut.SPK", GetRandomSalt());
        }

        public bool VerifyPassword(string Password, string HashPassword) {
            bool validPassword = false;
            try {
                validPassword = BCrypt.Net.BCrypt.Verify(Password + "Nut.SPK", HashPassword);
            }
            catch (Exception ex) {
                Project.GetErrorMessage(ex.Message);
            }
            return validPassword;
        }

    }
}