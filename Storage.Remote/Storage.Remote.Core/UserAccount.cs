using System;
using System.Collections.Generic;
using System.Text;

namespace Amica.vNext.Storage
{
    public class UserAccount
    {
        public UserAccount ()
        {
        }

		public string Username { get; set; }
		public string Password { get; set; }
        public bool IsLoggedIn { get; } = false;

        public bool Login()
        {
            return false;
        }
    }
}
