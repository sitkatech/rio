using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rio.EFModels.Entities
{
    public partial class FileResourceMimeType
    {
        public static FileResourceMimeType GetFileResourceMimeTypeByContentTypeName(RioDbContext dbContext, string contentTypeName)
        {
            return dbContext.FileResourceMimeTypes.Single(x => x.FileResourceMimeTypeContentTypeName == contentTypeName);
        }
    }
}
