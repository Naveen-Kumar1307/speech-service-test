using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Common.Logging;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.Practices.TransientFaultHandling;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
using GlobalEnglish.Utility.Diagnostics;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Data
{
	/// <summary>
	/// Provides access to appropriate configured connections.
	/// </summary>
	public class ConnectionFactory
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(ConnectionFactory));
		private static readonly Dictionary<string, string> ConnectionMap = new Dictionary<string, string>();

		/// <summary>
		/// Returns a named connection.
		/// </summary>
		/// <param name="connectionName">a connection name</param>
		/// <returns>a SqlConnection</returns>
		public static SqlConnection GetConnection(string connectionName)
		{
			string connectionString = GetConnectionString(connectionName);
			return new SqlConnection(connectionString);
		}

		/// <summary>
		/// Returns an appropriate connection string.
		/// </summary>
		/// <param name="connectionName">a connection name</param>
		/// <returns>a configured connection string</returns>
		public static string GetConnectionString(string connectionName)
		{
			string result = ConfiguredValue.ConnectionNamed(connectionName);


			try
			{
				if (RoleEnvironment.IsAvailable)
				{
					string candidate = RoleEnvironment.GetConfigurationSettingValue(connectionName);
					if (string.IsNullOrEmpty(candidate))
					{
						ReportMissingConnection(connectionName);
					}
					else
					{
						result = candidate;
					}
				}
			}
			catch (Exception)
			{
				ReportMissingConnection(connectionName);
			}

			// track whether a connection was previously made
			if (!ConnectionMap.ContainsKey(connectionName))
			{
				ConnectionMap.Add(connectionName, result);
			}

			return result;
		}

		/// <summary>
		/// Reports that a connection string was not overridden.
		/// </summary>
		private static void ReportMissingConnection(string connectionName)
		{
			if (Logger.IsDebugEnabled && !ConnectionMap.ContainsKey(connectionName))
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("No connection string named ");
				builder.Append(connectionName.SinglyQuoted());
				builder.Append(" in Service.cscfg, using value from Web.config");
				Logger.Debug(builder.ToString());
			}
		}

	} // ConnectionFactory


	/// <summary>
	/// Extends SqlConnection to support reliable operations (esp. with SQL Azure).
	/// </summary>
	public static class ConnectionExtensions
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(ConnectionExtensions));

		/// <summary>
		/// Returns a new command for a stored procedure.
		/// </summary>
		/// <param name="connection">this connection</param>
		/// <param name="procedureName">a stored procedure name</param>
		/// <returns>a SqlCommand</returns>
		public static SqlCommand CreateProcedure(this SqlConnection connection, string procedureName)
		{
			return connection.CreateCommand().MakeProcedure(procedureName);
		}

		/// <summary>
		/// Reports a connection retry event.
		/// </summary>
		private static void ReportConnectionRetry(object source, RetryingEventArgs retryEvent)
		{
			string message = "Retrying connection after exception: " + retryEvent.LastException.Message;
			Logger.Warn(message);
			GlobalEnglish.Denali.Util.Logger.WriteWarning(message);
		}

		/// <summary>
		/// Reports an error.
		/// </summary>
		private static void ReportError(Exception ex)
		{
			Logger.Error(ex.Message, ex);
			GlobalEnglish.Denali.Util.Logger.WriteError(ex.Message);
		}

	} // ConnectionExtensions


	/// <summary>
	/// Extends SqlTransaction to support enlisted command creation.
	/// </summary>
	public static class TransactionExtensions
	{
		/// <summary>
		/// Returns a new command enlisted in this transaction.
		/// </summary>
		/// <param name="transaction">this transaction</param>
		/// <returns>a SqlCommand</returns>
		public static SqlCommand CreateCommand(this SqlTransaction transaction)
		{
			SqlCommand result = transaction.Connection.CreateCommand();
			result.Transaction = transaction;
			return result;
		}

		/// <summary>
		/// Returns a new command for a stored procedure call.
		/// </summary>
		/// <param name="transaction">this transaction</param>
		/// <param name="procedureName">a stored procedure name</param>
		/// <returns>a SqlCommand</returns>
		public static SqlCommand CreateProcedure(this SqlTransaction transaction, string procedureName)
		{
			return transaction.CreateCommand().MakeProcedure(procedureName);
		}

	} // TransactionExtensions


	/// <summary>
	/// Extends SqlCommand to simplify command creation.
	/// </summary>
	public static class CommandExtensions
	{
		#region adding parameters
		/// <summary>
		/// Makes the associated command a stored procedure.
		/// </summary>
		/// <param name="command">this command</param>
		/// <param name="procedureName">a stored procedure name</param>
		/// <returns>this CommandBuilder</returns>
		public static SqlCommand MakeProcedure(this SqlCommand command, string procedureName)
		{
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = procedureName;
			return command;
		}

		/// <summary>
		/// Makes the associated command a SQL command.
		/// </summary>
		/// <param name="commandText">command text (SQL)</param>
		/// <returns>this CommandBuilder</returns>
		public static SqlCommand MakeSQL(this SqlCommand command, string commandText)
		{
			command.CommandType = CommandType.Text;
			command.CommandText = commandText;
			return command;
		}

		/// <summary>
		/// Adds a SQL integer parameter to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		/// <param name="value">a value</param>
		/// <returns>this CommandBuilder</returns>
		public static SqlCommand WithInteger(this SqlCommand command, string argumentName, int value)
		{
			command.AddInteger(argumentName, value);
			return command;
		}

		/// <summary>
		/// Adds a SQL integer parameter to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		/// <param name="value">a value</param>
		public static void AddInteger(this SqlCommand command, string argumentName, int value)
		{
			Argument.Check("argumentName", argumentName);
			command.Parameters.Add(CreateSqlInteger(argumentName, value));
		}

		/// <summary>
		/// Adds a SQL tiny integer to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		/// <param name="value">a value</param>
		/// <returns>this CommandBuilder</returns>
		public static SqlCommand WithByte(this SqlCommand command, string argumentName, int value)
		{
			command.AddByte(argumentName, value);
			return command;
		}

		/// <summary>
		/// Adds a SQL tiny integer to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		/// <param name="value">a value</param>
		public static void AddByte(this SqlCommand command, string argumentName, int value)
		{
			Argument.Check("argumentName", argumentName);
			command.Parameters.Add(CreateSqlTiny(argumentName, (byte)value));
		}

		/// <summary>
		/// Adds a SQL string to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		/// <param name="value">a value</param>
		/// <returns>this CommandBuilder</returns>
		public static SqlCommand WithString(this SqlCommand command, string argumentName, string value)
		{
			command.AddString(argumentName, value);
			return command;
		}

		/// <summary>
		/// Adds a SQL string to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		/// <param name="value">a value</param>
		public static void AddString(this SqlCommand command, string argumentName, string value)
		{
			Argument.Check("argumentName", argumentName);
			Argument.Check("value", value);
			command.Parameters.Add(CreateSqlString(argumentName, value));
		}

		/// <summary>
		/// Adds a SQL table to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		/// <param name="value">a value</param>
		/// <returns>this CommandBuilder</returns>
		/// <remarks>The supplied table must have the target DB type name as the table name.</remarks>
		public static SqlCommand WithData(this SqlCommand command, string argumentName, DataTable table)
		{
			command.AddData(argumentName, table);
			return command;
		}

		/// <summary>
		/// Adds a SQL table to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		/// <param name="value">a value</param>
		/// <remarks>The supplied table must have the target DB type name as the table name.</remarks>
		public static void AddData(this SqlCommand command, string argumentName, DataTable table)
		{
			Argument.Check("argumentName", argumentName);
			Argument.Check("table", table);
			Argument.Check("table.TableName", table.TableName);
			command.Parameters.Add(CreateSqlTable(argumentName, table));
		}

		/// <summary>
		/// Adds an output value to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		/// <returns>this CommandBuilder</returns>
		public static SqlCommand WithOutputInteger(this SqlCommand command, string argumentName)
		{
			command.AddOutputInteger(argumentName);
			return command;
		}

		/// <summary>
		/// Adds an output value to the command.
		/// </summary>
		/// <param name="argumentName">an argument name</param>
		public static void AddOutputInteger(this SqlCommand command, string argumentName)
		{
			command.Parameters.Add(CreateSqlOutputInteger(argumentName));
		}

		/// <summary>
		/// Returns the output value that results from a query.
		/// </summary>
		/// <returns>an integer value</returns>
		public static int GetOutputInteger(this SqlCommand command)
		{
			SqlParameter outputValue = command.GetOutputParameter();
			return (outputValue == null ? 0 : Convert.ToInt32(outputValue.Value));
		}

		/// <summary>
		/// Returns the first output parameter.
		/// </summary>
		/// <param name="command">this command</param>
		/// <returns>the first output parameter, or null</returns>
		public static SqlParameter GetOutputParameter(this SqlCommand command)
		{
			return command.Parameters.OfType<SqlParameter>()
					.FirstOrDefault(argument => argument.Direction == ParameterDirection.Output);
		}
		#endregion

		#region creating parameters
		private static SqlParameter CreateSqlOutputInteger(string argumentName)
		{
			SqlParameter result = CreateSqlInteger(argumentName, 0);
			result.Direction = ParameterDirection.Output;
			return result;
		}

		private static SqlParameter CreateSqlInteger(string argumentName, int value)
		{
			SqlParameter result = new SqlParameter(argumentName, SqlDbType.Int);
			result.Value = value;
			result.Size = 4;
			return result;
		}

		private static SqlParameter CreateSqlTiny(string argumentName, byte value)
		{
			SqlParameter result = new SqlParameter(argumentName, SqlDbType.TinyInt);
			result.Value = value;
			result.Size = 1;
			return result;
		}

		private static SqlParameter CreateSqlString(string argumentName, string value)
		{
			SqlParameter result = new SqlParameter(argumentName, SqlDbType.NVarChar);
			result.Value = value;
			return result;
		}

		private static SqlParameter CreateSqlTable(string argumentName, DataTable table)
		{
			SqlParameter result = new SqlParameter(argumentName, SqlDbType.Structured);
			result.TypeName = table.TableName;
			result.Value = table;
			return result;
		}
		#endregion

	} // CommandExtensions


	/// <summary>
	/// Extends IDataReader to simplify reading data.
	/// </summary>
	public static class ReaderExtensions
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(ReaderExtensions));

		/// <summary>
		/// Returns the bytes contained in the named column of the current row of this data reader.
		/// </summary>
		/// <param name="reader">this data reader</param>
		/// <param name="columnName">a column name</param>
		/// <returns>the bytes from the named column, or empty</returns>
		public static byte[] GetBytes(this IDataReader reader, string columnName)
		{
			int index = reader.GetOrdinal(columnName);
			if (reader.IsDBNull(index))
			{
				byte[] empty = { };
				return empty;
			}

			return (byte[])reader[index];
		}

		/// <summary>
		/// Returns the enumerated value from the named column in the current row of this data reader.
		/// </summary>
		/// <typeparam name="ValueType">an enumerated value type</typeparam>
		/// <param name="reader">this data reader</param>
		/// <param name="columnName">a column name</param>
		/// <returns>the value from the named column</returns>
		public static ValueType GetEnum<ValueType>(this IDataReader reader, string columnName)
		{
			return (ValueType)Enum.Parse(typeof(ValueType), reader.Get<string>(columnName));
		}

		/// <summary>
		/// Returns the value from the named column in the current row of this data reader.
		/// </summary>
		/// <typeparam name="ValueType">a value type</typeparam>
		/// <param name="reader">this data reader</param>
		/// <param name="columnName">a column name</param>
		/// <returns>the value from the named column</returns>
		public static ValueType Get<ValueType>(this IDataReader reader, string columnName)
		{
			int index = reader.GetOrdinal(columnName);
			if (reader.IsDBNull(index))
			{
				string message = "Expected non-null value for " + columnName;
				Logger.Error(message);
				throw new Exception(message);
			}

			return (ValueType)Convert.ChangeType(reader[index], typeof(ValueType));
		}

	} // ReaderExtensions
}
