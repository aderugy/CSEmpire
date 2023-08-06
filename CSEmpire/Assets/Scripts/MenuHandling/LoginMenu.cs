using TMPro;
using UnityEngine.UI;

namespace MenuHandling
{
    public class LoginMenu : Menu
    {
        public TMP_InputField UsernameLoginTMP_InputField;
        public TMP_InputField PasswordLoginTMP_InputField;
        public TMP_InputField UsernameRegisterTMP_InputField;
        public TMP_InputField EmailRegisterTMP_InputField;
        public TMP_InputField PasswordRegisterTMP_InputField;

        public async void Login()
        {
            string username = UsernameLoginTMP_InputField.text;
            string password = PasswordLoginTMP_InputField.text;

            if (username.Length == 0 || password.Length == 0)
                return;

            string hashedPassword = Credentials.HashBCrypt(password);
            
            if (await Credentials.Instance.Login(username, hashedPassword))
                LobbyMenuManager.Instance.OpenMainMenu();
            else
                PasswordLoginTMP_InputField.text = "";
        }

        public async void Register()
        {
            string username = UsernameRegisterTMP_InputField.text;
            string password = PasswordRegisterTMP_InputField.text;
            string email = EmailRegisterTMP_InputField.text;

            if (username.Length == 0 || password.Length == 0 || email.Length == 0)
                return;
            
            if (await Credentials.Instance.Register(username, password, email))
                LobbyMenuManager.Instance.OpenMainMenu();
        }
    }
}
