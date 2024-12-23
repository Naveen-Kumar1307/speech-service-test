using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.ServiceModel;
using Common.Logging;

namespace GlobalEnglish.SpeechRecognition.Repository
{
    internal static class DataHelper
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(DataHelper));

		#region Data accessors

		/// <summary>
        /// Gets a boolean value of a data reader by a column name
        /// </summary>
        /// <param name="rdr">Data reader</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A boolean value</returns>
        public static bool GetBoolean(IDataReader rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index))
            {
                string message = "Non null value expected. ";
                Logger.Error(message);
                throw new FaultException(message);
            }
            return Convert.ToBoolean(rdr[index]);
        }

        /// <summary>
        /// Gets a byte array of a data reader by a column name
        /// </summary>
        /// <param name="rdr">Data reader</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A byte array</returns>
        public static byte[] GetBytes(IDataReader rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index))
            {
                return null;
            }
            return (byte[])rdr[index];
        }

        /// <summary>
        /// Gets a datetime value of a data reader by a column name
        /// </summary>
        /// <param name="rdr">Data reader</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A date time</returns>
        public static DateTime GetDateTime(IDataReader rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index))
            {
                string message = "Non null value expected. ";
				Logger.Error(message);
				throw new FaultException(message);
            }
            return (DateTime)rdr[index];
        }

        /// <summary>
        /// Gets a decimal value of a data reader by a column name
        /// </summary>
        /// <param name="rdr">Data reader</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A decimal value</returns>
        public static decimal GetDecimal(IDataReader rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index))
            {
                string message = "Non null value expected. ";
				Logger.Error(message);
				throw new FaultException(message);
            }
            return Convert.ToDecimal(rdr[index]);
        }

        /// <summary>
        /// Gets a double value of a data reader by a column name
        /// </summary>
        /// <param name="rdr">Data reader</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A double value</returns>
        public static double GetDouble(IDataReader rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index))
            {
                string message = "Non null value expected. ";
				Logger.Error(message);
				throw new FaultException(message);
            }
            return (double)rdr[index];
        }

        /// <summary>
        /// Gets a GUID value of a data reader by a column name
        /// </summary>
        /// <param name="rdr">Data reader</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A GUID value</returns>
        public static Guid GetGuid(IDataReader rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index))
            {
                string message = "Non null value expected. ";
				Logger.Error(message);
				throw new FaultException(message);
            }
            return (Guid)rdr[index];
        }

        /// <summary>
        /// Gets an integer value of a data reader by a column name
        /// </summary>
        /// <param name="rdr">Data reader</param>
        /// <param name="columnName">Column name</param>
        /// <returns>An integer value</returns>
        public static int GetInt(IDataReader rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index))
            {
                string message = "Non null value expected. ";
				Logger.Error(message);
				throw new FaultException(message);
            }
            return Convert.ToInt32(rdr[index]);
        }

		/// <summary>
		/// Gets a nullable value of a data reader by a column name
		/// </summary>
		/// <param name="rdr">Data reader</param>
		/// <param name="columnName">Column name</param>
		/// <returns>nullable value</returns>
		public static Nullable<T> GetNullable<T>(IDataReader reader, string columnName) where T : struct
		{
			object columnValue = reader[columnName];

			if (!(columnValue is DBNull))
				return (T)Convert.ChangeType(columnValue, typeof(T));

			return null;
		}

		/// <summary>
		/// Gets an integer value of a data reader by a column name
		/// </summary>
		/// <param name="rdr">Data reader</param>
		/// <param name="columnName">Column name</param>
		/// <returns>An integer value</returns>
		public static T GetEnum<T>(IDataReader rdr, string columnName)
		{
			int index = rdr.GetOrdinal(columnName);
			if (rdr.IsDBNull(index))
			{
				string message = "Non null value expected. ";
				Logger.Error(message);
				throw new FaultException(message);
			}

			return (T)Enum.Parse(typeof(T), rdr[index].ToString());
		}

        /// <summary>
        /// Gets a string of a data reader by a column name
        /// </summary>
        /// <param name="rdr">Data reader</param>
        /// <param name="columnName">Column name</param>
        /// <returns>A string value</returns>
        public static string GetString(IDataReader rdr, string columnName)
        {
            int index = rdr.GetOrdinal(columnName);
            if (rdr.IsDBNull(index))
            {
                return null;
            }
            return (string)rdr[index];
		}

		#endregion Data accessors
	}
}
