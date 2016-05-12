$(document).ready(function () {
    // $.fn.DataTable.ext.pager.numbers_length = 5;

    var columnDefinitions = [{
        targets: -1,
        searchable: false,
        orderable: false,
        // https://datatables.net/reference/option/columns.render
        render: function (data, type, row, meta) {
            console.log(row);
            return "<span class='glyphicon glyphicon-edit green link-icons'></span>"
            + " <span class='glyphicon glyphicon-remove-circle red link-icons'><span></span></span>";
        }
    }];

    if (configValues.showCheckboxColumn) {
        columnDefinitions.push({
            targets: 0,
            // or asc/desc icon shown in thead
            orderable: false,
            render: function (data, type, full, meta) { return "<input type='checkbox' />"; }
        });
    };

    // DataTables API instance => $().DataTable() - note CASE
    var table = $(configTable.getTableId()).DataTable({
        processing: true,
        serverSide: true,
        deferRender: true,
        // stateSave: true,
        // true by default, allow  shift-click multiple column sorting
        // orderMulti: configValues.allowMultiColumnSorting,
        orderMulti: true,
        dom: "<'row'<'col-xs-6'li><'col-xs-6'p>>" +
            "<'row'<'col-xs-12'tr>>" +
            "<'row'<'col-xs-6'li><'col-xs-6'p>>",
        pagingType: 'full_numbers',
        // autoWidth: true,
        // order: [[1, 'asc']],
        // order: [[(configValues.showCheckboxColumn ? 1 : 0), 'asc']],
        language: {
            processing: configTable.getLoadingElement(),
            lengthMenu: 'Show _MENU_ per page.',

            //lengthMenu: 'Show <select>' +
            //      '<option value="5">5</option>'+
            //      '<option value="10">10</option>'+
            //      '<option value="20">20</option>'+
            //      '<option value="40">40</option>'+
            //      '<option value="50">50</option>'+
            //      '<option value="-1">All</option>'+
            //      '</select> per page.',

            info: '_START_ to _END_ of _TOTAL_ results',
            infoFiltered: '(<em>filtered from _MAX_ total</em>)',
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
            data: { checkColumn: configValues.showCheckboxColumn },
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
        columnDefs: columnDefinitions
    });

    configTable.setTable(table).setConfigValues(configValues).init();
});