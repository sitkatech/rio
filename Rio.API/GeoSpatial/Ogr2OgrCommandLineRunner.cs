using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace Rio.API.GeoSpatial
{
    /// <summary>
    /// Wrapper class for calling ogr2ogr.exe for the purpose of importing data from a File Geodatabase (.gdb) using the OpenFileGDB drivers in GDAL 1.11 and above
    /// </summary>
    public class Ogr2OgrCommandLineRunner
    {
        public const int DefaultCoordinateSystemId = 2229;

        private readonly FileInfo _ogr2OgrExecutable;
        private readonly int? _coordinateSystemId;
        private readonly double _totalMilliseconds;
        private readonly bool _specifyGdalDirectory;

        public Ogr2OgrCommandLineRunner(string pathToOgr2OgrExecutable, int? coordinateSystemId, double totalMilliseconds, bool specifyGdalDirectory)
        {
            _totalMilliseconds = totalMilliseconds;
            _specifyGdalDirectory = specifyGdalDirectory;
            _ogr2OgrExecutable = new FileInfo(pathToOgr2OgrExecutable);
            _coordinateSystemId = coordinateSystemId;
            Check.RequireFileExists(_ogr2OgrExecutable, "Can't find ogr2ogr program in expected path. Is it installed?");
            Check.RequireNotNull(_ogr2OgrExecutable.Directory,
                $"ogr2ogr must be a full path including directory but was \"{_ogr2OgrExecutable.FullName}\"");
        }

        public string ImportFileGdbToGeoJson(string inputGdbFilePath, string sourceLayerName,
            List<string> columnNameList, int? significantDigits, ILogger logger, string filter, bool explodeCollections)
        {
            Check.RequirePathExists(inputGdbFilePath, "Can't find input File GDB for import with ogr2ogr");
            var gdalDataDirectoryInfo = _specifyGdalDirectory ? new DirectoryInfo(Path.Combine(_ogr2OgrExecutable.Directory.FullName, "gdal-data")) : null;
            var commandLineArguments = BuildCommandLineArgumentsForFileGdbToGeoJson(inputGdbFilePath, gdalDataDirectoryInfo, sourceLayerName, sourceLayerName, columnNameList, filter, _coordinateSystemId, explodeCollections, significantDigits, null);
            var processUtilityResult = ExecuteOgr2OgrCommand(commandLineArguments, logger);
            return processUtilityResult.StdOut;
        }

        protected ProcessUtilityResult ExecuteOgr2OgrCommand(List<string> commandLineArguments,
            ILogger logger)
        {
            var processUtilityResult = ProcessUtility.ShellAndWaitImpl(_ogr2OgrExecutable.DirectoryName, _ogr2OgrExecutable.FullName, commandLineArguments, true, Convert.ToInt32(_totalMilliseconds), logger);
            if (processUtilityResult.ReturnCode != 0)
            {
                var argumentsAsString = String.Join(" ", commandLineArguments.Select(ProcessUtility.EncodeArgumentForCommandLine).ToList());
                var fullProcessAndArguments =
                    $"{ProcessUtility.EncodeArgumentForCommandLine(_ogr2OgrExecutable.FullName)} {argumentsAsString}";
                var errorMessage =
                    $"Process \"{_ogr2OgrExecutable.Name}\" returned with exit code {processUtilityResult.ReturnCode}, expected exit code 0.\r\n\r\nStdErr and StdOut:\r\n{processUtilityResult.StdOutAndStdErr}\r\n\r\nProcess Command Line:\r\n{fullProcessAndArguments}\r\n\r\nProcess Working Directory: {_ogr2OgrExecutable.DirectoryName}";
                throw new Ogr2OgrCommandLineException(errorMessage);
            }
            return processUtilityResult;
        }

        /// <summary>
        /// Produces the command line arguments for ogr2ogr.exe to run the File Geodatabase import.
        /// <example>"C:\Program Files\GDAL\ogr2ogr.exe" -preserve_fid --config GDAL_DATA "C:\\Program Files\\GDAL\\gdal-data" -t_srs EPSG:4326 -f GeoJSON /dev/stdout "C:\\svn\\sitkatech\\trunk\\Corral\\Source\\Neptune.Web\\Models\\GdalOgr\\SampleFileGeodatabase.gdb.zip" "somelayername"</example>
        /// </summary>
        internal static List<string> BuildCommandLineArgumentsForFileGdbToGeoJson(string inputGdbFilePath,
            DirectoryInfo gdalDataDirectoryInfo, string sourceLayerName, string targetTableName,
            List<string> columnNameList, string filter, int? coordinateSystemId, bool explodeCollections,
            int? significantDigits, Envelope extent)
        {
            var reservedFields = new[] { "Ogr_Fid", "Ogr_Geometry" };
            var filteredColumnNameList = columnNameList.Where(x => reservedFields.All(y => !String.Equals(x, y, StringComparison.InvariantCultureIgnoreCase))).ToList();
            const string ogr2OgrColumnListSeparator = ",";
            Check.Require(filteredColumnNameList.All(x => !x.Contains(ogr2OgrColumnListSeparator)),
                $"Found column names with separator character \"{ogr2OgrColumnListSeparator}\", can't continue. Columns:{String.Join("\r\n", filteredColumnNameList)}");

            var columnNames = filteredColumnNameList.Any() ? string.Join(ogr2OgrColumnListSeparator + " ", filteredColumnNameList) : "*";
            var selectStatement = $"select {columnNames} from {sourceLayerName} {filter}";

            var commandLineArguments = new List<string>
            {
                "-sql",
                selectStatement,
                explodeCollections ? "-explodecollections" : null,
                "-f",
                "GeoJSON",
                "/dev/stdout",
                inputGdbFilePath,
                "-nln",
                targetTableName
            };

            if (coordinateSystemId.HasValue)
            {
                commandLineArguments.AddRange(new List<string>
                {
                    "-t_srs",
                    GetMapProjection(coordinateSystemId.Value)
                });
            }
            

            if (gdalDataDirectoryInfo != null)
            {
                commandLineArguments.Add("--config");
                commandLineArguments.Add("GDAL_DATA");
                commandLineArguments.Add(gdalDataDirectoryInfo.FullName);
            }

            if (extent != null)
            {
                commandLineArguments.Add("-clipsrc");
                commandLineArguments.Add(extent.MinX.ToString());
                commandLineArguments.Add(extent.MinY.ToString());
                commandLineArguments.Add(extent.MaxX.ToString());
                commandLineArguments.Add(extent.MaxY.ToString());
            }

            // layer creation options: see https://gdal.org/drivers/vector/geojson.html
            var layerCreationOptions = new List<string>()
            {
                "-lco",
                "COORDINATE_PRECISION=14",
                significantDigits.HasValue ? "SIGNIFICANT_FIGURES=" + significantDigits : null
            };

            return commandLineArguments.Where(x => x != null).Concat(layerCreationOptions.Where(x => !string.IsNullOrWhiteSpace(x))).ToList();
        }

        public static string GetMapProjection(int coordinateSystemId)
        {
            return $"EPSG:{coordinateSystemId}";
        }
    }
}
