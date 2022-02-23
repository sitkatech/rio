using System;
using System.IO;

namespace Rio.API.GeoSpatial
{
    public class DisposableTempFile : IDisposable
    {
        private FileInfo _fileInfo;

        private bool _isDisposed;

        public DisposableTempFile()
            : this(Path.GetTempFileName())
        {
        }

        public DisposableTempFile(string tempFileName)
        {
            _fileInfo = new FileInfo(tempFileName);
        }

        public static DisposableTempFile MakeDisposableTempFileEndingIn(string fileEnding)
        {
            var tempFileName = Path.GetTempFileName();
            File.Delete(tempFileName); // we need to delete this right away once we get the path; Path.GetTempFileName() creates a zero byte file on disk
            var tempPath = tempFileName + fileEnding;
            return new DisposableTempFile(tempPath);
        }

        public FileInfo FileInfo
        {
            get
            {
                Check.Require(!_isDisposed, "Object is already disposed");
                return _fileInfo;
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                if (_fileInfo != null)
                {
                    var fileFullName = _fileInfo.FullName;
                    if (File.Exists(fileFullName))
                    {
                        File.Delete(fileFullName);
                    }
                    _fileInfo = null;
                }
                _isDisposed = true;
            }
        }

        ~DisposableTempFile()
        {
            Dispose();
        }
    }
}