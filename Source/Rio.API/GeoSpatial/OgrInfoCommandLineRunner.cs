using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using Rio.Models.DataTransferObjects;

namespace Rio.API.GeoSpatial
{
    public static class OgrInfoCommandLineRunner
    {
        public static List<string> GetFeatureClassNamesFromFileGdb(string pathToOgrInfoExecutable, string gdbFileInfo, double totalMilliseconds, ILogger logger)
        {
            var ogrInfoFileInfo = new FileInfo(pathToOgrInfoExecutable);
            var commandLineArguments = BuildOgrInfoCommandLineArgumentsToListFeatureClasses(gdbFileInfo, null);
            var processUtilityResult = ProcessUtility.ShellAndWaitImpl(ogrInfoFileInfo.DirectoryName, ogrInfoFileInfo.FullName, commandLineArguments, true, Convert.ToInt32(totalMilliseconds), logger);

            var featureClassesFromFileGdb = processUtilityResult.StdOut.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return featureClassesFromFileGdb.Select(x => x.Split(' ').Skip(1).First()).ToList();
        }

        public static List<FeatureClassInfo> GetFeatureClassInfoFromFileGdb(string pathToOgrInfoExecutable, string gdbFileInfo, double totalMilliseconds, ILogger logger, int? maxLayerNum = null)
        {
            var commandLineArguments = BuildOgrInfoCommandLineArgumentsToListFeatureClassInfos(gdbFileInfo, null);
            var processUtilityResult = ExecuteOgrInfoCommand(pathToOgrInfoExecutable, commandLineArguments, totalMilliseconds, logger);

            if (logger != null)
            {
                logger.LogInformation(processUtilityResult.StdOut);
            }
            else
            {
                Console.WriteLine(processUtilityResult.StdOut);
            }

            var featureClassesFromFileGdb = processUtilityResult.StdOut.Split(new[] { "\r\nLayer name: " }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();

            

            if (maxLayerNum.HasValue && featureClassesFromFileGdb.Count > maxLayerNum)
            {
                throw new ValidationException($"GDB has {featureClassesFromFileGdb.Count} layers, which exceeds the max layer count of {maxLayerNum}. Please upload a different GDB or edit the current one.");
            }

            var featureClassInfos = new List<FeatureClassInfo>();
            foreach (var featureClassBlob in featureClassesFromFileGdb)
            {
                var featureClassInfo = new FeatureClassInfo();

                if (!Proj4NetHelpers.WkTHasAppropriateSRS(featureClassBlob,
                    Ogr2OgrCommandLineRunner.DefaultCoordinateSystemId, logger))
                {
                    throw new ValidationException($"GDB contains a layer that is not projected to {Ogr2OgrCommandLineRunner.GetMapProjection(Ogr2OgrCommandLineRunner.DefaultCoordinateSystemId)}. Please upload a different GDB or edit the current one.");
                }

                var features = featureClassBlob.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).ToList();
                featureClassInfo.LayerName = features.First();
                featureClassInfo.FeatureType = features.First(x => x.StartsWith("Geometry: ")).Substring("Geometry: ".Length);
                featureClassInfo.FeatureCount = int.Parse(features.First(x => x.StartsWith("Feature Count: ")).Substring("Feature Count: ".Length));

                var columnNamesBlob = featureClassBlob.Split(new[] {"Geometry Column = "}, StringSplitOptions.RemoveEmptyEntries);
                if (columnNamesBlob.Length == 2)
                {
                    featureClassInfo.Columns = columnNamesBlob.Skip(1).Single()
                        .Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries).Skip(1).Select(x =>
                            x.Split(new[] {": "}, StringSplitOptions.RemoveEmptyEntries).First().ToLower()).ToList();
                }
                else
                {
                    featureClassInfo.Columns = new List<string>();
                }

                featureClassInfos.Add(featureClassInfo);
            }

            return featureClassInfos;
        }

        public static Envelope GetExtentForFeatureClassFromGdb(string pathToOgrInfoExecutable, string inputGdbFilePath,
            double totalMilliseconds, string featureClassName, int? optionalBuffer, ILogger logger, DirectoryInfo gdalDataDirectory)
        {
            var commandLineArguments = BuildOgrInfoCommandLineArgumentsToGetExtentForFeatureClass(inputGdbFilePath, gdalDataDirectory,
                featureClassName);
            var processUtilityResult = ExecuteOgrInfoCommand(pathToOgrInfoExecutable, commandLineArguments, totalMilliseconds, logger);

            var lines = processUtilityResult.StdOut.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (lines.Any(x => x.Contains("Feature Count: 0")))
            {
                return null;
            }

            var extentTokens = lines.First(x => x.StartsWith("Extent:")).Split(new[] { ' ', '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var envelope = new Envelope(double.Parse(extentTokens[1]), double.Parse(extentTokens[4]), double.Parse(extentTokens[2]), double.Parse(extentTokens[5]));
            if (optionalBuffer.HasValue)
            {
                envelope.ExpandBy(optionalBuffer.Value);
            }
            return envelope;
        }

        private static ProcessUtilityResult ExecuteOgrInfoCommand(string pathToOgrInfoExecutable,
            List<string> commandLineArguments,
            double totalMilliseconds, ILogger logger)
        {
            var ogrInfoFileInfo = new FileInfo(pathToOgrInfoExecutable);
            var processUtilityResult = ProcessUtility.ShellAndWaitImpl(ogrInfoFileInfo.DirectoryName, pathToOgrInfoExecutable,
                commandLineArguments, true, Convert.ToInt32(totalMilliseconds), logger);
            if (processUtilityResult.ReturnCode != 0)
            {
                var argumentsAsString = String.Join(" ", commandLineArguments.Select(ProcessUtility.EncodeArgumentForCommandLine).ToList());
                var fullProcessAndArguments =
                    $"{ProcessUtility.EncodeArgumentForCommandLine(ogrInfoFileInfo.FullName)} {argumentsAsString}";
                var errorMessage =
                    $"Process \"{ogrInfoFileInfo.Name}\" returned with exit code {processUtilityResult.ReturnCode}, expected exit code 0.\r\n\r\nStdErr and StdOut:\r\n{processUtilityResult.StdOutAndStdErr}\r\n\r\nProcess Command Line:\r\n{fullProcessAndArguments}\r\n\r\nProcess Working Directory: {ogrInfoFileInfo.DirectoryName}";
                throw new Ogr2OgrCommandLineException(errorMessage);
            }
            return processUtilityResult;
        }

        public static List<string> BuildOgrInfoCommandLineArgumentsToGetExtentForFeatureClass(string inputGdbFile, DirectoryInfo gdalDataDirectoryInfo, string featureClassName)
        {
            var commandLineArguments = new List<string>
            {
                "-ro",
                "-so",
                inputGdbFile,
                featureClassName
            };

            if (gdalDataDirectoryInfo != null)
            {
                commandLineArguments.Add("--config");
                commandLineArguments.Add("GDAL_DATA");
                commandLineArguments.Add(gdalDataDirectoryInfo.FullName);
            }

            return commandLineArguments;
        }

        public static List<string> BuildOgrInfoCommandLineArgumentsToListFeatureClasses(string inputGdbFile, DirectoryInfo gdalDataDirectoryInfo)
        {
            var commandLineArguments = new List<string>
            {
                "-ro",
                "-so",
                "-q",
                inputGdbFile
            };
            
            if (gdalDataDirectoryInfo != null)
            {
                commandLineArguments.Add("--config");
                commandLineArguments.Add("GDAL_DATA");
                commandLineArguments.Add(gdalDataDirectoryInfo.FullName);
            }

            return commandLineArguments;
        }
        public static List<string> BuildOgrInfoCommandLineArgumentsToListFeatureClassInfos(string inputGdbFile, DirectoryInfo gdalDataDirectoryInfo)
        {
            var commandLineArguments = new List<string>
            {
                "-al",
                "-ro",
                "-so",
                "-noextent",
                inputGdbFile
            };
            
            if (gdalDataDirectoryInfo != null)
            {
                commandLineArguments.Add("--config");
                commandLineArguments.Add("GDAL_DATA");
                commandLineArguments.Add(gdalDataDirectoryInfo.FullName);
            }

            return commandLineArguments;
        }

    }
}