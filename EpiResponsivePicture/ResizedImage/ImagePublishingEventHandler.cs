using System.Drawing;
using System.IO;
using EPiServer;
using EPiServer.Core;

namespace Forte.EpiResponsivePicture.ResizedImage
{
    public class ImagePublishingEventHandler
    {
        public void CalculateDimensions(object sender, ContentEventArgs e)
        {
            var mediaData = e.Content as MediaData;
            var content = e.Content as IResponsiveImage;

            if (content == null || mediaData == null) return;

            using (var stream = ReadBlob(mediaData))
            using (var binaryData = Image.FromStream(stream))
            {
                content.Width = binaryData.Width;
                content.Height = binaryData.Height;
            }
        }

        private static MemoryStream ReadBlob(MediaData content)
        {
            // when having Azure Blob stream directly, it causes errors for some PNGs
            // throws only for some large PNGs and for Azure Blob provider
            using (var stream = content.BinaryData.OpenRead())
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                var memoryStream = new MemoryStream(buffer, false);
                return memoryStream;
            }
        }
    }
}