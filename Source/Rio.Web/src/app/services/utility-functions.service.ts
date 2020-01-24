import { Injectable } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { CsvExportParams } from 'ag-grid-community';

@Injectable({
  providedIn: 'root'
})
export class UtilityFunctionsService {

  constructor() { }

  public exportGridToCsv(grid: AgGridAngular, fileName: string, columnKeys: Array<string>) {
    var params =
      {
        skipHeader: false,
        columnGroups: false,
        skipFooters: true,
        skipGroups: true,
        skipPinnedTop: true,
        skipPinnedBottom: true,
        allColumns: true,
        onlySelected: false,
        suppressQuotes: false,
        fileName: fileName,
        processCellCallback: function (p) {
          if (p.column.getColDef().cellRendererFramework) {
            if (p.value.DownloadDisplay) {
              return p.value.DownloadDisplay;
            } else {
              return p.value.LinkDisplay;
            }
          }
          else {
            return p.value;
          }
        }
      } as CsvExportParams
    if (columnKeys) {
      params.columnKeys = columnKeys;
    }
    grid.api.exportDataAsCsv(params);
  }
}
