using UnityEngine;

namespace _Development.Scripts.Upgrade.Data
{
    public static class PlayerPrefsAppCount
    {
        private static string NameApp = "AppCount";

        public static void SaveAppCount(int count) => 
            PlayerPrefs.SetInt(NameApp, count);

        public static int LoadAppCount() => 
            PlayerPrefs.GetInt(NameApp);
    }
}