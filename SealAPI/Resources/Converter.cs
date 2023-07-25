using Aspose.Words;
using Aspose.Words.Saving;
using System.Text;

namespace SealAPI.Resources
{
    public static class Converter
    {
        /// <summary>
        /// Convert the data from the stored type to a different type that allows proper conversion
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="inputData"></param>
        /// <returns></returns>
        private static byte[] GetData(ILogger logger, byte[] inputData)
        {
            try
            {
                // Get decoded data into a Base64 string 
                string base64string = Encoding.UTF8.GetString(inputData);

                // Remove trailing quotations and prefix
                string formatBase64 = base64string.Split(",")[1].Replace("\"", "");

                // Return the new byte array
                return Convert.FromBase64String(formatBase64);
            }
            catch (Exception ex)
            {
                logger.LogError("Error: could not convert byte[] arrays for HTML conversion (Exception: {0})", ex.GetType());
            }
            return new byte[0];
        }

        /// <summary>
        /// Convert the data from it's new type back to stored type
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="inputData"></param>
        /// <returns></returns>
        private static byte[] GetData(ILogger logger, byte[] inputData, string prefix)
        {
            try
            {
                // Convert to base64 string
                string base64string = Convert.ToBase64String(inputData);

                // Add prefix
                string fomrmatBase64 = prefix + base64string;

                // Return the new byte array
                return Encoding.UTF8.GetBytes(fomrmatBase64);
            }
            catch (Exception ex)
            {
                logger.LogError("Error: could not convert byte[] arrays back to stored type (Exception: {0})", ex.GetType());
            }
            return new byte[0];
        }

        /// <summary>
        /// Used to convert Excel documents to HTML. This is because there are rendering issues with the UI npm package
        /// when trying to render .xls and .xlxs files
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ConvertExcelToPng(ILogger logger, byte[] data)
        {
            try
            {
                var newData = GetData(logger, data);

                // Create an outer stream as we cannot expand the inner stream size for saving
                using (MemoryStream savingStream = new MemoryStream(newData.Length * 2))
                {
                    using (MemoryStream memoryStream = new MemoryStream(newData))
                    {
                        // Load the Excel file using Aspose.Cells
                        Aspose.Cells.Workbook workbook = new Aspose.Cells.Workbook(memoryStream);

                        // Save the document as HTML
                        workbook.Save(savingStream, Aspose.Cells.SaveFormat.Png);

                        // Get the byte array from the MemoryStream
                        return GetData(logger, savingStream.ToArray(), "data:image/png;base64,");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error: could not convert excel file to HTML (Exception: {0})", ex.GetType());
            }
            return new byte[0];
        }

        /// <summary>
        /// Used to convert word documents to PNG. This is because there are rendering issues with the UI npm package
        /// when trying to render .doc and .docx files
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] ConvertDocToPng(ILogger logger, byte[] data)
        {
            try
            {
                var newData = GetData(logger, data);

                // Create an outer stream as we cannot expand the inner stream size for saving
                using (MemoryStream savingStream = new MemoryStream(newData.Length * 2))
                {
                    using (MemoryStream memoryStream = new MemoryStream(newData))
                    {
                        // load document
                        Document doc = new Document(memoryStream);

                        // set output image format using SaveFormat
                        var options = new Aspose.Words.Saving.ImageSaveOptions(Aspose.Words.SaveFormat.Png);

                        // We only need to preview the first page, set to first page and
                        // ignore the rest
                        options.PageSet = new PageSet(0);

                        // Save as PNG
                        doc.Save(savingStream, options);
                        return GetData(logger, savingStream.ToArray(), "data:image/png;base64,");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error: could not convert document file to PNG (Exception: {0})", ex.GetType());
            }
            return new byte[0];
        }
    }
}
