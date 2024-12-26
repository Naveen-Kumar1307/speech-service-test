using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using Common.Logging;
using GlobalEnglish.Utility.Values;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Xml
{
    /// <summary>
    /// Provides access to resources (embedded or external).
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a resource path</item>
    /// <item>knows a resource name</item>
    /// <item>knows a resource length</item>
    /// <item>provides access to resource content</item>
    /// </list>
    /// </remarks>
    public class ResourceFile
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ResourceFile));

        /// <summary>
        /// An scheme for resources embedded in an assembly.
        /// </summary>
        public static readonly string AssemblyScheme = "assembly:";

        /// <summary>
        /// A resource location path separator.
        /// </summary>
        public static readonly string PathSeparator = "/";

        /// <summary>
        /// A file system folder path separator.
        /// </summary>
        public static readonly string FolderSeparator = Path.DirectorySeparatorChar.ToString();

        private String[] Segments { get; set; }

        #region locating resources
        /// <summary>
        /// Finds a target file relative to the executing assembly.
        /// </summary>
        /// <param name="targetFile">a target file path</param>
        /// <returns>a target file, or null</returns>
        public static FileInfo FindFile(string targetFile)
        {
            Argument.Check("targetFile", targetFile);
            targetFile = ConvertPath(targetFile);
            if (File.Exists(targetFile)) return new FileInfo(targetFile);

            string parentFolder = Path.GetDirectoryName(targetFile);
            DirectoryInfo baseFolder = FindFolder(parentFolder);
            if (baseFolder == null) return null;

            string foundParent = baseFolder.FullName;
            foundParent = foundParent.Substring(0, foundParent.Length - parentFolder.Length);
            return new FileInfo(BuildPath(foundParent, targetFile));
        }

        /// <summary>
        /// Finds a target folder relative to the executing assembly.
        /// </summary>
        /// <param name="targetFolder">a target folder path</param>
        /// <returns>a target folder, or null</returns>
        public static DirectoryInfo FindFolder(string targetFolder)
        {
            Argument.Check("targetFolder", targetFolder);
            string[] baseFolderPaths = 
            { 
                Environment.CurrentDirectory, 
                AppDomain.CurrentDomain.BaseDirectory
            };

            if (Logger.IsTraceEnabled)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("base folders = { '");
                builder.Append(Environment.CurrentDirectory);
                builder.Append("', '");
                builder.Append(AppDomain.CurrentDomain.BaseDirectory);
                builder.Append("' }");
                Logger.Trace(builder.ToString());
            }

            // return direct target if folder path exists or contains a volume
            targetFolder = ConvertPath(targetFolder);
            if (Directory.Exists(targetFolder) ||
                targetFolder.Contains(Path.VolumeSeparatorChar.ToString()))
                return new DirectoryInfo(targetFolder);

            // locate folder relative to a base folder (if possible)
            string topFolder = GetTopFolder(targetFolder);
            foreach (string baseFolderPath in baseFolderPaths)
            {
                DirectoryInfo baseFolder = new DirectoryInfo(baseFolderPath);
                DirectoryInfo foundFolder = FindFolder(targetFolder, baseFolder);
                if (foundFolder != null) return foundFolder;
            }

            // assume folder == target
            return new DirectoryInfo(targetFolder);
        }

        private static readonly SearchOption FindOption = SearchOption.TopDirectoryOnly;

        /// <summary>
        /// Finds a folder relative to a base folder.
        /// </summary>
        /// <param name="targetFolder">a target folder path</param>
        /// <param name="baseFolder">a base folder</param>
        /// <returns>a found folder, or null</returns>
        private static DirectoryInfo FindFolder(string targetFolder, DirectoryInfo baseFolder)
        {
            string topFolder = GetTopFolder(targetFolder);

            while (baseFolder != null)
            {
                DirectoryInfo[] folders = baseFolder.GetDirectories(topFolder, FindOption);
                if (folders.Length == 0)
                {
                    baseFolder = baseFolder.Parent;
                }
                else
                {
                    string candidatePath = BuildPath(baseFolder.FullName, targetFolder);
                    DirectoryInfo candidateFolder = new DirectoryInfo(candidatePath);
                    if (candidateFolder.Exists)
                    {
                        return candidateFolder;
                    }
                    else
                    {
                        baseFolder = baseFolder.Parent;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the top folder name from a path.
        /// </summary>
        private static string GetTopFolder(string targetFolder)
        {
            StringSplitOptions noEmpties = StringSplitOptions.RemoveEmptyEntries;
            char[] separators = { FolderSeparator[0] };
            string[] folderNames = targetFolder.Split(separators, noEmpties);
            if (folderNames.Length == 0) return null;
            return folderNames[0];
        }

        /// <summary>
        /// Builds a folder path by combining a parent path with a child path.
        /// </summary>
        private static string BuildPath(string parentPath, string childPath)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(parentPath);

            if (!childPath.StartsWith(FolderSeparator))
                builder.Append(FolderSeparator);

            builder.Append(childPath);
            return builder.ToString();
        }

        /// <summary>
        /// Returns a file system folder path.
        /// </summary>
        /// <param name="targetFolder">a folder path</param>
        /// <returns>a file system folder path</returns>
        public static string ConvertPath(string targetFolder)
        {
            return targetFolder.Replace(PathSeparator[0], FolderSeparator[0]);
        }
        #endregion

        #region creating instances
        /// <summary>
        /// Returns a new ResourceFile.
        /// </summary>
        /// <param name="fileName">a resource file name</param>
        /// <param name="sourceType">a source type</param>
        /// <returns>a new ResourceFile</returns>
        /// <remarks>
        /// The supplied <paramref name="sourceType"/> provides the name of its containing
        /// assembly and namespace, which are needed to build the full resource path.
        /// </remarks>
        public static ResourceFile From(string fileName, Type sourceType)
        {
            Argument.Check("fileName", fileName);
            Argument.Check("sourceType", sourceType);
            StringBuilder builder = new StringBuilder();
            builder.Append(AssemblyScheme);
            builder.Append(PathSeparator);
            builder.Append(PathSeparator);
            builder.Append(sourceType.Assembly.GetName().Name);
            builder.Append(PathSeparator);
            builder.Append(sourceType.Namespace);
            builder.Append(PathSeparator);
            builder.Append(fileName);
            return Named(builder.ToString());
        }

        /// <summary>
        /// Returns a new ResourceFile.
        /// </summary>
        /// <param name="resourcePath">a required resource path</param>
        /// <returns>a new ResourceFile</returns>
        public static ResourceFile Named(String resourcePath)
        {
            Argument.Check("resourcePath", resourcePath);
            if (resourcePath.StartsWith(AssemblyScheme))
            {
                String[] segments = resourcePath.Split(PathSeparator[0]);
                Argument.CheckLimit("resourcePath", segments.Length, Argument.EQUAL, 5);
                return new ResourceFile(segments);
            }
            else
            {
                String[] segments = { ConvertPath(resourcePath) };
                return new ResourceFile(segments);
            }
        }

        /// <summary>
        /// Constructs a new ResourceFile.
        /// </summary>
        /// <param name="segments">resource path segments</param>
        private ResourceFile(String[] segments)
        {
            Segments = segments;
        }
        #endregion

        #region accessing values
        /// <summary>
        /// Indicates whether the resource is located in the file system.
        /// </summary>
        public bool ReferencesFile
        {
            get { return (Segments.Length == 1); }
        }

        /// <summary>
        /// A simple resource name.
        /// </summary>
        public String SimpleName
        {
            get
            {
                if (ReferencesFile)
                {
                    return Path.GetFileName(Segments[0]);
                }
                else
                {
                    return Segments[Segments.Length - 1];
                }
            }
        }

        /// <summary>
        /// A full resource name.
        /// </summary>
        public String FullName
        {
            get
            {
                if (ReferencesFile)
                {
                    return Segments[0];
                }
                else
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (string segment in Segments)
                    {
                        if (builder.Length > 0) builder.Append(PathSeparator);
                        builder.Append(segment);
                    }
                    return builder.ToString();
                }
            }
        }

        /// <summary>
        /// A resource file type.
        /// </summary>
        public String FileType
        {
            get { return Path.GetExtension(SimpleName); }
        }

        /// <summary>
        /// Indicates whether this resource exists.
        /// </summary>
        public bool Exists
        {
            get
            {
                using (Stream stream = GetResourceStream())
                {
                    return (stream != null);
                }
            }
        }

        /// <summary>
        /// A resource length.
        /// </summary>
        public int Length
        {
            get
            {
                using (Stream stream = GetResourceStream())
                {
                    if (stream == null) return 0;
                    return (int)stream.Length;
                }
            }
        }
        #endregion

        #region loading resources
        /// <summary>
        /// Loads and decodes an embedded XML resource.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="fileName">an embedded resource file name</param>
        /// <param name="sourceType">a source type</param>
        /// <returns>a result decoded from XML</returns>
        /// <remarks>
        /// The supplied <paramref name="sourceType"/> provides the name of its containing
        /// assembly and namespace, which are needed to build the full resource path.
        /// </remarks>
        public static ResultType 
            LoadEmbedded<ResultType>(string fileName, Type sourceType) where ResultType : class
        {
            return From(fileName, sourceType).LoadResource<ResultType>();
        }

        /// <summary>
        /// Loads and decodes an XML resource.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <returns>a result decoded from XML</returns>
        public ResultType LoadResource<ResultType>() where ResultType : class
        {
            return XmlCodec<ResultType>.DecodeXml(GetResourceText());
        }

        /// <summary>
        /// Returns the content of a binary resource.
        /// </summary>
        /// <returns>the binary content, or empty</returns>
        public byte[] GetResourceBinary()
        {
            using (Stream stream = GetResourceStream())
            {
                if (stream == null) return new byte[0];
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    return reader.ReadBytes((int)stream.Length);
                }
            }
        }

        /// <summary>
        /// Returns the content of a text resource.
        /// </summary>
        /// <returns>a text string, or empty</returns>
        public String GetResourceText()
        {
            using (Stream stream = GetResourceStream())
            {
                if (stream == null) return String.Empty;
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Returns a new resource content stream.
        /// </summary>
        /// <returns>a new resource content stream, or null</returns>
        public Stream GetResourceStream()
        {
            if (ReferencesFile)
            {
                if (!File.Exists(Segments[0])) return null;
                return new FileStream(Segments[0], FileMode.Open, FileAccess.Read);
            }
            else
            {
                Assembly source = GetResourceAssembly();
                return (source == null ? null :
                        source.GetManifestResourceStream(GetEmbeddedResourceName()));
            }
        }

        /// <summary>
        /// Returns a resource assembly.
        /// </summary>
        /// <returns>a resource assembly, or null</returns>
        private Assembly GetResourceAssembly()
        {
            try
            {
                return Assembly.Load(Segments[2]);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns an embedded resource name.
        /// </summary>
        /// <returns>an embedded resource name</returns>
        private String GetEmbeddedResourceName()
        {
            if (Segments[3].Length > 0)
                return Segments[3] + "." + Segments[4];
            else
                return Segments[4];
        }
        #endregion

    } // ResourceFile
}
