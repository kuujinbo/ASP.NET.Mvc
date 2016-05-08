$(document).ready(function () {
    // DataTables API instance => $().DataTable() - note CASE
    var table = $(configTable.getTableId()).DataTable({
        processing: true,
        serverSide: true,
        deferRender: true,
        stateSave: true,
        // true by default, allow  shift-click multiple column sorting
        // orderMulti: configValues.allowMultiColumnSorting,
        orderMulti: true,
        dom: 'lrtip',
        pagingType: 'full_numbers',
        // autoWidth: true,
        order: [[1, 'asc']],
        language: {
            processing: configTable.getLoadingElement(),
            paginate: {
                previous: "<span class='glyphicon glyphicon-chevron-left' title='PREVIOUS' />",
                next: "<span class='glyphicon glyphicon-chevron-right'  title='NEXT' />",
                first: "<span class='glyphicon glyphicon-fast-backward' title='FIRST' />",
                last: "<span class='glyphicon glyphicon-fast-forward' title='LAST' />"
            }
        },
        /* ----------------------------------------------------------------
            V1.10.11 does **NOT** support .done/.fail /.always, so must use 
            deprecated .ajax() API
        */
        ajax: {
            url: configValues.dataUrl,
            type: 'POST',
            headers: configTable.getXsrfToken(),
            error: function (jqXHR, responseText, errorThrown) {
                // explicitly hide on error, or loading element never goes away
                var n = document.querySelector('div.dataTables_processing')
                if (n !== null) n.style.display = 'none';

                configTable.jqModalError(errorThrown);
                console.log(errorThrown);
            },
            complete: function (data, textStatus, jqXHR) {
                configTable.clearCheckAll();
            }
        },
        /* ----------------------------------------------------------------
            first and last columns hard-coded for consistent display:
            -- first: checkboxes => bulk action button(s)
            -- last: single row/record edit/delete
        */
        columnDefs: [{
            targets: 0,
            // TODO: fix search when hidden
            // visible: false,
            searchable: false,
            orderable: false,
            render: function (data, type, full, meta) { return "<input type='checkbox' />"; }
        },
        {
            targets: -1,
            searchable: false,
            orderable: false,
            render: function (data, type, full, meta) {
                return "<span class='glyphicon glyphicon-edit green link-icons'></span>"
                + " <span class='glyphicon glyphicon-remove-circle red link-icons'><span></span></span>";
            }
        }]
    });
    configTable.setTable(table).setConfigValues(configValues).init();
});