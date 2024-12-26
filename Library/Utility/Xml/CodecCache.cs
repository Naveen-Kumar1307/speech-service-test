using System;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;

namespace GlobalEnglish.Utility.Xml
{
    /// <summary>
    /// Caches codecs to prevent their regeneration.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows each cached codec by type name</item>
    /// <item>registers a new codec when a requested one is missing</item>
    /// </list>
    /// </remarks>
    public class CodecCache
    {
        #region accessing codecs
        /// <summary>
        /// Returns a codec for a given kind of object.
        /// </summary>
        /// <typeparam name="SerializedType">a serialized type</typeparam>
        /// <returns>an appropriate codec</returns>
        public static XmlSerializer GetCodec<SerializedType>()
        {
            Type serializedType = typeof(SerializedType);
            if (!GetCache().Contains(serializedType))
            {
                GetCache().Register(serializedType);
            }

            return GetCache()[serializedType];
        }
        #endregion

        #region creating instances
        /// <summary>
        /// Constructs a new CodecCache.
        /// </summary>
        private CodecCache()
        {
            Contents = new Dictionary<String, XmlSerializer>();
        }
        #endregion

        #region caching codecs
        /// <summary>
        /// The sole instance of this class.
        /// </summary>
        private static CodecCache Instance = null;

        /// <summary>
        /// The sole instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static CodecCache GetCache()
        {
            if (Instance == null) Instance = new CodecCache();

            return Instance;
        }

        /// <summary>
        /// A map from full type name to codec.
        /// </summary>
        private Dictionary<String, XmlSerializer> Contents { get; set; }

        /// <summary>
        /// Returns a codec given its type.
        /// </summary>
        /// <param name="serializedType">a serialized type</param>
        /// <returns>a codec</returns>
        private XmlSerializer this[Type serializedType]
        {
            get { return Contents[serializedType.FullName]; }
            set { Contents[serializedType.FullName] = value; }
        }

        /// <summary>
        /// Indicates whether this cache contains a codec for a given type.
        /// </summary>
        /// <param name="serializedType">a serialized type</param>
        /// <returns>whether this cache contains a codec for a given type</returns>
        private bool Contains(Type serializedType)
        {
            return Contents.ContainsKey(serializedType.FullName);
        }

        /// <summary>
        /// Adds a new codec for a given type.
        /// </summary>
        /// <param name="serializedType">a serialized type</param>
        private void Register(Type serializedType)
        {
            this[serializedType] = new XmlSerializer(serializedType);
        }
        #endregion

    } // CodecCache
}
