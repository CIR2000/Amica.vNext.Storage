using Amica.vNext.Models;

namespace Amica.vNext.Storage
{
    public class UserAccount
    {
        /// <summary>
		/// Username. Used to authenticate the user.
		/// </summary>
		public string Username { get; set; }
		/// <summary>
		/// User password. Needed to authenticate the user.
		/// </summary>
		public string Password { get; set; }
		public Company ActiveCompany { get; set; }
        public bool LoggedIn { get; set; }
    }
}
