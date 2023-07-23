namespace SealAPI.Resources
{
    /// <summary>
    /// Static helper class used to reduce helper methods within the controllers
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// Log detailed exception for debugging
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string LogException(Exception ex)
        {
            return "Exception Thrown: "
                 + "\n  Type:    " + ex.GetType().Name
                 + "\n  Message: " + ex.Message;
        }

        /// <summary>
        /// Resolve the file type based on it's extension
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FileType ResolveFileType(string name)
        {
            int extIndex = name.LastIndexOf('.');
            if (extIndex > 0)
            {
                string extStr = name.Substring(extIndex);
                switch(extStr.ToLower())
                {
                    case ".pdf": { return FileType.PDF; }
                    case ".xls": { return FileType.XLS; }
                    case ".xlsx": { return FileType.XLSX; }
                    case ".doc": { return FileType.DOC; }
                    case ".docx": { return FileType.DOCX; }
                    case ".txt": { return FileType.TXT; }
                    case ".png": { return FileType.PNG; }
                    case ".jpeg": { return FileType.JPEG; }
                    case ".jpg": { return FileType.JPG; }
                    default: { return FileType.UNKNOWN; }
                }
            }
            return FileType.UNKNOWN;
        }
    }

    /// <summary>
    /// Supported file extensions
    /// </summary>
    public enum FileType
    {
        UNKNOWN = -1,
        PDF,
        XLS,
        XLSX,
        DOC,
        DOCX,
        TXT,
        PNG,
        JPEG,
        JPG
    }
}
