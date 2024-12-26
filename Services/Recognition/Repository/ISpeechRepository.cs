using System;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;

using DataContracts = GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Recognition.DataContracts;

namespace GlobalEnglish.Recognition.Repository
{
    [ServiceContract]
    public interface ISpeechRepository
    {
        [OperationContract]
        void SaveAsrAttempt(int clsUserId, int engineId, DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result);

        [OperationContract]
        void SaveAsrAttemptHistory(int clsUserId, int engineId, DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result);

        [OperationContract]
        List<DataContracts.PhonemeQuality> GetRecentPhonemes(int clsUserId, int? qualifiedThreshold);
    }
}
