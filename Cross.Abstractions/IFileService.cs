using Cross.Abstractions.EntityEnums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cross.Abstractions
{
    public interface IFileService
    {
        string SaveFile(IFormFile file, FileTypes fileType);
        bool FileTypeValidator(IFormFile file, FileTypes fileType);
        IFormFile PhotoResizer(IFormFile file);
        string SizeDeterminator(long bytes);
    }
}
