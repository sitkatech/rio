using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rio.API.GeoSpatial
{
    /// <summary>
    /// Design By Contract Checks.
    ///
    /// Each method generates an exception or
    /// a Trace assertion if the contract is broken.
    ///
    /// If you wish to use Trace statements rather than exception handling then call the methods ending in Trace
    /// e.g., Check.RequireTrace(a > 1, "a must be > 1");
    /// Then output will be directed to a Trace listener. For example, you could insert
    ///
    /// Trace.Listeners.Clear();
    /// Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
    /// 
    /// </summary>
    public class Check
    {
        // abstract class - No creation
        private Check()
        {
        }

        #region Require

        /// <summary>
        /// Pinch point for all the throwing
        /// </summary>
        private static void ThrowThisException(Exception ex)
        {
            throw ex;
        }

        public static void Require(bool assertion)
        {
            if (!assertion)
                ThrowThisException(new PreconditionException());
        }

        public static void Require(bool assertion, string message)
        {
            if (!assertion)
                ThrowThisException(new PreconditionException(message));
        }

        public static void Require(bool assertion, string message, Exception inner)
        {
            if (!assertion)
                ThrowThisException(new PreconditionException(message, inner));
        }

        public static void Require(bool assertion, Exception ex)
        {
            if (!assertion)
                throw ex;
        }

        public static void Require(bool assertion, Func<Exception> func)
        {
            if (!assertion)
                throw func();
        }

        public static void RequireGreaterThanZero(int value, string message)
        {
            if (value <= 0)
                ThrowThisException(new PreconditionException(message));
        }

        public static void RequireNotDisposed(bool isDisposed, object thisObject)
        {
            if (isDisposed)
                ThrowThisException(new ObjectDisposedException(thisObject.GetType().ToString()));
        }

        public static void RequireNotNull(object thisObject)
        {
            if (thisObject == null)
                ThrowThisException(new NullReferenceException());
        }

        public static void RequireNotNull(object thisObject, string message)
        {
            if (thisObject == null)
                ThrowThisException(new NullReferenceException(message));
        }

        public static void RequireNotNull(object thisObject, Exception exception)
        {
            if (thisObject == null)
                throw exception;
        }

        public static void RequireNotNull(object thisObject, Func<Exception> func)
        {
            if (thisObject == null)
                throw func();
        }

        public static void RequireNotNullNotEmpty(string stringToCheck, string message)
        {
            RequireNotNull(stringToCheck, message);
            if (string.IsNullOrEmpty(stringToCheck))
                ThrowThisException(new ArgumentException(message + " String is empty, expected non-empty string."));
        }

        public static void RequireNotNullNotEmpty<T>(IEnumerable<T> itemsToCheck, string message)
        {
            RequireNotNull(itemsToCheck, message);
            if (!itemsToCheck.Any())
            {
                ThrowThisException(new ArgumentException(message + " Item list is empty, expected non-empty list."));
            }
        }

        public static void RequireNotNullNotEmptyNotWhitespace(string stringToCheck, string message)
        {
            RequireNotNull(stringToCheck, message);
            if (string.IsNullOrWhiteSpace(stringToCheck))
                ThrowThisException(new ArgumentException(message + " String is empty or only blank, expected non-empty string with some non-whitespace characters."));
        }

        public static void RequireNoWhitespace(string stringToExamine, string message)
        {
            if (Regex.IsMatch(stringToExamine, @"\s"))
            {
                ThrowThisException(new ArgumentException(message + " String \"{0}\" contains one or more whitespace characters and isn't allowed to contain whitespace."));
            }
        }

        public static void RequireDirectoryExists(string path)
        {
            if (!DirectoryExists(path, out var problem))
            {
                ThrowThisException(new DirectoryNotFoundException(problem));
            }
        }

        public static void RequireDirectoryExists(DirectoryInfo dir)
        {
            if (!DirectoryExists(dir, out var problem))
            {
                ThrowThisException(new DirectoryNotFoundException(problem));
            }
        }
        public static void RequireDirectoryExists(string path, string message)
        {
            if (!DirectoryExists(path, out var problem))
            {
                ThrowThisException(new DirectoryNotFoundException(message + "\r\n" + problem + "\r\n" + "Directory: " + path));
            }
        }


        public static void RequireFileExists(FileInfo file, string message)
        {
            if (!FileExists(file, out var problem))
            {
                var messageFormatted = (String.IsNullOrEmpty(message) || message.Trim() == String.Empty) ? string.Empty : message + "\r\n";
                ThrowThisException(new FileNotFoundException(messageFormatted + problem, file.FullName));
            }
        }

        public static void RequirePathExists(string path, string message)
        {
            var isFileAndExists = File.Exists(path);
            var isDirectoryAndExists = Directory.Exists(path);
            if (!isFileAndExists && !isDirectoryAndExists)
            {
                var messageFormatted = (String.IsNullOrEmpty(message) || message.Trim() == String.Empty) ? string.Empty : message + "\r\n";
                ThrowThisException(new FileNotFoundException(messageFormatted, path));
            }
        }

        public static void RequireFileExists(FileInfo file)
        {
            RequireFileExists(file, string.Empty);
        }

        public static void RequireFileExists(string file)
        {
            RequireFileExists(new FileInfo(file), string.Empty);
        }

        public static void RequireFileExists(string file, string message)
        {
            RequireFileExists(new FileInfo(file), message);
        }

        public static void RequireFileExists(FileInfo[] files)
        {
            string problem;
            if (!FileExists(files, out problem))
            {
                ThrowThisException(new FileNotFoundException(problem));
            }
        }

        #endregion

        private static bool FileExists(FileInfo[] files, out string problem)
        {
            bool exists = true;
            problem = string.Empty;
            if (files == null)
            {
                exists = false;
                problem = "FileInfo[] object is null.\n";
            }
            else
            {
                foreach (FileInfo file in files)
                {
                    var thisExists = FileExists(file, out var thisProblem);
                    exists = exists && thisExists;
                    problem += thisProblem;
                }
            }
            return exists;
        }

        private static bool FileExists(FileInfo file, out string problem)
        {
            var exists = true;
            problem = string.Empty;
            if (file == null)
            {
                exists = false;
                problem = "FileInfo object is null.\n";
            }
            else if (!File.Exists(file.FullName))
            {
                exists = false;
                problem = $"File \"{file.FullName}\" not found.\n";
            }
            return exists;
        }

        private static bool DirectoryExists(string dirName, out string problem)
        {
            var exists = true;
            problem = string.Empty;
            if (dirName == null)
            {
                exists = false;
                problem = "Directory Name is null.\n";
            }
            else if (!Directory.Exists(dirName))
            {
                exists = false;
                problem = $"Directory \"{dirName}\" not found.\n";
            }
            return exists;
        }

        private static bool DirectoryExists(DirectoryInfo dir, out string problem)
        {
            bool exists;
            if (dir == null)
            {
                exists = false;
                problem = "DirectoryInfo object is null.\n";
            }
            else
            {
                exists = DirectoryExists(dir.FullName, out problem);
            }
            return exists;
        }


        public static void RequireType<T>(object objectRequiringType, string message)
        {
            var fullMessage = $"Expected object of type {typeof(T).Name} but got type {objectRequiringType.GetType().Name}";
            if (!(string.IsNullOrEmpty(message)))
            {
                fullMessage = $"{message} {fullMessage}";
            }
            Require(objectRequiringType is T, fullMessage);
        }
    }
}