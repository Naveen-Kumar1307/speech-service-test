using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;

using GlobalEnglish.Utility.Values;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Xml
{
    /// <summary>
    /// Encodes and decodes item data in an XML document (or stream).
    /// </summary>
    /// <typeparam name="ItemType">the kind of data item</typeparam>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows the structure of a data item</item>
    /// <item>encodes a data item as XML</item>
    /// <item>decodes a data item from XML</item>
    /// </list>
    /// <h4>Clients must:</h4>
    /// <list type="bullet">
    /// <item>supply a stream during construction (for encoding), or</item>
    /// <item>use the convenience methods to encode and decode items</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// ItemType item = new ItemType();
    /// String xml = XmlCodec&lt;ItemType&gt;.EncodeXml(item);
    /// ItemType result = XmlCodec&lt;ItemType&gt;.DecodeXml(xml);
    /// </code>
    /// </example>
    public class XmlCodec<ItemType> where ItemType : class
    {
        private static readonly Type StoredType = typeof(ItemType);
        private static readonly Encoding XmlEncoding = new UTF8Encoding(false);
        private static readonly String StandardHeader = 
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

        private static readonly XmlQualifiedName[]
            EmptyNamespace = { new XmlQualifiedName("", "") };

        private static readonly XmlSerializerNamespaces
            Namespaces = new XmlSerializerNamespaces(EmptyNamespace);

        private Stream stream;
        private bool includeHeader = true;
        private XmlSerializer serializer = CodecCache.GetCodec<ItemType>();

        #region creating instances
        /// <summary>
        /// Constructs a new XmlCodec.
        /// </summary>
        /// <param name="stream">a stream</param>
        private XmlCodec(Stream stream)
        {
            this.stream = stream;
        }
        #endregion

        #region decoding items from XML
        /// <summary>
        /// Loads an item decoded from an XML file.
        /// </summary>
        /// <param name="file">a required file</param>
        /// <returns>an item</returns>
        public static ItemType LoadFrom(FileInfo file)
        {
            Argument.Check("file", file);
            if (!file.Exists) return null;
            using (Stream stream = file.OpenRead())
            {
                return new XmlCodec<ItemType>(stream).DecodedItem;
            }
        }

        /// <summary>
        /// Returns an item decoded from XML.
        /// </summary>
        /// <param name="xml">a required XML document</param>
        /// <returns>an item</returns>
        public static ItemType DecodeXml(String xml)
        {
            if (Argument.IsAbsent(xml)) return null;
            using (MemoryStream stream = new MemoryStream(XmlEncoding.GetBytes(xml)))
            {
                return new XmlCodec<ItemType>(stream).DecodedItem;
            }
        }

        /// <summary>
        /// The decoded item.
        /// </summary>
        private ItemType DecodedItem
        {
            get { return Serializer.Deserialize(new XmlTextReader(Stream)) as ItemType; }
        }
        #endregion

        #region encoding items as XML
        /// <summary>
        /// Stores an item as XML in a file.
        /// </summary>
        /// <param name="item">a required item</param>
        /// <param name="file">a required file</param>
        public static void StoreXml(ItemType item, FileInfo file)
        {
            Argument.Check("file", file);
            using (FileStream stream = file.Open(FileMode.Create))
            {
                XmlCodec<ItemType> codec = new XmlCodec<ItemType>(stream);
                codec.Encode(item);
                stream.Flush();
                stream.Close();
            }
        }

        /// <summary>
		/// Encodes an item as XML.
		/// </summary>
		/// <param name="item">a required item</param>
		/// <returns>an XML document</returns>
		public static String EncodeXml(ItemType item)
		{
			return EncodeXml(item, false);
		}

        /// <summary>
        /// Encodes an item as XML.
        /// </summary>
        /// <param name="item">a required item</param>
		/// <param name="includeHeader">indicates whether to include a file header</param>
        /// <returns>an XML document</returns>
        public static String EncodeXml(ItemType item, bool includeHeader)
        {
            if (item == null) return String.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                XmlCodec<ItemType> codec = new XmlCodec<ItemType>(stream);
                codec.IncludeHeader = includeHeader;
                return codec.Encode(item).EncodedXml;
            }
        }

        /// <summary>
        /// Encodes an item as XML.
        /// </summary>
        /// <param name="item">a required item</param>
        /// <returns>this codec</returns>
        private XmlCodec<ItemType> Encode(ItemType item)
        {
            CheckType(item);
            XmlTextWriter textWriter = new XmlTextWriter(Stream, Encoding.ASCII);
            Serializer.Serialize(textWriter, item, Namespaces);
            return this;
        }

        /// <summary>
        /// The encoded XML document.
        /// </summary>
        private String EncodedXml
        {
            get
            {
                String xml = XmlEncoding.GetString(MemoryStream.ToArray());
                if (xml[0] != StandardHeader[0])
                {
                    xml = xml.Substring(1); // strip off byte order marker
                }

                if (!IncludeHeader && xml.StartsWith(StandardHeader))
                {
                    xml = xml.Substring(StandardHeader.Length); // strip off XML header
                }

                return xml;
            }
        }
        #endregion

        #region accessing internals
        /// <summary>
        /// Indicates whether to include an XML file header.
        /// </summary>
        public bool IncludeHeader
        {
            get { return this.includeHeader; }
            set { this.includeHeader = value; }
        }

        /// <summary>
        /// The serializer.
        /// </summary>
        public XmlSerializer Serializer
        {
            get { return this.serializer; }
        }

        /// <summary>
        /// The stream.
        /// </summary>
        public Stream Stream
        {
            get { return this.stream; }
        }

        /// <summary>
        /// The memory stream.
        /// </summary>
        private MemoryStream MemoryStream
        {
            get { return Stream as MemoryStream; }
        }

        /// <summary>
        /// Verifies that the types match.
        /// </summary>
        /// <param name="item">an item</param>
        private void CheckType(ItemType item)
        {
            Argument.Check("item", item);
            if (!item.GetType().IsAssignableFrom(StoredType))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("This XmlCodec expects a ");
                builder.Append(StoredType.Name);
                builder.Append(" and will not properly encode a derived ");
                builder.Append(item.GetType().Name);
                throw new ArgumentOutOfRangeException(builder.ToString());
            }
        }
        #endregion

    } // XmlCodec
}
