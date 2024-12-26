using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.WindowsAzure.TransientFaultHandling.SqlAzure;
using Microsoft.Practices.TransientFaultHandling;
using GlobalEnglish.Denali.Util;

namespace GlobalEnglish.Recognition.Repository
{
	/// <summary>
	/// A centralized place to create Azure Sql Retry Policy object
	/// </summary>
	public class GEAzureSqlRetryPolicy : RetryPolicy<SqlAzureTransientErrorDetectionStrategy>
	{
		// GUID to keep track of a series of retries from the same source. 
		private Guid _id;

		/// <summary>
		/// Constructor. Include logging for retry. 
		/// </summary>
		public GEAzureSqlRetryPolicy()
			: base(3, TimeSpan.FromSeconds(7))
		{
			_id = Guid.NewGuid();
			this.Retrying += (sender, args) =>
			{
				// Log details of the retry.
				string message = String.Format("Sql Azure Retry - GUID:{0}, Time:{1}, Count:{2}, Delay:{3}, Exception:{4}",
					_id, DateTime.Now.ToLongTimeString(), args.CurrentRetryCount, args.Delay, args.LastException);
				Logger.WriteWarning(message);
			};
		}
	}
}
