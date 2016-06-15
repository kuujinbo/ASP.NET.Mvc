// $.fn.DataTable.ext.pager.numbers_length = 5;
$(document).tooltip({
    open: function (event, ui) {
        $(ui.tooltip).siblings('.ui-tooltip').remove();
    },
    track: true, items: '*:not(.ui-dialog-titlebar-close)'
});

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
    order: [[1, 'asc']],
    // order: [[(configValues.showCheckboxColumn ? 1 : 0), 'asc']],
    language: {
        processing: configTable.getLoadingElement(),
        lengthMenu: 'Show _MENU_',

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
            previous: "<span class='glyphicon glyphicon-chevron-left' title='Previous' />",
            next: "<span class='glyphicon glyphicon-chevron-right'  title='Next' />",
            first: "<span class='glyphicon glyphicon-fast-backward' title='First' />",
            last: "<span class='glyphicon glyphicon-fast-forward' title='Last' />"
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

            configTable.jqModalError(jqXHR.responseJSON || errorThrown);
            console.log(jqXHR.responseJSON || errorThrown);
        },
        complete: function (data, textStatus, jqXHR) {
            configTable.clearCheckAll();
        }
    },
    createdRow: function (row, data, index) {
        for (var i = 0; i < data.length; ++i) {
            if (/^true/i.test(data[i])) {
                console.log('Yes');
                $('td', row).eq(i).html('Yes');
            } else if (/^false$/i.test(data[i])) {
                $('td', row).eq(i).html('No');
            }
        }
    },
    rowCallback: function (row, data, index) {
        //    var color = data[7].toLowerCase() !== 'yellow' ? '#fff' : '#000';
        //    $('td:eq(7)', row).css({
        //        backgroundColor: data[7].toLowerCase(), color: color,
        //        fontWeight: 'bold', opacity: 0.79
        //    });
    },
    columnDefs: [
    {   // checkboxes => bulk action button(s), also holds recordId
        targets: 0,
        visible: configValues.showCheckboxColumn,
        searchable: !configValues.showCheckboxColumn,
        orderable: !configValues.showCheckboxColumn,
        render: function (data, type, full, meta) {
            return "<input type='checkbox' />";
        }
    },
    {   // single row/record edit/delete
        targets: -1,
        searchable: false,
        orderable: false,
        render: function (data, type, row, meta) {
            var title = row[1];

            var infoLink = configTable.getConfigValues().infoRowUrl
                ? "<span class='glyphicon glyphicon-info-sign blue link-icons' data-action='"
                    + configTable.getInfoAction()
                    + "' title='Information'></span>"
                : '';

            var editLink = configTable.getConfigValues().editRowUrl
                ? "<span class='glyphicon glyphicon-edit green link-icons' data-action='"
                    + configTable.getEditAction()
                    + "' title='Edit'></span>"
                : '';

            var deleteLink = configTable.getConfigValues().deleteRowUrl
                ? "<span class='glyphicon glyphicon-remove-circle red link-icons' data-action='"
                    + configTable.getDeleteAction()
                    + "' title='Delete'><span></span></span>"
                : '';

            return [infoLink, editLink, deleteLink].join(' ');
        }
        //render: function (data, type, row, meta) {
        //    var title =  row[1];
        //    return "<span class='glyphicon glyphicon-edit green link-icons' title='Edit ["
        //        + (title || 'this record')
        //        + "]'></span>"
        //        + " <span class='glyphicon glyphicon-remove-circle red link-icons' title='Delete ["
        //        + (title || 'this record')
        //        + "]'><span></span></span>";
        //}
    }]
});

configTable.setTable(table).setConfigValues(configValues).init();