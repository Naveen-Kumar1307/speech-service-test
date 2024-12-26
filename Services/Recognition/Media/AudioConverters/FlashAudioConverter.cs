using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using JDP;
using GlobalEnglish.Media.ServiceContracts;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Media.AudioConverters
{
    /// <summary>
    /// Converts audio data from flash format to speex format.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>converts audio data from flash format to speex format</item>
    /// </list>
    /// </remarks>
    public class FlashAudioConverter : IFormatConversionService
    {
        public static readonly string OggSuffix = ".ogg";

        /// <summary>
        /// The standard audio file type.
        /// </summary>
        public static string StandardAudioFileType =
                                ConfiguredValue.Named("StandardAudioFileType", OggSuffix);

        private static readonly string ConversionFolderPath =
                                ConfiguredValue.Named("AudioConversionFolder");

        #region IFormatConversionService Members

        /// <inheritdoc/>
        public byte[] ConvertFormat(byte[] formattedData, string fileName)
        {
            byte[] results = { }; // empty bytes

            if (Argument.IsAbsent(formattedData))
            {
                // bail out if nothing to do
                if (Argument.IsAbsent(fileName)) return results;

                // load audio data from the supplied file (path)
                formattedData = ResourceFile.Named(fileName).GetResourceBinary();

                // bail out if nothing to do
                if (formattedData.Length == 0) return results;
            }

            using (MemoryStream flashStream = new MemoryStream(formattedData))
            using (MemoryStream speexStream = new MemoryStream(formattedData.Length))
            {
                FLVFile result = new FLVFile(flashStream);
                result.ExtractStreams(speexStream, null);
                results = speexStream.ToArray();
                flashStream.Close();
                speexStream.Close();
            }

            if (ConversionFolderPath.Length > 0)
            {
                DirectoryInfo folder =
                    ResourceFile.FindFolder(ConversionFolderPath).CreateIfMissing();

                string filePath = Path.Combine(folder.FullName,
                                  Path.GetFileNameWithoutExtension(fileName) +
                                  StandardAudioFileType);

                // save a copy of the converted audio
                FileInfo resultFile = new FileInfo(filePath);
                using (FileStream stream = resultFile.OpenWrite())
                {
                    stream.Write(results, 0, results.Length);
                    stream.Flush();
                    stream.Close();
                }
            }

            return results;
        }

        #endregion

    } // FlashAudioConverter
}
