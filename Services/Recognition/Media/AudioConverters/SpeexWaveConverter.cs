using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using VoiceCodec;
using Common.Logging;
using GlobalEnglish.Media.ServiceContracts;
using GlobalEnglish.Utility.Diagnostics;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Media.AudioConverters
{
    /// <summary>
    /// Converts audio data from speex format to wave format.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>converts audio data from speex format to wave format</item>
    /// </list>
    /// </remarks>
    public class SpeexWaveConverter : IFormatConversionService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SpeexWaveConverter));
        private static string RecognizerPort = Environment.GetEnvironmentVariable("RECPORT");

        // recycle if configured
        private static readonly bool RecycleGarbage =
                                ConfiguredValue.Get<bool>("RecycleAudioGarbage",
                                ConfiguredValue.Get<bool>("RecycleGarbage", false));

        private static readonly string AudioFolderPath = 
                                ConfiguredValue.Named("AudioConversionFolder", "/SiteLogs/audio");

        private static readonly string FlashSuffix = ".spx";
        private static readonly string SampleSuffix = ".pcm.wav";

        static SpeexWaveConverter()
        {
            string folderPath = ResourceFile.ConvertPath(AudioFolderPath);
            DirectoryInfo audioFolder = new DirectoryInfo(folderPath).CreateIfMissing();
            Logger.Debug("using audio folder " + folderPath);
        }

        #region IFormatConversionService Members

        /// <inheritdoc/>
        public byte[] ConvertFormat(byte[] formattedData, string fileName)
        {
            string folderPath = ResourceFile.ConvertPath(AudioFolderPath);
            string uniqueName = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(fileName));

            string speexFileName = uniqueName + FlashSuffix;
            string sampleFileName = uniqueName + SampleSuffix;

            FileInfo speexFile = new FileInfo(speexFileName);
            FileInfo sampleFile = new FileInfo(sampleFileName);

            using (FileStream stream = speexFile.OpenWrite())
            {
                stream.Write(formattedData, 0, formattedData.Length);
                stream.Flush();
            }

            ProcessStartInfo procStart = new ProcessStartInfo();
            procStart.FileName = "speexdec.exe";
            //procStart.Arguments = "--force-wb  - - "; // use std in + std out
            procStart.Arguments = "--force-wb  " + speexFileName + " " + sampleFileName;
            procStart.ErrorDialog = false;
            procStart.UseShellExecute = false;
            procStart.RedirectStandardInput = false; // true;
            procStart.RedirectStandardOutput = false; // true;
            procStart.RedirectStandardError = false;
            procStart.CreateNoWindow = true;

            Process decProc = new Process();
            decProc.StartInfo = procStart;
            decProc.Start();

            //decProc.StandardInput.AutoFlush = true;
            //decProc.StandardInput.BaseStream.Write(formattedData, 0, formattedData.Length);
            //decProc.StandardInput.BaseStream.Close();
            //byte[] data = ReadStandardOutput(decProc);

            decProc.WaitForExit();

            //using (MemoryStream stream = new MemoryStream(data.Length + 50))
            //{
            //    WaveProcessor.From(data).WriteHeaderAndData(stream);
            //    return stream.ToArray();
            //}

            try
            {
                return ResourceFile.Named(sampleFileName).GetResourceBinary();
            }
            finally
            {
                DisposeGarbage(speexFile, sampleFile);
            }
        }

        /// <summary>
        /// Removes the intermediate files (if configured to do so).
        /// </summary>
        private void DisposeGarbage(FileInfo speexFile, FileInfo sampleFile)
        {
            if (RecycleGarbage)
            {
                if (speexFile.Exists) speexFile.Delete();
                if (sampleFile.Exists) sampleFile.Delete();

                string simpleName = Path.GetFileNameWithoutExtension(speexFile.Name);
                string filePath = AudioFolderPath + @"\" + simpleName + ".*";
                Logger.Debug("deleted audio files from " + filePath);
            }
        }

        /// <summary>
        /// Reads standard output in its entirety from a process.
        /// </summary>
        /// <param name="p">a process</param>
        /// <returns></returns>
        private static byte[] ReadStandardOutput(Process p)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                int sample = 0;

                do
                {
                    sample = p.StandardOutput.BaseStream.ReadByte();
                    if (sample > -1) stream.WriteByte((byte)sample);
                }
                while (sample > -1);

                return stream.ToArray();
            }
        }

        #endregion

    } // SpeexWaveConverter
}
