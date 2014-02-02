using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System.Text;

namespace ContosoMobile
{
    /// <summary>
    /// Settings Model
    /// </summary>
    public class SettingsModel
    {
        private static SettingsModel _settings;

        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public string AzureNamespace
        {
            get;
            set;
        }

        public string StsEndpoint
        {
            get;
            set;
        }

        public static SettingsModel GetSettings()
        {
            if (_settings == null)
            {
                IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

                // Look for settings in isolated storage
                if (settings.Count > 0)
                {
                    _settings = new SettingsModel()
                        {
                            UserName = ConvertEncryptedBytesToString((byte[])settings["UserName"]),
                            Password = ConvertEncryptedBytesToString((byte[])settings["Password"]),
                            AzureNamespace = (string)settings["AzureNamespace"],
                            StsEndpoint = (string)settings["StsEndpoint"]
                        };
                }
                // If not found, the app is starting up for the first time and we need to initialize with the default values
                else
                {
                    _settings = new SettingsModel();
                    _settings.SaveSettings();
                }
            }

            return _settings;
        }

        public void SaveSettings()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            settings["UserName"] = ConvertStringToEncryptedBytes(this.UserName ?? "");
            settings["Password"] = ConvertStringToEncryptedBytes(this.Password ?? "");
            settings["AzureNamespace"] = this.AzureNamespace ?? "";
            settings["StsEndpoint"] = this.StsEndpoint ?? "";
        }

        private static byte[] ConvertStringToEncryptedBytes(string unencryptedInput)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(unencryptedInput);
            return ProtectedData.Protect(stringBytes, null);
        }

        private static string ConvertEncryptedBytesToString(byte[] encryptedInput)
        {
            byte[] stringBytes = ProtectedData.Unprotect(encryptedInput, null);
            return Encoding.UTF8.GetString(stringBytes, 0, stringBytes.Length);
        }
    }
}