using Amica.Models;

namespace Amica.Storage
{
    public class UserAccount : ObservableObject
    {
        private string _username;
        private string _password;

		public string Username
        {
            get { return _username; }
		    set { SetProperty(ref _username, value); }
        }
		public string Password
        {
            get { return _password; }
		    set { SetProperty(ref _password, value); }
        }
    }
}
