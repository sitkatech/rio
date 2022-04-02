using System.Linq;

namespace Rio.EFModels.Entities
{
    public static class FileResourceMimeTypes
    {
        public static FileResourceMimeType GetFileResourceMimeTypeByContentTypeName(RioDbContext dbContext, string contentTypeName)
        {
            return FileResourceMimeType.All.Single(x => x.FileResourceMimeTypeContentTypeName == contentTypeName);
        }
    }
}
