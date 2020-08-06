using Cross.Abstractions;
using Cross.Abstractions.EntityEnums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Cross
{
    public class FileService : IFileService
    {
        private IHostingEnvironment _env;

        public FileService(IHostingEnvironment env)
        {
            _env = env;
        }

        public bool FileTypeValidator(IFormFile file, FileTypes fileType)
        {
            if (file == null || file.Length == 0) return false;
            var extention = Path.GetExtension(file.FileName);
            switch (fileType)
            {
                case FileTypes.Photo:
                    if (extention == ".jpg" || extention == ".png" || extention == ".JPG" || extention == ".PNG") return true;
                    return false;
            }
            return false;
        }

        public IFormFile PhotoResizer(IFormFile file)
        {
            try
            {
                Bitmap originalBitmap = new Bitmap(file.OpenReadStream());
                Image originalImage = originalBitmap;

                int newWidth = 200;
                float tempHeight = originalImage.Size.Width / originalImage.Size.Height;
                int newHeight = Convert.ToInt32(newWidth / tempHeight);

                Bitmap newBitmap = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(newBitmap);
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                g.Dispose();

                Stream outputStream = new MemoryStream();
                Image newImage = newBitmap;


                try
                {
                    if (Path.GetExtension(file.FileName) == ".jpg" || Path.GetExtension(file.FileName) == ".JPG") newImage.Save(outputStream, ImageFormat.Jpeg);
                    else if (Path.GetExtension(file.FileName) == ".png" || Path.GetExtension(file.FileName) == ".PNG") newImage.Save(outputStream, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw ex;
                }

                outputStream.Seek(0, SeekOrigin.Begin);
                return new FormFile(outputStream, 0, outputStream.Length, file.Name, file.FileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw ex;
            }
        }

        public string SaveFile(IFormFile file, FileTypes fileType)
        {
            var typePath = "";
            switch (fileType)
            {
                case FileTypes.Photo:
                    typePath = "Uploads/Images";
                    break;
            }
            if (typePath == null)
            {
                return null;
            }
            var profilePhotoPath = Path.Combine(_env.ContentRootPath, typePath);
            Directory.CreateDirectory(profilePhotoPath);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(profilePhotoPath, fileName);

            SaveImage(file, filePath, fileName);

            return fileName;
        }

        public string SizeDeterminator(long bytes)
        {
            var fileSize = new decimal(bytes);
            var kilobyte = new decimal(1024);
            var megabyte = new decimal(1024 * 1024);
            var gigabyte = new decimal(1024 * 1024 * 1024);

            switch (fileSize)
            {
                case var _ when fileSize < kilobyte:
                    return $"Less then 1KB";
                case var _ when fileSize < megabyte:
                    return $"{Math.Round(fileSize / kilobyte, 0, MidpointRounding.AwayFromZero):##,###.##}KB";
                case var _ when fileSize < gigabyte:
                    return $"{Math.Round(fileSize / megabyte, 2, MidpointRounding.AwayFromZero):##,###.##}MB";
                case var _ when fileSize >= gigabyte:
                    return $"{Math.Round(fileSize / gigabyte, 2, MidpointRounding.AwayFromZero):##,###.##}GB";
                default:
                    return "n/a";
            }
        }

        public static Bitmap ResizeImage(Image originalImg)
        {
            try
            {
                int newWidth = 200;
                float tempHeight = originalImg.Size.Width / originalImg.Size.Height;
                int newHeight = Convert.ToInt32(newWidth / tempHeight);
                Bitmap bm = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(bm);
                g.DrawImage(originalImg, 0, 0, newWidth, newHeight);
                g.Dispose();
                return bm;
            }
            catch
            {
                return null;
            }
        }

        public static void SaveImage(IFormFile file, string path, string fileName)
        {
            try
            {
                Image im = Image.FromStream(file.OpenReadStream());
                Bitmap bmp = ResizeImage(im);
                SaveImage(bmp, path, fileName);
            }
            catch
            {
                throw new ArgumentException("Parameter is not valid", "HttpPostFileBase");
            }

        }

        public static void SaveImage(Bitmap bmp, string path, string fileName)
        {
            try
            {
                path = Path.Combine(path);
                string FileName = path + fileName;
                bmp.Save(FileName, ImageFormat.Jpeg);
            }
            catch
            {
                throw new ArgumentException("Parameter is not valid", "Bitmap");
            }
        }
    }
}
