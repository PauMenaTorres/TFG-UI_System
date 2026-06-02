namespace ModularUIRuntime
{
    public static class ModularUIPaths
    {
#if UNITY_EDITOR
        public static string BasePath
        {
            get
            {
                if (UnityEditor.AssetDatabase.IsValidFolder("Packages/com.pau.modularui"))
                {
                    return "Packages/com.pau.modularui/";
                }
                return "Assets/Plugins/ModularUI/";
            }
        }

        public static string BasePathNoSlash => BasePath.TrimEnd('/');

        public static string SettingsConfigPath => BasePathNoSlash + "/Settings/UIConfiguration.asset";
#else
        public static string SettingsConfigPath => null;
#endif
    }
}
