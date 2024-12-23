using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

/// <summary>
/// Class Name  : clsWaveProcessor
/// By          : Sujoy G
/// Dt          : 12th Apr 2007
/// 
/// Description :-
/// 
/// The clsWaveProcessor class is intended to encapsulate basic but necessary methods for manipulating 
/// WAVE audio files programmatically. Initially the class is developed to meet basic requirements 
/// of voice mail and telephony applications. That is why the class expects input files in [16bit 8kHz Mono] format. 
/// However, with minor modification, the class can be used for other formats too.
/// 
/// The basic idea is adapted from the CodeProject article "Concatenation Wave Files using C# 2005" by Ehab Mohamed Essa,
/// URL - http://www.codeproject.com/useritems/Concatenation_Wave_Files.asp. Then modified for practical purposes.
/// 
/// The class is highly independent and doesn't require expensive 3rd party libraries to function.
/// <summary>

namespace VoiceCodec
{
    /// <summary>
    /// A wave format processor.
    /// </summary>
    public class WaveProcessor
    {
        // Constants for default or base format [16bit 8kHz Mono]
        private const short CHNL = 1;
        private const int SMPL_RATE = 16000;
        private const int BIT_PER_SMPL = 16;
        private const short FILTER_FREQ_LOW = -10000;
        private const short FILTER_FREQ_HIGH = 10000;

        // Public Fields can be used for various operations
        public int Length;
        public short Channels;
        public int SampleRate;
        public int DataLength;
        public short BitsPerSample;
        public ushort MaxAudioLevel;
        public byte[] RawPcmWaveData;

        /// <summary>
        /// Returns a new WaveProcessor.
        /// </summary>
        /// <param name="rawData">raw audio data</param>
        /// <returns>a new WaveProcessor</returns>
        public static WaveProcessor From(byte[] rawData)
        {
            using (MemoryStream stream = new MemoryStream(rawData))
            {
                //Pass the output to clsWaveProcessor
                WaveProcessor cwp = new WaveProcessor();

                //Process the header and the data
                cwp.SamplesIN(stream);

                return cwp;
            }
        }

        /// <summary>
        /// Constructs a new WaveProcessor.
        /// </summary>
        public WaveProcessor()
        {
            Length = 0;
            Channels = CHNL;
            SampleRate = SMPL_RATE;
            BitsPerSample = BIT_PER_SMPL;
        }

        /// <summary>
        /// Copies wave data from a stream.
        /// </summary>
        /// <param name="s">an audio stream</param>
        public void SamplesIN(Stream s)
        {
            RawPcmWaveData = new byte[s.Length];
            DataLength = (int)s.Length;
            Length = DataLength + 36;
            s.Read(RawPcmWaveData, 0, RawPcmWaveData.Length);
        }

        //-----------------------------------------------------
        //Methods Starts Here
        //-----------------------------------------------------
        /// <summary>
        /// Read the wave file header and store the key values in public variable.
        /// Adapted from - Concatenation Wave Files using C# 2005 by By Ehab Mohamed Essa
        /// URL - http://www.codeproject.com/useritems/Concatenation_Wave_Files.asp
        /// </summary>
        /// <param name="strPath">The physical path of wave file incl. file name for reading</param>
        /// <returns>True/False</returns>
        public bool WaveHeaderIN(Stream s)
        {

            BinaryReader br = new BinaryReader(s);
            try
            {
                Length = (int)s.Length - 8;
                s.Position = 22;
                Channels = br.ReadInt16(); //1
                s.Position = 24;
                SampleRate = br.ReadInt32(); //8000
                s.Position = 34;
                BitsPerSample = br.ReadInt16(); //16
                DataLength = (int)s.Length - 44;
                RawPcmWaveData = new byte[s.Length - 44];
                s.Position = 44;
                s.Read(RawPcmWaveData, 0, RawPcmWaveData.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                br.Close();
                s.Close();
            }
            return true;
        }

        /// <summary>
        /// Write default WAVE header to the output. See constants above for default settings.
        /// Adapted from - Concatenation Wave Files using C# 2005 by By Ehab Mohamed Essa
        /// URL - http://www.codeproject.com/useritems/Concatenation_Wave_Files.asp
        /// </summary>
        /// <param name="strPath">The physical path of wave file incl. file name for output</param>
        /// <returns>True/False</returns>
        public bool WaveHeaderOUT(string strPath)
        {
            if (strPath == null) strPath = "";
            if (strPath == "") return false;

            FileStream fs = new FileStream(strPath, FileMode.Create, FileAccess.Write);
            try
            {
                return WaveHeaderOUT(fs);
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Writes a wave file header on a stream.
        /// </summary>
        /// <param name="fs">a stream</param>
        /// <returns>whether this succeeded</returns>
        public bool WaveHeaderOUT(Stream fs)
        {
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                fs.Position = 0;

                bw.Write(new char[4] { 'R', 'I', 'F', 'F' });

                bw.Write(Length);

                bw.Write(new char[8] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });

                bw.Write((int)16);

                bw.Write((short)1);
                bw.Write(Channels);

                bw.Write(SampleRate);

                bw.Write((int)(SampleRate * ((BitsPerSample * Channels) / 8)));

                bw.Write((short)((BitsPerSample * Channels) / 8));

                bw.Write(BitsPerSample);

                bw.Write(new char[4] { 'd', 'a', 't', 'a' });
                bw.Write(DataLength);
            }
            catch
            {
                return false;
            }
            finally
            {
                //bw.Close();
            }
            return true;
        }

        public void WriteHeaderAndData(Stream fs)
        {
            WaveHeaderOUT(fs);
            fs.Write(RawPcmWaveData, 0, RawPcmWaveData.Length);
        }

        //public DecodeReturn GetDecodeResult()
        //{
        //    return new DecodeReturn(RawPcmWaveData, 
        //                            ((BitsPerSample / 8) * SampleRate),
        //                            SampleRate, Channels != 1, BitsPerSample);
        //}

    } // WaveProcessor
}
