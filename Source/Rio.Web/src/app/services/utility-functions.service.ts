import { DatePipe, DecimalPipe } from '@angular/common';
import { Injectable } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, CsvExportParams } from 'ag-grid-community';

@Injectable({
  providedIn: 'root'
})
export class UtilityFunctionsService {

  constructor(
    private decimalPipe: DecimalPipe,
    private datePipe: DatePipe
  ) { }

  private decimalValueGetter(params: any, fieldName): number {
    const fieldNames = fieldName.split('.');
    if (fieldNames.length == 1) {
      return params.data[fieldName] ?? 0;
    }

    // checks that each part of a nested field is not null
    var fieldValue = params.data;
    fieldNames.forEach(x => {
      fieldValue = fieldValue[x];
      if (!fieldValue) {
        fieldValue = 0;
        return;
      }
    });

    return fieldValue;
  }

  public createDecimalColumnDef(headerName: string, fieldName: string, width?: number, decimalPlacesToDisplay?: number) {
    const _decimalPipe = this.decimalPipe;
    const decimalFormatString = decimalPlacesToDisplay != null ? 
      '1.' + decimalPlacesToDisplay + '-' + decimalPlacesToDisplay : '1.2-2';
  
    var decimalColDef: ColDef = {
      headerName: headerName, filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right' },
      valueGetter: params => this.decimalValueGetter(params, fieldName),
      valueFormatter: params => _decimalPipe.transform(params.value, decimalFormatString),
      filterValueGetter: params => parseFloat(_decimalPipe.transform(this.decimalValueGetter(params, fieldName), decimalFormatString))
    }
    if (width) {
      decimalColDef.width = width
    }
  
    return decimalColDef;
  }

  private dateFilterComparator(filterLocalDateAtMidnight, cellValue) {
    const filterDate = Date.parse(filterLocalDateAtMidnight);
    const cellDate = Date.parse(cellValue);

    if (cellDate == filterDate) {
      return 0;
    }
    return (cellDate < filterDate) ? -1 : 1;
  }

  private dateSortComparator (id1: any, id2: any) {
    const date1 = id1 ? Date.parse(id1) : Date.parse("1/1/1900");
    const date2 = id2 ? Date.parse(id2) : Date.parse("1/1/1900");
    if (date1 < date2) {
      return -1;
    }
    return (date1 > date2)  ?  1 : 0;
  }

  public createDateColumnDef(headerName: string, fieldName: string, dateFormat: string, width?: number): ColDef {
    const _datePipe = this.datePipe;
    var dateColDef: ColDef = {
      headerName: headerName, valueGetter: function (params: any) {
        return _datePipe.transform(params.data[fieldName], dateFormat);
      },
      comparator: this.dateSortComparator,
      filter: 'agDateColumnFilter',
      filterParams: {
        filterOptions: ['inRange'],
        comparator: this.dateFilterComparator
      }, 
      width: 110,
      resizable: true,
      sortable: true
    };
    if (width) {
      dateColDef.width = width;
    }

    return dateColDef;
  }

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
