using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Sirius2Ch.Data;
using Sirius2Ch.Models;
using Image = Sirius2Ch.Models.Image;

namespace Sirius2Ch
{
    public static class ImageUploader
    {
        private const int previewSize = 256;
        
        private static ImageCodecInfo GetEncoder(ImageFormat format)  
        {  
            var codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid);
        }

        private static Size PreviewSize(Size size)
        {
            var d = Math.Max(size.Width, size.Height) / (double)previewSize;
            if (d <= 1.0)
                return size;
            return new Size(Math.Min(previewSize, (int)(size.Width / d)), Math.Min(previewSize, (int)(size.Height / d)));
        }
        
        private static (string, string) ResizeImage(string path)
        {
            string fullName = Path.GetTempFileName(), previewName = Path.GetTempFileName();
            try
            {
                using (var bm = new Bitmap(path))
                {
                    var codecInfo = GetEncoder(ImageFormat.Jpeg);
                    var encoder = Encoder.Quality;
                    var @params = new EncoderParameters(1);
                    @params.Param[0] = new EncoderParameter(encoder, 50L);
                    
                    bm.Save(fullName, codecInfo, @params);
                    using (var pbm = new Bitmap(bm, PreviewSize(bm.Size)))
                        pbm.Save(previewName, codecInfo, @params);
                }

                return (fullName, previewName);
            }
            catch (Exception)
            {
                File.Delete(fullName);
                File.Delete(previewName);
                throw;
            }
        }
        
        public static Image UploadImage(this ApplicationDbContext db, Stream imageStream, string name)
        {
            string fname = null, full = null, prev = null;
            try
            {
                fname = Path.GetTempFileName();
                using (var f = File.OpenWrite(fname))
                    imageStream.CopyTo(f);

                (full, prev) = ResizeImage(fname);
                var image = new Image
                {
                    ContentType = "image/jpeg",
                    MaxRes = File.ReadAllBytes(full),
                    Preview256 = File.ReadAllBytes(prev),
                    Name = name,
                };
                var id = db.Images.Add(image);
                
                return image;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (fname != null)
                    File.Delete(fname);
                if (full != null)
                    File.Delete(full);
                if (prev != null)
                    File.Delete(prev);
            }
        }
    }
}