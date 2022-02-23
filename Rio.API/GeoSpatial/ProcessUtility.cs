﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Rio.API.GeoSpatial
{
    public static class ProcessUtility
    {
        private static readonly TimeSpan MaxTimeout = TimeSpan.FromMinutes(10);

        public static string ConjoinCommandLineArguments(List<string> commandLineArguments)
        {
            return string.Join(" ", commandLineArguments.Select(EncodeArgumentForCommandLine).ToList());
        }

        public static ProcessUtilityResult ShellAndWaitImpl(string workingDirectory, string exeFileName,
            List<string> commandLineArguments, bool redirectStdErrAndStdOut, int? maxTimeoutMs, ILogger logger)
        {
            var argumentsAsString = ConjoinCommandLineArguments(commandLineArguments);
            var stdErrAndStdOut = string.Empty;

            // Start the indicated program and wait for it
            // to finish, hiding while we wait.
            var objProc = new Process { StartInfo = new ProcessStartInfo(exeFileName, argumentsAsString) };

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                objProc.StartInfo.WorkingDirectory = workingDirectory;
            }
            var streamReader = new ProcessStreamReader();
            if (redirectStdErrAndStdOut)
            {
                objProc.StartInfo.UseShellExecute = false;
                objProc.StartInfo.RedirectStandardOutput = true;
                objProc.StartInfo.RedirectStandardError = true;
                objProc.StartInfo.CreateNoWindow = true;
                objProc.OutputDataReceived += streamReader.ReceiveStdOut;
                objProc.ErrorDataReceived += streamReader.ReceiveStdErr;
            }

            var processDebugInfo = $"Process Details:\"{exeFileName}\" {argumentsAsString}\r\nWorking Directory: {workingDirectory}";
            var processInfoMessage = $"Starting Process: {processDebugInfo}";
            if (logger != null)
            {
                logger.LogInformation(processInfoMessage);
            }
            else
            {
                Console.WriteLine(processInfoMessage);
            }

            try
            {
                objProc.Start();
            }
            catch (Exception e)
            {
                var message = $"Program {Path.GetFileName(exeFileName)} got an exception on process start.\r\nException message: {e.Message}\r\n{processDebugInfo}";
                throw new Exception(message, e);
            }

            if (redirectStdErrAndStdOut)
            {
                objProc.BeginOutputReadLine();
                objProc.BeginErrorReadLine();
            }

            var processTimeoutPeriod = (maxTimeoutMs.HasValue) ? TimeSpan.FromMilliseconds(maxTimeoutMs.Value) : MaxTimeout;
            var hasExited = objProc.WaitForExit(Convert.ToInt32(processTimeoutPeriod.TotalMilliseconds));

            if (!hasExited)
            {
                objProc.Kill();
            }

            if (redirectStdErrAndStdOut)
            {
                // TODO: Fix this so it works without a hacky "Sleep", right now this hack waits for the output to trickle in. The asychronous reads of STDERR and STDOUT may not yet be complete (run unit test under debugger for example) even though the process has exited. -MF & ASW 11/21/2011
                Thread.Sleep(TimeSpan.FromSeconds(.25));
                stdErrAndStdOut = streamReader.StdOutAndStdErr;
            }

            if (!hasExited)
            {
                var message = $"Program {Path.GetFileName(exeFileName)} did not exit within timeout period {processTimeoutPeriod} and was terminated.\r\n{processDebugInfo}\r\nOutput:\r\n{streamReader.StdOutAndStdErr}";
                throw new Exception(message);
            }
            return new ProcessUtilityResult(objProc.ExitCode, streamReader.StdOut, stdErrAndStdOut);
        }

        /// <summary>
        /// Handles the reading of standard out and standard error
        /// </summary>
        private class ProcessStreamReader
        {
            private readonly object _outputLock = new object();
            private readonly StringBuilder _diagnosticOutput = new StringBuilder();
            private readonly StringBuilder _standardOut = new StringBuilder();

            public void ReceiveStdOut(object sender, DataReceivedEventArgs e)
            {
                lock (_outputLock)
                {
                    _diagnosticOutput.Append($"[stdout] {e.Data}\r\n");
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        _standardOut.Append($"{e.Data}\r\n");
                    }
                }
            }

            public void ReceiveStdErr(object sender, DataReceivedEventArgs e)
            {
                var message = $"[stderr] {e.Data}";
                lock (_outputLock)
                {
                    _diagnosticOutput.Append($"{message}\r\n");
                }
            }


            public string StdOutAndStdErr
            {
                get
                {
                    lock (_outputLock)
                    {
                        return _diagnosticOutput.ToString();
                    }
                }
            }

            public string StdOut
            {
                get
                {
                    lock (_outputLock)
                    {
                        return _standardOut.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Encode a command line argument
        /// </summary>
        public static string EncodeArgumentForCommandLine(string unencodedCommandLineArgument)
        {
            // We should be escaping command line arguments in C# directly, taking an string[] args here and doing the encoding inside
            // http://blogs.msdn.com/b/twistylittlepassagesallalike/archive/2011/04/23/everyone-quotes-arguments-the-wrong-way.aspx
            // http://stackoverflow.com/questions/5510343/escape-command-line-arguments-in-c-sharp
            // http://stackoverflow.com/questions/13796722/canonical-solution-for-escaping-net-command-line-arguments
            // http://msdn.microsoft.com/en-us/library/17w5ykft.aspx

            // void
            //ArgvQuote (
            //    const std::wstring& Argument,
            //    std::wstring& CommandLine,
            //    bool Force
            //    )
            //Routine Description:
            //    This routine appends the given argument to a command line such
            //    that CommandLineToArgvW will return the argument string unchanged.
            //    Arguments in a command line should be separated by spaces; this
            //    function does not add these spaces.
            //Arguments:
            //    Argument - Supplies the argument to encode.
            //    CommandLine - Supplies the command line to which we append the encoded argument string.
            //    Force - Supplies an indication of whether we should quote
            //            the argument even if it does not contain any characters that would
            //            ordinarily require quoting.
            //Return Value:
            //    None.
            //Environment:
            //    Arbitrary.
            //    //
            //    // Unless we're told otherwise, don't quote unless we actually
            //    // need to do so --- hopefully avoid problems if programs won't
            //    // parse quotes properly
            //    //
            //    if (Force == false &&
            //        Argument.empty () == false &&
            //        Argument.find_first_of (L" \t\n\v\"") == Argument.npos)
            //    {
            //        CommandLine.append (Argument);
            //    }
            //    else {
            //        CommandLine.push_back (L'"');
            //        for (auto It = Argument.begin () ; ; ++It) {
            //            unsigned NumberBackslashes = 0;
            //            while (It != Argument.end () && *It == L'\\') {
            //                ++It;
            //                ++NumberBackslashes;
            //            }
            //            if (It == Argument.end ()) {
            //                //
            //                // Escape all backslashes, but let the terminating
            //                // double quotation mark we add below be interpreted
            //                // as a metacharacter.
            //                //
            //                CommandLine.append (NumberBackslashes * 2, L'\\');
            //                break;
            //            }
            //            else if (*It == L'"') {
            //                //
            //                // Escape all backslashes and the following
            //                // double quotation mark.
            //                //
            //                CommandLine.append (NumberBackslashes * 2 + 1, L'\\');
            //                CommandLine.push_back (*It);
            //            }
            //            else {
            //                //
            //                // Backslashes aren't special here.
            //                //
            //                CommandLine.append (NumberBackslashes, L'\\');
            //                CommandLine.push_back (*It);
            //            }
            //        }
            //        CommandLine.push_back (L'"');
            //    }
            //}

            // Parsing C++ Command-Line Arguments
            // http://msdn.microsoft.com/en-us/library/17w5ykft.aspx
            // Microsoft C/C++ startup code uses the following rules when interpreting arguments given on the operating system command line: 
            // • Arguments are delimited by white space, which is either a space or a tab.
            // • The caret character (^) is not recognized as an escape character or delimiter. The character is handled completely by the command-line parser in the operating system before being passed to the argv array in the program.
            // • A string surrounded by double quotation marks ("string") is interpreted as a single argument, regardless of white space contained within. A quoted string can be embedded in an argument.
            // • A double quotation mark preceded by a backslash (\") is interpreted as a literal double quotation mark character (").
            // • Backslashes are interpreted literally, unless they immediately precede a double quotation mark.
            // • If an even number of backslashes is followed by a double quotation mark, one backslash is placed in the argv array for every pair of backslashes, and the double quotation mark is interpreted as a string delimiter.
            // • If an odd number of backslashes is followed by a double quotation mark, one backslash is placed in the argv array for every pair of backslashes, and the double quotation mark is "escaped" by the remaining backslash, causing a literal double quotation mark (") to be placed in argv.


            var charactersThatRequireEncodingRegex = new Regex("[ \t\r\n\v]");
            if (!charactersThatRequireEncodingRegex.IsMatch(unencodedCommandLineArgument))
            {
                return unencodedCommandLineArgument;
            }

            // We must surround with DQUOTE, but first handle special BACKSLASH and embedded DQUOTE stuff
            const char backslash = '\\';
            var encodedArgument = String.Empty;
            for (var i = 0; ; i++)
            {
                var numberOfBackslashes = 0;

                // When we hit a streak of backslashes, stop processing until we reach the end of the streak
                while (i < unencodedCommandLineArgument.Length && unencodedCommandLineArgument[i] == backslash)
                {
                    ++i;
                    ++numberOfBackslashes;
                }

                // If we're now at the end of the entire argument string...
                if (i == unencodedCommandLineArgument.Length)
                {
                    // Escape all backslashes, but let the terminating DQUOTE mark we add below be interpreted as a metacharacter not part of the actual argument
                    // • If an even number of backslashes is followed by a double quotation mark, one backslash is placed in the argv array for every pair of backslashes, and the double quotation mark is interpreted as a string delimiter
                    encodedArgument += new string(backslash, numberOfBackslashes * 2);
                    break;
                }

                // If we're now at a DQUOTE in the argument string...
                if (unencodedCommandLineArgument[i] == '"')
                {
                    // Escape all backslashes and the following DQUOTE
                    // • If an odd number of backslashes is followed by a double quotation mark, one backslash is placed in the argv array for every pair of backslashes, and the double quotation mark is "escaped" by the remaining backslash, causing a literal double quotation mark (") to be placed in argv
                    encodedArgument += new string(backslash, numberOfBackslashes * 2 + 1);
                    encodedArgument += unencodedCommandLineArgument[i];
                }
                else
                {
                    // Backslashes aren't special here, put them back in place and carry on
                    encodedArgument += new string(backslash, numberOfBackslashes);
                    encodedArgument += unencodedCommandLineArgument[i];
                }
            }
            // Surround the entire argument with DQUOTE
            return $"\"{encodedArgument}\"";
        }
    }
}