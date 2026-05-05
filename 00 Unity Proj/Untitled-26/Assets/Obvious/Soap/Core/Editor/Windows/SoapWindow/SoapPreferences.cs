using System.Collections.Generic;
using UnityEditor;

namespace Obvious.Soap.Editor
{
    public static class SoapPreferences
    {
        [SettingsProvider]
        public static SettingsProvider CreateSoapPreferencesProvider()
        {
            var soapSettingsWindow = new SoapWindowSettings(null);

            // Create the SettingsProvider with a path and a method to draw the GUI
            var provider = new SettingsProvider("Preferences/Soap", SettingsScope.User)
            {
                label = "Soap",

                // Define the GUI logic inside the OnGUI delegate
                guiHandler = (searchContext) => { soapSettingsWindow.Draw(); },

                // Define keywords to help search for this setting
                keywords = new HashSet<string>(new[]
                {
                    "Soap", "Settings"
                })
            };
            return provider;
        }
    }
}