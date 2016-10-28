using Amica.Models;

namespace Amica.vNext.Storage
{
    public class UserAccount : ObservableObject
    {

        private bool _loggedIn;
        private string _username;
        private string _password;
        private Company _company;
        private string _sbuscription;

        public UserAccount()
        {
            _loggedIn = false;
            _sbuscription = "Gratuita";
        }

        public bool LoggedIn {
            get { return _loggedIn; }
            set { SetProperty(ref _loggedIn, value); }
        } 
        /// <summary>
		/// Username. Used to authenticate the user.
		/// </summary>
		public string Username {
            get { return _username; }
		    set { SetProperty(ref _username, value); }
        }
		/// <summary>
		/// User password. Needed to authenticate the user.
		/// </summary>
		public string Password {
            get { return _password; }
		    set { SetProperty(ref _password, value); }
        }
		public Company ActiveCompany
        {
            get { return _company; }
			set { SetProperty(ref _company, value); }
        }

        public string Subscription
        {
            get { return _sbuscription; }
            set { SetProperty(ref _sbuscription, value); }
        }

    }
}
