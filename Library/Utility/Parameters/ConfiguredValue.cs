using System;
using System.Text;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;

namespace GlobalEnglish.Utility.Parameters
{
    /// <summary>
    /// Provides access to configured values.
    /// </summary>
    public class ConfiguredValue
    {
        private static readonly ValueConverter Converter = new ValueConverter();

        #region accessing values
        /// <summary>
        /// Returns a configured value.
        /// </summary>
        /// <typeparam name="ResultType">a configured value type</typeparam>
        /// <param name="valueName">a configured value name</param>
        /// <returns>a configured value</returns>
        public static ResultType Get<ResultType>(String valueName)
        {
            return Get<ResultType>(valueName, default(ResultType));
        }

        /// <summary>
        /// Returns a configured value, or a default (if missing).
        /// </summary>
        /// <typeparam name="ResultType">a configured value type</typeparam>
        /// <param name="valueName">a configured value name</param>
        /// <param name="defaultValue"></param>
        /// <returns>a configured value</returns>
        public static ResultType Get<ResultType>(String valueName, ResultType defaultValue)
        {
            String namedValue = Named(valueName);
            if (Argument.IsAbsent(namedValue)) return defaultValue;
            return Converter.ConvertTo<ResultType>(namedValue);
        }

        /// <summary>
        /// Returns a configured value.
        /// </summary>
        /// <param name="valueName">a configured value name</param>
        /// <returns>a configured value</returns>
        public static String Named(String valueName)
        {
            return Named(valueName, String.Empty);
        }

        /// <summary>
        /// Returns a configured value.
        /// </summary>
        /// <param name="valueName">a configured value name</param>
        /// <param name="defaultValue">a default value</param>
        /// <returns>a configured value, or the default value</returns>
        public static String Named(String valueName, String defaultValue)
        {
            String result = ConfigurationManager.AppSettings[valueName];
            return (Argument.IsAbsent(result) ? defaultValue : result.ToString());
        }

        /// <summary>
        /// Returns the configured value names.
        /// </summary>
        public static string[] ConfiguredNames
        {
            get { return ConfigurationManager.AppSettings.AllKeys; }
        }

        /// <summary>
        /// Returns a configured database connection string.
        /// </summary>
        /// <param name="connectionName">a configured connection name</param>
        /// <returns>a configured connection string, or empty</returns>
        public static String ConnectionNamed(String connectionName)
        {
            ConnectionStringSettings result =
                ConfigurationManager.ConnectionStrings[connectionName];

            return (Argument.IsAbsent(result) ? String.Empty : result.ToString());
        }

        /// <summary>
        /// Returns a configured database connection settings.
        /// </summary>
        /// <param name="connectionName">a configured connection name</param>
        /// <returns>a configured database connection settings, or null</returns>
        public static ConnectionStringSettings ConnectionSettingsNamed(String connectionName)
        {
            foreach (ConnectionStringSettings settings in ConfigurationManager.ConnectionStrings)
            {
                if (settings.Name == connectionName) return settings;
            }

            return null;
        }
        #endregion

    } // ConfiguredValue
}
