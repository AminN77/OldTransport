//using System;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Hosting;

//namespace Utility.Helper
//{
//    /// <summary>
//    /// کلاس ولیدیشن
//    /// </summary>
//    public static class ValidationLibrary
//    {
//        // EMAIL
//        public static bool isEmail(string word)
//        {
//            string pattern = @"/^(\w|\.|_){0,20}@(\w){1,20}(\.[a-zA-Z]{2,6}){1,5}$/";
//            return Regex.IsMatch(word, pattern);
//        }

//        // PERSIAN
//        public static bool isFarsi(string word)
//        {
//            string pattern = @"/^([\u0600-\u06ff]|\s){3,}$/";
//            return Regex.IsMatch(word, pattern);
//        }

//        // ENGLISH
//        public static bool isEnglish(string word)
//        {
//            string pattern = @"/^([a-zA-Z]|[0-9]|-|_|\.){3,}$/";
//            return Regex.IsMatch(word, pattern);
//        }

//        // Digit
//        public static bool isDigit(string word)
//        {
//            string pattern = @"/^\d{*}$/";
//            return Regex.IsMatch(word, pattern);
//        }

//        // Word
//        public static bool isWord(string word)
//        {
//            string pattern = @"/^[a-zA-Z | \u0600-\u06ff]$/";
//            return Regex.IsMatch(word, pattern);
//        }

//        // length
//        public static bool isLength(string word, int length)
//        {
//            string pattern = @"/^.{" + length + "}$/";
//            return Regex.IsMatch(word, pattern);
//        }

//        // Image File
//        public static bool isImg(string file)
//        {
//            string pattern = @"/^([a-zA-Z0-9\s_\\.\-:])+(.jpg|.jpeg|.gif|.png|.bmp)$/";
//            return Regex.IsMatch(file, pattern);
//        }

//        // Image File
//        public static bool isDocument(string file)
//        {
//            string pattern = @"/^([a-zA-Z0-9\s_\\.\-:])+(.doc|.docx|.pdf)$/";
//            return Regex.IsMatch(file, pattern);
//        }

//    }

//    /// <summary>
//    /// کلاس کار با فایل ها
//    /// </summary>
//    public class FileLibrary
//    {

//        #region Variables

//        public readonly static string defaultCodeIconPath = HostingEnvironment.MapPath("~/Uploads/Images/CodeIcon");

//        public readonly static string defaultTtsFilePath = HostingEnvironment.MapPath("~/Uploads/Sounds/Tts");
//        #endregion

//        #region Methods

//        //==========================================================  File Zip

//        public static void ZipFile(string name)
//        {

//        }

//        public static void ExtractZipFile(string name)
//        {

//        }


//        //==========================================================  File Save ( Text )
//        /// <summary>
//        /// ذخیره فایل
//        /// </summary>
//        public static string SaveFile(HttpPostedFileBase file)
//        {
//            try
//            {
//                string extension = Path.GetExtension(file.FileName);
//                string fileName = "file-" + Guid.NewGuid().ToString("N") + extension;
//                file.SaveAs(defaultTtsFilePath + fileName);
//                return fileName;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        internal static string SaveFile(Stream stream, string fileName)
//        {
//            try
//            {
//                string extension = Path.GetExtension(fileName);
//                string newFileName = "file-" + Guid.NewGuid().ToString("N") + extension;
//                using (var fileStream = new FileStream(defaultTtsFilePath + newFileName, FileMode.Create, FileAccess.Write))
//                {
//                    stream.CopyTo(fileStream);
//                }
//                return newFileName;
//                //FileStream fileStream = new FileStream("", FileMode.Open, FileAccess.ReadWrite);

//                //FileStream fileStream = File.Create(defaultFilePath, (int)stream.Length);
//                //// Initialize the bytes array with the stream length and then fill it with data
//                //byte[] bytesInStream = new byte[stream.Length];
//                //stream.Read(bytesInStream, 0, bytesInStream.Length);
//                //// Use write method to write to the file specified above
//                //fileStream.Write(bytesInStream, 0, bytesInStream.Length);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        internal static void DeleteFile(string fileName)
//        {
//            try
//            {
//                string deleteFileName = defaultTtsFilePath + fileName;
//                if (File.Exists(deleteFileName))
//                {
//                    File.Delete(deleteFileName);
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }


//        //==========================================================  File Save ( Images )

//        /// <summary>
//        /// با یک ورودی تصویر
//        /// </summary>
//        /// <param name="image"></param>
//        public static string SaveImage(Stream image, string fileName)
//        {
//            try
//            {
//                Image im = Image.FromStream(image);
//                Bitmap bmp = ImagesLibrary.ResizeImage(im);
//                string extension = Path.GetExtension(fileName);
//                string newFileName = "file-" + Guid.NewGuid().ToString("N") + extension;
//                SaveImage(bmp, defaultCodeIconPath, newFileName);
//                return newFileName;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        /// <summary>
//        /// با یک ورودی تصویر
//        /// </summary>
//        /// <param name="image"></param>
//        public static string SaveImage(HttpPostedFileBase image)
//        {
//            try
//            {
//                Image im = Image.FromStream(image.InputStream);
//                Bitmap bmp = ImagesLibrary.ResizeImage(im);
//                string extension = Path.GetExtension(image.FileName);
//                string fileName = "file-" + Guid.NewGuid().ToString("N") + extension;
//                SaveImage(bmp, defaultCodeIconPath, fileName);
//                return fileName;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        /// <summary>
//        /// با دو ورودی تصویر و مسیر
//        /// </summary>
//        /// <param name="image"></param>
//        /// <param name="path"></param>
//        public static string SaveImage(HttpPostedFileBase image, string path)
//        {
//            try
//            {
//                Image im = Image.FromStream(image.InputStream);
//                Bitmap bmp = ImagesLibrary.ResizeImage(im);
//                string extension = Path.GetExtension(image.FileName);
//                string fileName = "file-" + Guid.NewGuid().ToString("N") + extension;
//                SaveImage(bmp, path, fileName);
//                return fileName;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }

//        /// <summary>
//        /// با سه ورودی تصویر ، مسیر و نام فایل
//        /// </summary>
//        /// <param name="image"></param>
//        /// <param name="path"></param>
//        /// <param name="fileName"></param>
//        public static void SaveImage(HttpPostedFileBase image, string path, string fileName)
//        {
//            try
//            {
//                Image im = Image.FromStream(image.InputStream);
//                Bitmap bmp = ImagesLibrary.ResizeImage(im);
//                SaveImage(bmp, path, fileName);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }

//        }

//        /// <summary>
//        /// با سه ورودی تصویر ، مسیر ، نام فایل
//        /// </summary>
//        /// <param name="bmp"></param>
//        /// <param name="path"></param>
//        /// <param name="fileName"></param>
//        public static void SaveImage(Bitmap bmp, string path, string fileName)
//        {
//            try
//            {
//                path = Path.Combine(path);
//                string FileName = path + fileName;
//                bmp.Save(FileName, ImageFormat.Jpeg);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }


//        //==========================================================  File Delte ( Images )
//        /// <summary>
//        /// حذف تصویر
//        /// </summary>
//        /// <param name="imageName"></param>
//        public static void DeleteImage(string imageName)
//        {
//            try
//            {
//                string fileName = defaultCodeIconPath + imageName;
//                if (File.Exists(fileName))
//                {
//                    File.Delete(fileName);
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }

//        #endregion
//    }

//    /// <summary>
//    /// کلاس کار با تصاویر
//    /// </summary>
//    public class ImagesLibrary
//    {

//        #region Enums
//        public enum ImageComperssion
//        {
//            Maximum = 50,
//            Good = 60,
//            Normal = 70,
//            Fast = 80,
//            Minimum = 90,
//            None = 100,
//        }
//        #endregion

//        #region Methods

//        //==========================================================  Resize Images
//        /// <summary>
//        /// با یک ورودی تصویر
//        /// </summary>
//        /// <param name="originalImg"></param>
//        /// <returns></returns>
//        public static Bitmap ResizeImage(Image originalImg)
//        {
//            try
//            {
//                int newWidth = 200;
//                float tempHeight = (float)originalImg.Size.Width / (float)originalImg.Size.Height;
//                int newHeight = Convert.ToInt32(newWidth / tempHeight);
//                return ResizeImage(originalImg, newWidth, newHeight);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        /// <summary>
//        /// با دو ورودی تصویر و اندازه عرض
//        /// </summary>
//        /// <param name="originalImg"></param>
//        /// <param name="nWidth"></param>
//        /// <returns></returns>
//        public static Bitmap ResizeImage(Image originalImg, int nWidth)
//        {
//            try
//            {
//                int newWidth = nWidth;
//                float tempHeight = (float)originalImg.Size.Width / (float)originalImg.Size.Height;
//                int newHeight = Convert.ToInt32(newWidth / tempHeight);
//                return ResizeImage(originalImg, newWidth, newHeight);
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }
//        /// <summary>
//        /// با سه ورودی تصویر و اندازه طول و عرض
//        /// </summary>
//        /// <param name="originalImg"></param>
//        /// <param name="nWidth"></param>
//        /// <param name="nHeight"></param>
//        /// <returns></returns>
//        public static Bitmap ResizeImage(Image originalImg, int nWidth, int nHeight)
//        {
//            try
//            {
//                Bitmap bm = new Bitmap(nWidth, nHeight);
//                Graphics g = Graphics.FromImage(bm);
//                g.DrawImage(originalImg, 0, 0, nWidth, nHeight);
//                g.Dispose();
//                return bm;
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//        }


//        //==========================================================  GrayScale Images
//        /// <summary>
//        /// با یک ورودی تصویر
//        /// </summary>
//        /// <param name="originalImg"></param>
//        /// <returns></returns>
//        public static Bitmap GrayScaleImage(Image originalImg)
//        {
//            try
//            {
//                Bitmap newBitmap = new Bitmap(originalImg.Width, originalImg.Height);
//                Graphics g = Graphics.FromImage(newBitmap);

//                ColorMatrix colorMatrix = new ColorMatrix(
//                    new float[][]{
//                new float[] {.3f, .3f, .3f, 0, 0},
//                new float[] {.59f, .59f, .59f, 0, 0},
//                new float[] {.11f, .11f, .11f, 0, 0},
//                new float[] {0, 0, 0, 1, 0},
//                new float[] {0, 0, 0, 0, 1}});

//                ImageAttributes attributes = new ImageAttributes();
//                attributes.SetColorMatrix(colorMatrix);

//                g.DrawImage(originalImg,
//                    new Rectangle(0, 0, originalImg.Width, originalImg.Height),
//                    0, 0, originalImg.Width, originalImg.Height,
//                    GraphicsUnit.Pixel, attributes
//                );

//                g.Dispose();
//                return newBitmap;
//            }
//            catch
//            {
//                throw new ArgumentNullException("Argument null exception.");
//            }
//        }


//        #endregion
//    }

//}