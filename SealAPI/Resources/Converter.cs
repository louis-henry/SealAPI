using Aspose.Pdf;
using Aspose.Pdf.Devices;
using System.Text;

namespace SealAPI.Resources
{
    public static class Converter
    {
        private static string AddPrefix()
        {
            return "data:image/png;base64,";
        }
        public static byte[] ConvertPdfToPng(byte[] pdfBytes)
        {
            // Create PDF document
            Document pdfDocument = new Document();
            pdfDocument.Pages.Add();
            pdfDocument.Save(new MemoryStream(pdfBytes));

            // Create a list to store the output images
            List<byte[]> images = new List<byte[]>();

            // We only need the first page content
            if (pdfDocument.Pages.Count > 0)
            {
                var firstPage = pdfDocument.Pages.First();

                // Create a byte array stream for the output image
                MemoryStream imageStream = new MemoryStream();

                // Create Resolution object
                Resolution resolution = new Resolution(300);

                // Create Png device with specified attributes
                // Width, Height, Resolution
                PngDevice PngDevice = new PngDevice(2480, 3508, resolution);

                // Convert a particular page and save the image to stream
                PngDevice.Process(firstPage, imageStream);

                // Get the image data from the stream
                byte[] imageData = imageStream.ToArray();

                // Close the stream
                imageStream.Close();
                string base64Text = Convert.ToBase64String(imageData);

                return Encoding.UTF8.GetBytes(AddPrefix() + base64Text);
            }
            return new byte[0];
        }
    }
}
