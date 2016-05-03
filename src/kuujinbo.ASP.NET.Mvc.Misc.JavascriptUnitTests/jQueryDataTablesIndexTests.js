/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/lib/DataTables/jquery.dataTables.js" />
/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/lib/DataTables/dataTables.bootstrap.js" />
/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/lib/jquery-ui-1.11.4.js" />
/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/jQueryDataTablesIndex.js" />
'use strict';

describe('jQueryDataTablesIndex', function () {

    var _configTable = window.configTable;

    it('should TEST', function () {
        expect(_configTable).toBeDefined();
        expect(configTable.getTableId()).toEqual('#jquery-data-table');
    });

});