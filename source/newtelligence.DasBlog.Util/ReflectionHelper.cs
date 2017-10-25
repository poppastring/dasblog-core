using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace newtelligence.DasBlog.Util
{
    public static class ReflectionHelper
    {
        //************************************************************
        //  Method Name: CodeBase
        /// <summary>
        /// Returns the code base file location as a standard directory.
        /// </summary>
        /// <returns>This assemblies code base as a local file system path.</returns>
        /// <creationDate>March 15, 2002</creationDate>
        //************************************************************
        public static string CodeBase()
        {
            Assembly currentAssembly = null;
            StackTrace stackTrace = new StackTrace();
            StackFrame currentFrame;

            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                currentFrame = stackTrace.GetFrame(i);
                currentAssembly = currentFrame.GetMethod().DeclaringType.Assembly;
                if (!currentAssembly.GlobalAssemblyCache)
                {
                    break;
                }
            }

            string[] codeBaseSplit = currentAssembly.CodeBase.Split('/');
            StringBuilder fileSpec = new StringBuilder();

            for (int split = 3; split < codeBaseSplit.Length - 1; split++)
            {
                fileSpec.Append(codeBaseSplit[split] + "\\");
            }

            return fileSpec.ToString();
        }
    }
}