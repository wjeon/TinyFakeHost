using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Internal;
using System.Reflection;

namespace TinyFakeHostHelper.Tests.Unit.Helpers
{
    public static class AppSettingHelper
    {
        public static void AddAppSettingInMemory(string key, string value)
        {
            UpdateAppSettingInMemory(AppSettingUpdateType.Add, key, value);
        }

        public static void RemoveAppSettingInMemory(string key)
        {
            UpdateAppSettingInMemory(AppSettingUpdateType.Remove, key, null);
        }

        private static void UpdateAppSettingInMemory(AppSettingUpdateType updateType, string key, string value)
        {
            var configSystem = new ConfigSystem();

            if (updateType == AppSettingUpdateType.Remove)
                configSystem.Settings.Remove(key);
            else
                configSystem.Settings.Add(key, value);

            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static;

            var fieldInfo = typeof(ConfigurationManager)
                .GetField("s_configSystem", flags);

            if (fieldInfo != null)
                fieldInfo.SetValue(null, configSystem);
        }

        private enum AppSettingUpdateType
        {
            Add,
            Remove
        }

        private class ConfigSystem : IInternalConfigSystem
        {
            public readonly NameValueCollection Settings = new NameValueCollection();

            public object GetSection(string configKey)
            {
                return Settings;
            }

            public void RefreshConfig(string sectionName)
            {
            }

            public bool SupportsUserConfig { get; private set; }
        }
    }
}
