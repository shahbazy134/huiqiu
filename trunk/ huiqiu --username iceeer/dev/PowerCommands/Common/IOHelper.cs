/***************************************************************************

Copyright (c) 2008 Microsoft Corporation. All rights reserved.

***************************************************************************/

using System.IO;
using System.Linq;

namespace Microsoft.PowerCommands.Common
{
    /// <summary>
    /// Helper class for IO operations
    /// </summary>
    public static class IOHelper
    {
        /// <summary>
        /// Sanitizes the path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string SanitizePath(string path)
        {
            string sanitizedPath = path;
            if(sanitizedPath.IndexOf(" ") > -1)
            {
                sanitizedPath = string.Concat("\"", sanitizedPath, "\"");
            }

            return sanitizedPath;
        }

        /// <summary>
        /// Gets the file without extension.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string GetFileWithoutExtension(string fileName)
        {
            if(!string.IsNullOrEmpty(fileName))
            {
                string[] fileNameParts = Path.GetFileName(fileName).Split('.');

                if(fileNameParts.Count() > 0)
                {
                    return fileNameParts[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public static string GetFileExtension(string fileName)
        {
            if(!string.IsNullOrEmpty(fileName))
            {
                string[] fileNameParts = Path.GetFileName(fileName).Split('.');

                if(fileNameParts.Count() > 0)
                {
                    return string.Concat(".", string.Join(".", fileNameParts, 1, (fileNameParts.Count() - 1)));
                }
            }

            return null;
        }
    }
}