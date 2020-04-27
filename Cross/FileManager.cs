//using System;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Hosting;

//namespace Cross.FileManager
//{
//    public class FileManager : IDisposable
//    {
//        public enum FileSavingMode
//        {
//            CreateNew,
//            OverWrite
//        }

//        public enum PathSavingMode
//        {
//            Pure,
//            HostingEnvironment,
//            AppDomainApplicationBase
//        }

//        public enum DirectorySavingMode
//        {
//            UseExisted,
//            Create
//        }

//        public async Task<string> GetFullPathAsync(string path, string fileName,
//            PathSavingMode pathMode)
//        {
//            string fullPath;
//            switch (pathMode)
//            {
//                case PathSavingMode.Pure:
//                    fullPath = path.LastOrDefault() == '/' ? path + fileName : path + "/" + fileName;
//                    break;
//                case PathSavingMode.HostingEnvironment:
//                    await Task.Run(() => path = HostingEnvironment.MapPath(path));
//                    fullPath = path + fileName;
//                    break;
//                case PathSavingMode.AppDomainApplicationBase:
//                    await Task.Run(() => path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + (path.FirstOrDefault() == '~' ? path.Remove(0, 2) : path.FirstOrDefault() == '/' ? path.Remove(0, 1) : path));
//                    fullPath = path + fileName;
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(pathMode), pathMode, null);
//            }
//            return fullPath;
//        }

//        private async Task<Stream> GetFileStreamAsync(string path, string fileName, FileSavingMode fileSavingMode, PathSavingMode pathMode, DirectorySavingMode directorySavingMode)
//        {
//            var fullPath = await GetFullPathAsync(path, fileName, pathMode);
//            var directory = new DirectoryInfo(fullPath.Replace(fileName, ""));
//            switch (directorySavingMode)
//            {
//                case DirectorySavingMode.UseExisted:
//                    if (!directory.Exists) throw new DirectoryNotFoundException("Could not find a part of the path " + path);
//                    break;
//                case DirectorySavingMode.Create:
//                    if (!directory.Exists) await Task.Run(() => directory.Create());
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(directorySavingMode), directorySavingMode, null);
//            }
//            FileStream fileStream = null;
//            switch (fileSavingMode)
//            {
//                case FileSavingMode.CreateNew:
//                    await Task.Run(() => fileStream = new FileStream(fullPath, FileMode.CreateNew));
//                    break;
//                case FileSavingMode.OverWrite:
//                    await Task.Run(() => fileStream = new FileStream(fullPath, FileMode.Create));
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(fileSavingMode), fileSavingMode, null);
//            }
//            return fileStream;
//        }

//        public async Task SaveFileAsync(HttpPostedFileBase httpPostedFile, string path, FileSavingMode fileSavingMode = FileSavingMode.OverWrite, PathSavingMode pathMode = PathSavingMode.HostingEnvironment, DirectorySavingMode directorySavingMode = DirectorySavingMode.Create)
//        {
//            var fileName = httpPostedFile.FileName;
//            var fileStream = await GetFileStreamAsync(path, fileName, fileSavingMode, pathMode, directorySavingMode);
//            await httpPostedFile.InputStream.CopyToAsync(fileStream);
//            await Task.Run(() =>
//            {
//                fileStream.Close();
//                fileStream.Dispose();
//            });
//        }

//        public async Task SaveFileAsync(HttpPostedFileBase httpPostedFile, string path, string fileName, FileSavingMode fileSavingMode = FileSavingMode.OverWrite, PathSavingMode pathMode = PathSavingMode.HostingEnvironment, DirectorySavingMode directorySavingMode = DirectorySavingMode.Create)
//        {
//            var fileStream = await GetFileStreamAsync(path, fileName, fileSavingMode, pathMode, directorySavingMode);
//            await httpPostedFile.InputStream.CopyToAsync(fileStream);
//            await Task.Run(() =>
//            {
//                fileStream.Close();
//                fileStream.Dispose();
//            });
//        }

//        public async Task SaveFileAsync(Stream stream, string path, string fileName, FileSavingMode fileSavingMode = FileSavingMode.OverWrite, PathSavingMode pathMode = PathSavingMode.HostingEnvironment, DirectorySavingMode directorySavingMode = DirectorySavingMode.Create)
//        {
//            var fileStream = await GetFileStreamAsync(path, fileName, fileSavingMode, pathMode, directorySavingMode);
//            await stream.CopyToAsync(fileStream);
//            await Task.Run(() =>
//            {
//                fileStream.Close();
//                fileStream.Dispose();
//            });
//        }

//        public async Task SaveFileAsync(byte[] bytes, string path, string fileName, FileSavingMode fileSavingMode = FileSavingMode.OverWrite, PathSavingMode pathMode = PathSavingMode.HostingEnvironment, DirectorySavingMode directorySavingMode = DirectorySavingMode.Create)
//        {
//            var fileStream = await GetFileStreamAsync(path, fileName, fileSavingMode, pathMode, directorySavingMode);
//            await fileStream.WriteAsync(bytes, 0, bytes.Length);
//            await Task.Run(() =>
//            {
//                fileStream.Close();
//                fileStream.Dispose();
//            });
//        }

//        public async Task<string> IsFileExistedAsync(string path, string fileName, PathSavingMode pathMode = PathSavingMode.HostingEnvironment)
//        {
//            var fullPath = await GetFullPathAsync(path, fileName, pathMode);
//            return await Task.Run(() => File.Exists(fullPath)) ? fullPath : null;
//        }

//        public async Task<string> IsFolderExistedAsync(string path, PathSavingMode pathMode = PathSavingMode.HostingEnvironment)
//        {
//            var fullPath = await GetFullPathAsync(path, "", pathMode);
//            return await Task.Run(() => Directory.Exists(fullPath)) ? fullPath : null;
//        }

//        public async Task DeleteFileAsync(string path, string fileName, PathSavingMode pathMode = PathSavingMode.HostingEnvironment)
//        {
//            var fullPath = await GetFullPathAsync(path, fileName, pathMode);
//            if (File.Exists(fullPath)) await Task.Run(() => File.Delete(fullPath));
//        }

//        public async Task DeleteFolderAsync(string path, PathSavingMode pathMode = PathSavingMode.HostingEnvironment)
//        {
//            var fullPath = await GetFullPathAsync(path, "", pathMode);
//            if (Directory.Exists(fullPath)) await Task.Run(() => Directory.Delete(fullPath, true));
//        }

//        public void Dispose()
//        {
//            // TODO: Implement file manager dispose method.
//        }

//        public async Task CopyFileAsync(string sourceFileFullPath, string destinationFileFullPath)
//        {
//            await Task.Run(() => File.Copy(sourceFileFullPath, destinationFileFullPath));
//        }
//    }
//}
