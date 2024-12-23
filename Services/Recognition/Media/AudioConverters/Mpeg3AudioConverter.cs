using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Common.Logging;
using GlobalEnglish.Media.ServiceContracts;
using GlobalEnglish.Utility.Diagnostics;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Media.AudioConverters
{
    /// <summary>
    /// Converts audio data from some unknown format to PCM format.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>converts audio data from some unknown format to PCM format</item>
    /// </list>
    /// </remarks>
    public class Mpeg3AudioConverter : IFormatConversionService
    {
        private static readonly string Blank = " ";
        private static readonly string Dash = "-";
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Mpeg3AudioConverter));
        private static string RecognizerPort = Environment.GetEnvironmentVariable("RECPORT");

        private static readonly int RiffHeaderLength = 8;
        private static readonly int DataHeaderLength = 46;

        // recycle if configured
        private static readonly bool RecycleGarbage = 
                                ConfiguredValue.Get<bool>("RecycleAudioGarbage",
                                ConfiguredValue.Get<bool>("RecycleGarbage", false));

        private static readonly string AudioFolderPath = 
                                ConfiguredValue.Named("AudioConversionFolder", "/SiteLogs/audio");

        private static readonly string RecordFolderPath =
                                ConfiguredValue.Named("AudioRecordFolder", AudioFolderPath);

        private static readonly int ConversionTimeout =
                                ConfiguredValue.Get<int>("ConversionProcessTimeout", 0) * 1000;

        // standard file type for unknown format (likely ogg/vorbis or mpeg3)
        private static readonly string SpeechSuffix = ".wav";
        private static readonly string SampleSuffix = ".pcm.wav";

        // FFmpeg converter command line options
        private static readonly string Arguments = "-i {0} -acodec pcm_s16le -ac 1 -ar 16000 -f wav {1} ";

        // default server converter process location
        private static readonly string ProcessPathName = "AudioConverterProcess";
        private static readonly string ConverterProcess = "/Server/ffmpeg.exe";
        private static string ConverterPath = string.Empty;

        static Mpeg3AudioConverter()
        {
            string recordPath = ResourceFile.ConvertPath(RecordFolderPath);
            DirectoryInfo recordFolder = new DirectoryInfo(recordPath).CreateIfMissing();
            Logger.Debug("using audio recording folder " + recordFolder);

            string folderPath = ResourceFile.ConvertPath(AudioFolderPath);
            DirectoryInfo audioFolder = new DirectoryInfo(folderPath).CreateIfMissing();
            Logger.Debug("using audio conversion folder " + folderPath);

            string Process = ConfiguredValue.Named(ProcessPathName, ConverterProcess);
            ConverterPath = ResourceFile.FindFile(Process).FullName;
            Logger.Debug("using audio converter " + ConverterPath);
        }

        public string FileType { get; set; }

        public Mpeg3AudioConverter()
        {
            FileType = SpeechSuffix;
        }

        #region IFormatConversionService Members

        /// <inheritdoc/>
        public byte[] ConvertFormat(byte[] formattedData, string fileName)
        {
            byte[] empty = { };
            string recordPath = ResourceFile.ConvertPath(RecordFolderPath);
            string folderPath = ResourceFile.ConvertPath(AudioFolderPath);
            string resampleName = 
                Path.GetFileNameWithoutExtension(fileName).Replace(Blank[0], Dash[0]);

            string speechFileName = Path.Combine(recordPath, resampleName + SpeechSuffix);
            string sampleFileName = Path.Combine(folderPath, resampleName + SampleSuffix);
            string logFileName = Path.Combine(folderPath, resampleName + ".log");

            FileInfo speechFile = new FileInfo(speechFileName);
            FileInfo sampleFile = new FileInfo(sampleFileName);
            FileInfo logFile = new FileInfo(logFileName);

            string speechPath = Dash; // speechFile.FullName;
            string samplePath = Dash; // sampleFile.FullName;

            ProcessStartInfo procStart = new ProcessStartInfo();
            procStart.FileName = ConverterPath;
            procStart.Arguments = string.Format(Arguments, speechPath, samplePath);
            procStart.ErrorDialog = false;
            procStart.UseShellExecute = false;
            procStart.RedirectStandardInput = (speechPath == Dash);
            procStart.RedirectStandardOutput = (samplePath == Dash);
            procStart.RedirectStandardError = true;
            procStart.CreateNoWindow = true;

            using (StreamWriter log = new StreamWriter(logFile.OpenWrite()))
            {
                log.AutoFlush = true;
                log.WriteLine("ffmpeg " + procStart.Arguments);

                // launch FFmpeg and capture the conversion results
                using (MemoryStream results = new MemoryStream())
                using (Process decProc = Process.Start(procStart))
                {
                    try
                    {
                        decProc.Refresh();

                        if (procStart.RedirectStandardOutput)
                        {
                            // capture FFmpeg stdout
                            object[] arguments = { decProc, results };
                            WorkerThread.SpawnBackground(arguments, CaptureStandardOutputData);
                        }

                        if (procStart.RedirectStandardInput)
                        {
                            // write the incoming data to FFmpeg stdin
                            using (BinaryWriter writer =
                                new BinaryWriter(decProc.StandardInput.BaseStream))
                            {
                                writer.Write(formattedData, 0, formattedData.Length);
                                writer.Flush();
                            }
                        }

                        decProc.WaitForExit();
                        decProc.Refresh();

                        if (procStart.RedirectStandardError)
                        {
                            // log FFmpeg stderr results
                            using (StreamReader errors = decProc.StandardError)
                            {
                                log.Write(errors.ReadToEnd());
                            }
                        }

                        if (procStart.RedirectStandardOutput)
                        {
                            return GetConvertedData(results);
                        }
                        else
                        {
                            return ResourceFile.Named(sampleFileName).GetResourceBinary();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Speech conversion failed", ex);
                    }
                    finally
                    {
                        decProc.Close();
                        log.Close();

                        DisposeGarbage(sampleFile);
                        DisposeGarbage(logFile);
                    }
                }
            }

            return empty;
        }

        /// <summary>
        /// Captures standard output data from the decoding process.
        /// </summary>
        void CaptureStandardOutputData(object argumentHolder)
        {
            object[] arguments = argumentHolder as object[];
            Process decProc = arguments[0] as Process;
            MemoryStream stream = arguments[1] as MemoryStream;
            using (Stream reader = decProc.StandardOutput.BaseStream)
            {
                int bytesRead = 0;
                byte[] buffer = new byte[4096];
                do
                {
                    bytesRead = reader.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        stream.Write(buffer, 0, bytesRead);
                    }
                }
                while (bytesRead > 0);
                stream.Flush();
            }
        }

        /// <summary>
        /// Returns the converted audio data.
        /// </summary>
        private byte[] GetConvertedData(MemoryStream results)
        {
            using (MemoryStream output = new MemoryStream())
            {
                byte[] sampleBuffer = results.ToArray();
                int bufferLength = sampleBuffer.Length;
                byte[] riffLength = BitConverter.GetBytes(bufferLength - RiffHeaderLength);
                byte[] dataLength = BitConverter.GetBytes(bufferLength - DataHeaderLength);
                int headerLength = DataHeaderLength - (RiffHeaderLength + dataLength.Length);
                output.Write(sampleBuffer, 0, RiffHeaderLength - riffLength.Length);
                output.Write(riffLength, 0, riffLength.Length);
                output.Write(sampleBuffer, RiffHeaderLength, headerLength);
                output.Write(dataLength, 0, dataLength.Length);
                output.Write(sampleBuffer, DataHeaderLength, bufferLength - DataHeaderLength);
                output.Flush();

                return output.ToArray();
            }
        }

        /// <summary>
        /// Removes the intermediate files (if configured to do so).
        /// </summary>
        private void DisposeGarbage(FileInfo sampleFile)
        {
            if (RecycleGarbage)
            {
                if (sampleFile.Exists)
                {
                    sampleFile.Delete();
                }
            }
        }

        #endregion

    } // Mpeg3AudioConverter
}
