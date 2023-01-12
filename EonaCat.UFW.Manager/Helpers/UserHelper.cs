using EonaCat.Linux.UFW;
using System;
using System.Threading.Tasks;

namespace EonaCat.UFW.Manager.Helpers
{
    internal class UFWUser
    {
        public bool HasPassword => IsRoot || (HasSudo && !string.IsNullOrEmpty(Password));
        private string Password { get; set; }
        public string Username { get; set; }
        public bool IsRoot { get; internal set; }
        private bool _hasSudo;
        public bool HasSudo
        {
            get
            {
                return _hasSudo;
            }

            set
            {
                if (IsRoot)
                {
                    _hasSudo = true;
                    return;
                }
                _hasSudo = value;
            }
        }
    }
    internal class UserHelper
    {
        private UFWUser _user;
        public UFWUser UFWUser
        {
            get
            {
                if (_user == null)
                {
                    _user = new UFWUser();
                }
                return _user;
            }
        }

        internal async Task<string> GetUsernameAsync()
        {
            var result = await Linux.UFW.Manager.GetUsername();
            _user.IsRoot = result == "root";
            if (!_user.IsRoot)
            {
                _user.HasSudo = await Linux.UFW.Manager.HasSudoRights();
            }
            return result;
        }

        internal async Task SetPasswordAsync(string password)
        {
            if (!_user.HasSudo)
            {
                // We don't have sudo or are root so deny the request
                return;
            }
            await Linux.UFW.Manager.SetPasswordAsync(password);
        }
    }
}