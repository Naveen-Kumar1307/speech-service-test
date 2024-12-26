using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Spring.Context;
using Spring.Context.Support;
using GlobalEnglish.Utility.Parameters;
using System.Runtime.CompilerServices;

namespace GlobalEnglish.Utility.Spring
{
    /// <summary>
    /// Provides access to entities configured within a Spring context.
    /// </summary>
    public class SpringContext
    {
        private static readonly string StandardContextFile = "SpringContext.xml";
        private static readonly string AssemblyProtocol = "assembly://";
        private static readonly string FileProtocol = "file://";
        private static readonly string PathSeparator = "/";

        /// <summary>
        /// A cache of the previously loaded contexts.
        /// </summary>
        private static Dictionary<string, IApplicationContext> LoadedContexts = null;

        /// <summary>
        /// Returns the context cache.
        /// </summary>
        private static Dictionary<string, IApplicationContext> ContextCache
        {
            get
            {
                if (LoadedContexts == null)
                    LoadedContexts = new Dictionary<string, IApplicationContext>();

                return LoadedContexts;
            }
        }

        #region accessing configured instances
        /// <summary>
        /// Returns a configured entity whose name matches its type name.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <returns>an entity of the given result type, or null</returns>
        public static ResultType GetConfigured<ResultType>() where ResultType : class
        {
            Type resultType = typeof(ResultType);
            return GetConfigured<ResultType>(resultType.Name);
        }

        /// <summary>
        /// Returns a configured entity of a specific kind.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="resultName">an entity name</param>
        /// <returns>a configured entity, or null</returns>
        public static ResultType GetConfigured<ResultType>(string resultName) where ResultType : class
        {
            Argument.Check("resultName", resultName);
            IApplicationContext context = GetContext();
            if (context == null) return null;
            return context.GetObject(resultName) as ResultType;
        }

        /// <summary>
        /// Indicates whether the configured context contains a known entity.
        /// </summary>
        /// <param name="resultName">an entity name</param>
        /// <returns>whether the configured context contains a known entity</returns>
        public static bool HasConfigured(string resultName)
        {
            Argument.Check("resultName", resultName);
            IApplicationContext context = GetContext();
            if (context == null) return false;
            return context.ContainsObject(resultName);
        }

        /// <summary>
        /// Returns the configured Spring context.
        /// </summary>
        /// <returns>a Spring context, or null</returns>
        public static IApplicationContext GetContext()
        {
            try
            {
                if (File.Exists(StandardContextFile))
                {
                    return new XmlApplicationContext(FileProtocol + StandardContextFile);
                }
                else
                {
                    IApplicationContext result = ContextRegistry.GetContext();
                    return result;
                }
            }
            catch (Exception ex)
            {
                ReportLoadProblem(ex);
                return null;
            }
        }
        #endregion

        #region creating instances
        /// <summary>
        /// Returns a new SpringContext.
        /// </summary>
        /// <returns>a new SpringContext</returns>
        public static SpringContext FromStandardContextFile()
        {
            IApplicationContext context = GetContext();
            if (context == null) return null;

            SpringContext result = new SpringContext();
            result.Context = context;
            return result;
        }

        /// <summary>
        /// Returns a new SpringContext.
        /// </summary>
        /// <param name="resourceOwner">a required resource owner</param>
        /// <returns>a new SpringContext</returns>
        public static SpringContext From(Type resourceOwner)
        {
            Argument.Check("resourceOwner", resourceOwner);
            return From(resourceOwner, StandardContextFile);
        }

        /// <summary>
        /// Returns a new SpringContext.
        /// </summary>
        /// <param name="resourceOwner">a required resource owner</param>
        /// <param name="resourceName">a required resource name</param>
        /// <returns>a new SpringContext</returns>
        public static SpringContext From(Type resourceOwner, String resourceName)
        {
            Argument.Check("resourceOwner", resourceOwner);
            Argument.Check("resourceName", resourceName);
            SpringContext result = new SpringContext();
            result.Context = GetContext(resourceOwner, resourceName);
            return result;
        }

        /// <summary>
        /// Returns a new context.
        /// </summary>
        /// <param name="resourceOwner">a resource owner</param>
        /// <param name="resourceName">a resource name</param>
        /// <returns>a new context</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static IApplicationContext GetContext(Type resourceOwner, String resourceName)
        {
            string contextPath = BuildContextPath(resourceOwner, resourceName);
            if (ContextCache.ContainsKey(contextPath)) return ContextCache[contextPath];
            IApplicationContext result = new XmlApplicationContext(contextPath);
            ContextCache[contextPath] = result;
            return result;
        }

        /// <summary>
        /// Builds a resource path to a context embedded in an assembly.
        /// </summary>
        /// <param name="resourceOwner">a resource owner</param>
        /// <param name="resourceName">a resource name</param>
        /// <returns>an assembly context path</returns>
        private static string BuildContextPath(Type resourceOwner, String resourceName)
        {
            String assemblyName = resourceOwner.Assembly.GetName().Name;
            String nameSpace = resourceOwner.Namespace;

            StringBuilder builder = new StringBuilder();
            builder.Append(AssemblyProtocol);
            builder.Append(assemblyName);
            builder.Append(PathSeparator);
            builder.Append(nameSpace);
            builder.Append(PathSeparator);

            if (resourceName.StartsWith(nameSpace))
                builder.Append(resourceName.Substring(nameSpace.Length + 1));
            else
                builder.Append(resourceName);
            
            return builder.ToString();
        }

        private SpringContext() { }
        #endregion

        #region accessing instances
        /// <summary>
        /// Returns a configured entity whose name matches its type name.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <returns>an entity of the given result type, or null</returns>
        public ResultType Get<ResultType>() where ResultType : class
        {
            Type resultType = typeof(ResultType);
            return Get<ResultType>(resultType.Name);
        }

        /// <summary>
        /// Returns a configured entity of a specific kind.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="resultName">an entity name</param>
        /// <returns>a configured entity, or null</returns>
        public ResultType Get<ResultType>(string resultName) where ResultType : class
        {
            Argument.Check("resultName", resultName);
            return Context.GetObject(resultName) as ResultType;
        }

        /// <summary>
        /// The application context.
        /// </summary>
        public IApplicationContext Context { get; private set; }
        #endregion

        #region reporting problems
        private static void ReportLoadProblem(Exception ex)
        {
            Console.WriteLine("Spring context not found. Have you configured common/logging?");
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
        #endregion

    } // SpringContext
}
