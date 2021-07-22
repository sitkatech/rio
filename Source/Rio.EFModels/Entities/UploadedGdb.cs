using System;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    public partial class UploadedGdb
    {
        public static byte[] GetUploadedGdbFileContents(RioDbContext dbContext, int uploadedGdbID)
        {
            return dbContext.UploadedGdbs.AsNoTracking().SingleOrDefault(x => x.UploadedGdbID == uploadedGdbID)?.GdbFileContents;
        }

        public static int CreateNew(RioDbContext dbContext, byte[] gdbFileContents)
        {
            var nextUploadedGdbID = (dbContext.UploadedGdbs.Any() ? dbContext.UploadedGdbs.Max(x => x.UploadedGdbID) : 0) + 1;

            var dataTable = new DataTable();
            dataTable.Columns.Add("UploadedGdbID", typeof(int));
            dataTable.Columns.Add("GdbFileContents", typeof(byte[]));
            dataTable.Columns.Add("UploadDate", typeof(DateTime));
            var dataRow = dataTable.NewRow();
            dataRow["UploadedGdbID"] = nextUploadedGdbID;
            dataRow["GdbFileContents"] = gdbFileContents;
            dataRow["UploadDate"] = DateTime.UtcNow;
            dataTable.Rows.Add(dataRow);

            // delete the existing row
            const string tableName = "dbo.UploadedGdb";

            using var sqlBulk = new SqlBulkCopy(dbContext.Database.GetDbConnection().ConnectionString, SqlBulkCopyOptions.KeepIdentity);
            sqlBulk.DestinationTableName = tableName;
            sqlBulk.WriteToServer(dataTable);
            return nextUploadedGdbID;
        }


        public static void Delete(RioDbContext dbContext, int uploadedGdbID)
        {
            // doing it this way because it's too slow to load the humungous GDB file contents just to delete it
            dbContext.Database.ExecuteSqlRaw($"DELETE FROM dbo.UploadedGdb WHERE UploadedGdbID = {uploadedGdbID}");
        }
    }
}