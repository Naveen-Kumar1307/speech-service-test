using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using NUnit.Framework;
using Common.Logging;
using GlobalEnglish.Denali.Metadata;
using GlobalEnglish.Denali.ContentServiceClient;
using GlobalEnglish.Denali.ContentServiceDataContract;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Denali.Metadata.Cls;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Recognition.Services
{
    /// <summary>
    /// Verifies that all known activity grammars are legit.
    /// </summary>
    [TestFixture, Ignore]
    public class ContentGrammarTestFixture
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ContentGrammarTestFixture));
        private static readonly int CourseLimit = 12;

        private static readonly string Language = "EN";
        private static readonly byte[] EmptyAudio = { };

        private ISpeechRecognitionService Recognizer = 
                EduSpeakRecognizer.InitializeWithDefaultPackage();

        private ContentServiceClient Client = new ContentServiceClient();

        /// <summary>
        /// Verifies that all activity grammars are legit.
        /// </summary>
        [Test]
        public void VerifyGrammars()
        {
            for (int courseNum = 0; courseNum < CourseLimit; courseNum++)
            {
                List<Assignment> assignments = Client.GetCourseAssignments(courseNum);
                foreach (Assignment assignment in assignments)
                {
                    Logger.Info("Course " + courseNum + " Assignment " + assignment.Id + " " + assignment.Index);
                    VerifyGrammars(assignment);
                }
            }
        }

        private void VerifyGrammars(Assignment assignment)
        {
            if (assignment.ClassId == ClassId.ReviewTest)
            {
                VerifyTestGrammars(assignment);
            }
            else
            {
                VerifyAssignmentGrammars(assignment);
            }
        }

        private void VerifyTestGrammars(Assignment assignment)
        {
            List<ReviewTestSection> testSections = Client.GetReviewTestSections(assignment.Id);
            if (!Argument.IsEmpty(testSections))
            {
                foreach (ReviewTestSection section in testSections)
                {
                    VerifyTestGrammars(section);
                }
            }
            else
            {
                ReportMissingSections(assignment);
            }
        }

        private void VerifyTestGrammars(ReviewTestSection section)
        {
            if (section.ClassId == ClassId.SpeakingTestSection)
            {
                VerifyTestGrammars(section.ObjectId,
                    Client.GetCommunicationActivity(section.ObjectId, Language));

                return;
            }
        }

        private void VerifyAssignmentGrammars(Assignment assignment)
        {
            List<Activity> activities = Client.GetAssignmentActivities(assignment.Id);
            if (!Argument.IsEmpty(activities))
            {
                foreach (Activity activity in activities)
                {
                    VerifyGrammars(activity);
                }
            }
            else
            {
                ReportMissingActivities(assignment);
            }
        }

        private void VerifyGrammars(Activity activity)
        {
            if (activity.ClassId == ClassId.PronuciationActivity)
            {
                VerifyGrammars(activity.ObjectId, 
                    Client.GetPronunciationActivity(activity.ObjectId, Language));

                return;
            }

            if (activity.ClassId == ClassId.TelephoneActivity)
            {
                VerifyGrammars(activity.ObjectId,
                    Client.GetCommunicationActivity(activity.ObjectId, Language));

                return;
            }
        }

        private void VerifyTestGrammars(int sectionID, CommunicationActivityContent content)
        {
            if (content == null) return;
            foreach (CommunicationActivityQuestion question in content.Questions)
            {
                ReportTestProgress(sectionID, question.Index);
                VerifyGrammar(sectionID, question);
            }
        }

        private void VerifyGrammars(int activityID, CommunicationActivityContent content)
        {
            if (content == null) return;
            foreach (CommunicationActivityQuestion question in content.Questions)
            {
                ReportProgress(activityID, question.Index);
                VerifyGrammar(activityID, question);
            }
        }

        private void VerifyGrammar(int activityID, CommunicationActivityQuestion question)
        {
            try
            {
                RecognitionResult result =
                    Recognizer.RecognizeSpeech(question.Grammar, "dummy.wav", EmptyAudio);

                if (result.TypeKind == ResultKind.RecognitionError)
                {
                    if (result.ResultDetailKinds.Contains(ResultDetailKind.GrammarWasRejected))
                    {
                        ReportGrammarProblem(activityID, question.Grammar);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportActivityProblem(activityID, ex);
            }
        }

        private void VerifyGrammars(int activityID, PronunciationActivityContent content)
        {
            if (content == null) return;
            foreach (PronunciationActivityQuestion question in content.Questions)
            {
                VerifyGrammar(activityID, question);
            }
        }

        private void VerifyGrammar(int activityID, PronunciationActivityQuestion question)
        {
            try
            {
                ReportProgress(activityID, question.Index);

                RecognitionResult result =
                    Recognizer.RecognizeSpeech(question.Grammar, "dummy.wav", EmptyAudio);

                if (result.TypeKind == ResultKind.RecognitionError)
                {
                    if (result.ResultDetailKinds.Contains(ResultDetailKind.GrammarWasRejected))
                    {
                        ReportGrammarProblem(activityID, question.Grammar);
                    }
                }
            }
            catch (Exception ex)
            {
                ReportActivityProblem(activityID, ex);
            }
        }

        private void ReportTestProgress(int sectionID, int questionIndex)
        {
            Console.Write(".");
            Console.Out.Flush();
            Logger.Info("Review Test Section " + sectionID + " question " + questionIndex);
        }

        private void ReportProgress(int activityID, int questionIndex)
        {
            Console.Write(".");
            Console.Out.Flush();
            Logger.Info("Activity " + activityID + " question " + questionIndex);
        }

        private void ReportMissingActivities(Assignment assignment)
        {
            Logger.Info("No activities found for assignment " + assignment.Id);
        }

        private void ReportMissingSections(Assignment assignment)
        {
            Logger.Info("No sections found for review test " + assignment.Id);
        }

        private void ReportGrammarProblem(int activityID, string grammar)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Rejected from activity ");
            builder.Append(activityID);
            builder.Append(" grammar ");
            builder.Append(grammar);
            Logger.Error(builder.ToString());
        }

        private void ReportActivityProblem(int activityID, Exception ex)
        {
            Logger.Error("Activity " + activityID + " problem " + ex.Message, ex);
        }

    } // ContentGrammarTestFixture
}
