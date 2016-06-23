﻿var configTable = function() {
    var _table;
    var _configValues = {};
    var _infoEditDelete = '';
    var _xsrf = '__RequestVerificationToken';

    return {
        jqPartialViewModal: $('#datatable-partial-modal').dialog({
            autoOpen: false, width: 'auto'
            // DO NOT SET THE FOLLOWING; IE's focus() is HORRIBLY broken
            //, modal: true
        }),
        jqPartialViewModalOK: function (html, title) {
            configTable.jqPartialViewModal.html(html)
                .dialog({ title: title })
                .dialog('open');
        },
        jqBulkActionModal: $('#datatable-success-error-modal').dialog({
            autoOpen: false, height: 276, width: 476
        }),
        jqModalOK: function (msg) {
            var success = 'Request Processed Successfully';
            var html = "<h1><span class='glyphicon glyphicon-ok green'></span></h1>"
                + '<div>' + (msg || success) + '</div>';
            configTable.jqBulkActionModal.html(html)
                .dialog({ title: success })
                .dialog('open');
        },
        jqModalError: function (msg) {
            var err = 'Error Processing Your Request'
            var html = "<h1><span class='glyphicon glyphicon-flag red'></span></h1>"
                + '<div>' + (msg || err) + '</div>';
            configTable.jqBulkActionModal.html(html)
                .dialog({ title: err })
                .dialog('open');
        },
        /* -----------------------------------------------------------------
            selectors and DOM elements
        */
        getTableId: function() { return '#jquery-data-table'; },
        getCheckAllId: function() { return '#datatable-check-all'; },
        setTable: function (table) {
            _table = table;
            return this;
        },
        getConfigValues: function() { return _configValues; },
        setConfigValues: function (config) {
            _configValues = config;
            // reset InfoEditDelete link cache
            _infoEditDelete = null;
            return this;
        },
        getLoadingElement: function() {
            return "<h1 class='dataTablesLoading'>"
                + 'Loading data'
                + " <span class='glyphicon glyphicon-refresh spin-infinite' />"
                + '</h1>';
        },
        getSpinClasses: function() {
            return 'glyphicon glyphicon-refresh spin-infinite'.split(/\s+/);
        },
        getSelectedRowClass: function() {
            return 'datatable-select-row';
        },
        getInvalidUrlMessage: function() {
            return '<h2>Invalid URL</h2>Please contact the application administrators.';
        },
        getActionButtonSelector: function() { return '#data-table-actions button.btn'; },
        getSearchFilterSelector: function() { return 'th input[type=text], th select'; },
        getCheckedSelector: function() { return 'input[type="checkbox"]:checked'; },
        getUncheckedSelector: function() { return 'input[type="checkbox"]:not(:checked)'; },

        getInfoAction: function() { return 'info'; },
        getEditAction: function() { return 'edit'; },
        getDeleteAction: function() { return 'delete'; },
        getInfoEditDelete: function() {
            // calculate once then cache value
            if (_infoEditDelete) return _infoEditDelete;

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

            _infoEditDelete = [infoLink, editLink, deleteLink].join(' ').trim();
            return _infoEditDelete;
        },
        /* -----------------------------------------------------------------
            DataTables wrappers
        ----------------------------------------------------------------- */
        clearSearchColumns: function() { _table.search('').columns().search(''); },
        draw: function() { _table.draw(false); },
        drawAndGoToPage1: function() { _table.draw(); },
        getRowData: function (row) {
            return _table.row(row).data()[0];
        },
        reload: function() { _table.ajax.reload(); },
        setSearchColumn: function (element) {
            _table.column(element.dataset.columnNumber).search(element.value);
        },
        /* -----------------------------------------------------------------
            helper functions
        ----------------------------------------------------------------- */
        clearCheckAll: function() {
            // ajax call only updates tbody
            var n = document.querySelector(configTable.getCheckAllId());
            if (n !== null) n.checked = false;
        },
        clearSearchFilters: function() {
            var selector = configTable.getSearchFilterSelector()
            var elements = document.querySelectorAll(selector);
            for (i = 0; i < elements.length; ++i) elements[i].value = '';

            configTable.clearSearchColumns();
        },
        getSelectedRowIds: function() {
            var selectedIds = [];
            _table.rows().every(function (rowIdx, tableLoop, rowLoop) {
                var cb = this.node()
                    .querySelector(configTable.getCheckedSelector());

                if (cb !== null && cb.checked) selectedIds.push(this.data()[0]);
            });
            return selectedIds;
        },
        getXsrfToken: function() {
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
        search: function() {
            var searchCount = 0;
            var elements = document.querySelectorAll(configTable.getSearchFilterSelector());
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
        sendXhr: function (element, url, requestData, requestType) {
            configTable.showSpin(element, true);
            $.ajax({
                url: url,
                headers: configTable.getXsrfToken(),
                data: requestData, // bulk action button => record Id array
                type: requestType || 'POST'
            })
            .done(function (data, textStatus, jqXHR) {
                if (requestData !== null) {
                    configTable.draw();
                    configTable.jqModalOK(data);
                }
                else {
                    configTable.jqPartialViewModalOK(
                        data,               // HTML from partial view
                        element.textContent // button text for modal title
                    );
                }
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                configTable.jqModalError(
                    jqXHR.statusText || jqXHR.responseJSON
                );
            })
            .always(function() {
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
            var isModal = target.hasAttribute('data-modal');

            if (url) {
                if (isModal) {
                    configTable.sendXhr(target, url, null, 'GET');
                }
                else {
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
                for (i = 0; i < elements.length; ++i) elements[i].checked = true;
            } else {
                var elements = document.querySelectorAll(
                    configTable.getCheckedSelector()
                );
                for (i = 0; i < elements.length; ++i) elements[i].checked = false;
            }
        },
        // search icons in <span>
        clickSearch: function (e) {
            var target = e.target;
            if (target.classList.contains('glyphicon-search')) {
                configTable.search();
            }
            else if (target.classList.contains('glyphicon-repeat')) {
                configTable.clearSearchFilters();
                configTable.reload();
            }
        },
        clickTable: function (e) {
            var target = e.target;
            var action = target.dataset.action;

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
            // info, edit, & delete links
            else if (action) {
                var row = target.parentNode.parentNode;
                if (action === configTable.getDeleteAction()) {
                    // delete record from dataset...
                    configTable.sendXhr(
                        target,
                        configTable.getConfigValues().deleteRowUrl,
                        { id: configTable.getRowData(row) }
                    );
                    configTable.clearCheckAll();
                }
                else if (action === configTable.getEditAction()) {
                    configTable.redirect(
                        configTable.getConfigValues().editRowUrl
                        + '/'
                        + configTable.getRowData(row)
                    );
                }
                else if (action === configTable.getInfoAction()) {
                    configTable.redirect(
                        configTable.getConfigValues().infoRowUrl
                        + '/'
                        + configTable.getRowData(row)
                    );
                }
            }
        },
        // search when ENTER key pressed in <input> text
        keyupSearch: function (e) {
            if (e.key === 'Enter') configTable.search();
        },
        addListeners: function (tableId) {
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

            // search icons
            var searchIcons = document.querySelectorAll('tfoot span.search-icons');
            for (var i = 0; i < searchIcons.length; i++) {
                searchIcons[i].addEventListener('click', configTable.clickSearch, false);
            }

            // search input fields
            var footerSearchBoxes = document.querySelectorAll(tableId + ' tfoot input[type=text]');
            for (var i = 0; i < footerSearchBoxes.length; i++) {
                footerSearchBoxes[i]
                    .addEventListener('keyup', configTable.keyupSearch, false);
            }
        },
        init: function() {
            var tableId = configTable.getTableId();

            configTable.addListeners(tableId);
        }
    }
}();