using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Data;

namespace GlobalEnglish.Recognition.Repository
{
    public class AsrAttemptEntity : TableServiceEntity
    {
        public AsrAttemptEntity(string userId)
        {
            this.PartitionKey = userId;
            this.RowKey = Guid.NewGuid().ToString();
        }

        public AsrAttemptEntity() { }

        public int RecognitionType { get; set; }
        public int EngineId { get; set; }
        public string Grammar { get; set; }
        public int SentenceConfidence { get; set; }
        public int SentenceScore { get; set; }
        public string WordConfidenceList { get; set; }
        public string WordScoreList { get; set; }
        public string PhonemeTrackingDetail { get; set; }
    }
    
    public class AzureTableHelper
    {
        public static readonly string TableName = "AsrAttempt";
        public static readonly string ConnectionName = "AzureTable";

        public static CloudTableClient GetAzureTableClient()
        {
            string conn = ConnectionFactory.GetConnectionString(ConnectionName);
            var account = CloudStorageAccount.Parse(conn);
            return account.CreateCloudTableClient();
        }
    }
}

