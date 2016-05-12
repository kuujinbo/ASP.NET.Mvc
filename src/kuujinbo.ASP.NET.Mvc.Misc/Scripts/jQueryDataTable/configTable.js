var configTable = function () {
    var _table;
    var _configValues = {};
    var _xsrf = '__RequestVerificationToken';

    return {
        jqModal: $('#datatable-success-error-modal').dialog({
            autoOpen: false, height: 276, width: 476
        }),
        //jqModal: $('<div></div>').dialog({
        //    autoOpen: false, height: 276, width: 476
        //}),
        jqModalOK: function (msg) {
            var success = 'Request Processed Successfully';
            var html = "<h1><span class='glyphicon glyphicon-ok green'></span></h1>"
                + '<div>' + (msg || success) + '</div>';
            configTable.jqModal.html(html)
                .dialog({ title: success })
                .dialog('open');
        },
        jqModalError: function (msg) {
            var err = 'Error Processing Your Request'
            var html = "<h1><span class='glyphicon glyphicon-flag red'></span></h1>"
                + '<div>' + (msg || err) + '</div>';
            configTable.jqModal.html(html)
                .dialog({ title: err })
                .dialog('open');
        },
        /* -----------------------------------------------------------------
            selectors and DOM elements
        */
        getTableId: function () { return '#jquery-data-table'; },
        getCheckAllId: function () { return '#datatable-check-all'; },
        setTable: function (table) {
            _table = table;
            return this;
        },
        getConfigValues: function () { return _configValues; },
        setConfigValues: function (config) {
            _configValues = config;
            return this;
        },
        getLoadingElement: function () {
            return "<h1 class='dataTablesLoading'>"
                + 'Loading data'
                + " <span class='glyphicon glyphicon-refresh spin-infinite' />"
                + '</h1>';
        },
        getSpinClasses: function () {
            return 'glyphicon glyphicon-refresh spin-infinite'.split(/\s+/);
        },
        getSelectedRowClass: function () {
            return 'datatable-select-row';
        },
        getInvalidUrlMessage: function () {
            return '<h2>Invalid URL</h2>Please contact the application administrators.';
        },
        getActionButtonSelector: function () { return '#data-table-actions button.btn'; },
        getSearchBoxSelector: function () { return 'tfoot input[type=text]'; },
        getCheckedSelector: function () { return 'input[type="checkbox"]:checked'; },
        getUncheckedSelector: function () { return 'input[type="checkbox"]:not(:checked)'; },
        /* -----------------------------------------------------------------
            DataTables wrappers
        ----------------------------------------------------------------- */
        clearSearchColumns: function () { _table.search('').columns().search(''); },
        draw: function () { _table.draw(false); },
        drawAndGoToPage1: function () { _table.draw(); },
        getDataColumn: function () { return _table.columns()[0].length - 1; },
        getRowData: function (row) {
            return _table.row(row).data()[configTable.getDataColumn()];
        },
        reload: function () { _table.ajax.reload(); },
        setSearchColumn: function (element) {
            _table.column(element.dataset.columnNumber).search(element.value);
        },
        /* -----------------------------------------------------------------
            helper functions
        ----------------------------------------------------------------- */
        clearCheckAll: function () {
            // ajax call only updates tbody
            var n = document.querySelector(configTable.getCheckAllId());
            if (n !== null) n.checked = false;
        },
        clearSearchBoxes: function () {
            var elements = document.querySelectorAll(
                configTable.getSearchBoxSelector()
            );
            for (i = 0; i < elements.length; ++i) elements[i].value = '';

            configTable.clearSearchColumns();
        },
        getSelectedRowIds: function () {
            var selectedIds = [];
            _table.rows().every(function (rowIdx, tableLoop, rowLoop) {
                var cb = this.node()
                    .querySelector(configTable.getCheckedSelector());

                if (cb !== null && cb.checked) selectedIds.push(this.data()[configTable.getDataColumn()]);
            });
            return selectedIds;
        },
        getXsrfToken: function () {
            var token = document.querySelector('input[name=' + _xsrf + ']');
            if (token !== null) {
                var xsrf = {};
                xsrf[_xsrf] = token.value;
                return xsrf;
            }
            return null;
        },
        redirect: function (url) {
            document.location.href = url;
        },
        search: function () {
            var searchCount = 0;
            var elements = document.querySelectorAll('input[type=text]');
            for (i = 0; i < elements.length; ++i) {
                var searchText = elements[i].value;
                // search only if non-whitespace
                if (searchText !== '' && !/^\s+$/.test(searchText)) {
                    ++searchCount;
                    configTable.setSearchColumn(elements[i]);
                }
                    /* explicitly clear individual input, or will save last value 
                       if user backspaces.
                    */
                else {
                    elements[i].value = '';
                    configTable.setSearchColumn(elements[i]);
                }
            }
            if (searchCount > 0) {
                configTable.clearCheckAll();
                configTable.drawAndGoToPage1();
            }
        },
        showSpin: function (element, doAdd) {
            var span = element.querySelector('span');
            if (span) {
                if (doAdd) {
                    configTable.getSpinClasses()
                        .forEach(function (i) { span.classList.add(i) });
                }
                else {
                    configTable.getSpinClasses()
                        .forEach(function (i) { span.classList.remove(i) });
                }
            }
        },
        sendXhr: function (element, url, data) {
            configTable.showSpin(element, true);
            $.ajax({
                url: url,
                headers: configTable.getXsrfToken(),
                data: data ? data : null,
                type: 'POST'
            })
            .done(function (data, textStatus, jqXHR) {
                configTable.jqModalOK(data);

                // redisplay UI on row delete
                if (url === _configValues.deleteRowUrl) {
                    configTable.draw();
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                configTable.jqModalError(jqXHR.data);
            })
            .always(function () {
                configTable.showSpin(element)
            });
        },
        /* -----------------------------------------------------------------
            event listeners
        ----------------------------------------------------------------- */
        clickActionButton: function (e) {
            e.preventDefault();
            var target = e.target;
            var url = target.dataset.url;

            if (url) {
                var ids = configTable.getSelectedRowIds();
                if (ids.length > 0) {
                    configTable.sendXhr(target, url, { ids: ids });
                } else {
                    configTable.jqModalError(
                        '<h2>No Records Selected</h2>'
                        + 'Select one or more records to process the '
                        + (target.textContent || 'selected')
                        + ' action.'
                    );
                }
            }
            else {
                configTable.jqModalError(configTable.getInvalidUrlMessage());
            }

            return false;
        },
        clickCheckAll: function (e) {
            if (e.target.checked) {
                var elements = document.querySelectorAll(
                    configTable.getUncheckedSelector()
                );
                for (i = 0; i < elements.length; ++i) elements[i].click();
            } else {
                var elements = document.querySelectorAll(
                    configTable.getCheckedSelector()
                );
                for (i = 0; i < elements.length; ++i) elements[i].click();
            }
        },
        // search icons in <span>
        clickSearch: function (e) {
            var target = e.target;
            if (target.classList.contains('glyphicon-search')) {
                configTable.search();
            }
            else if (target.classList.contains('glyphicon-repeat')) {
                configTable.clearSearchBoxes();
                configTable.reload();
            }
        },
        clickTable: function (e) {
            var target = e.target;
            // single checkbox click
            if (target.type === 'checkbox') {
                var row = target.parentNode.parentNode;
                if (row && row.tagName.toLowerCase() === 'tr') {
                    var cl = configTable.getSelectedRowClass();
                    if (target.checked) {
                        row.classList.add(cl);
                    } else {
                        row.classList.remove(cl);
                    }
                }
            }
                // edit & delete links
            else if (target.tagName.toLowerCase() === 'span'
            && target.classList.contains('glyphicon')) {
                var row = target.parentNode.parentNode;
                if (target.classList.contains('glyphicon-remove-circle')) {
                    // delete record from dataset...
                    configTable.sendXhr(
                        target,
                        configTable.getConfigValues().deleteRowUrl,
                        { id: configTable.getRowData(row) }
                    );

                    configTable.clearCheckAll();
                }
                else if (target.classList.contains('glyphicon-edit')) {
                    configTable.redirect(
                        configTable.getConfigValues().editRowUrl
                        + '/'
                        + configTable.getRowData(row)
                    );
                }
            }
        },
        // search when ENTER key pressed in <input> text
        keyupSearch: function (e) {
            if (e.which === 13) configTable.search();
        },
        /* -----------------------------------------------------------------
            initialize DataTable and event listeners
        ----------------------------------------------------------------- */
        init: function () {
            var tableId = configTable.getTableId();

            // allow ENTER in search boxes, otherwise possible form submit
            document.onkeypress = function (e) {
                if ((e.which === 13) && (e.target.type === 'text')) { return false; }
            };

            // action buttons
            var buttons = document.querySelectorAll(
                configTable.getActionButtonSelector()
            );
            for (i = 0 ; i < buttons.length ; i++) {
                buttons[i].addEventListener('click', configTable.clickActionButton, false);
            }

            // 'check all' checkbox
            var checkAll = document.querySelector(configTable.getCheckAllId());
            if (checkAll != null) checkAll.addEventListener('click', configTable.clickCheckAll, false);

            // datatable clicks
            var clickTable = document.querySelector(tableId);
            if (clickTable != null) clickTable.addEventListener('click', configTable.clickTable, false);

            // inject search icons & add event listeners
            var footers = document.querySelectorAll(tableId + ' tfoot th');
            footers[footers.length - 1].innerHTML =
                "<span class='search-icons glyphicon glyphicon-search' title='Search'></span>"
                + "<span class='search-icons glyphicon glyphicon-repeat title='Clear Search'></span>";
            var searchIcons = document.querySelectorAll('tfoot span.search-icons');
            for (var i = 0; i < searchIcons.length; i++) {
                searchIcons[i].addEventListener('click', configTable.clickSearch, false);
            }

            /* ---------------------------------------------------------------
                first column checkbox, last column edit & delete icon links
                -- inject search textboxes
                -- add event listeners to perform search on ENTER key press
            */
            var footerSearchBoxes = document.querySelectorAll(tableId + ' tfoot th[data-is-searchable]');
            for (var i = 0; i < footerSearchBoxes.length; i++) {
                if (!footerSearchBoxes[i].dataset.isSearchable) continue;

                footerSearchBoxes[i].innerHTML =
                    "<input style='width:100% !important;display: block !important;'"
                    + " data-column-number='" + i + "'"
                    + " class='form-control' type='text' placeholder='Search' />";

                footerSearchBoxes[i]
                    .addEventListener('keyup', configTable.keyupSearch, false);
            }
        }
    }
}();