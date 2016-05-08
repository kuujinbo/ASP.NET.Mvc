var configTable = function() {
    var _table;
    var _configValues = {};
    var _tableId = '#jquery-data-table';
    var _checkAllId = '#datatable-check-all';
    var _searchBoxSelector = 'tfoot input[type=text]';
    var _selectRowClass = 'datatable-select-row';
    var _actionButtonSelector = '#data-table-actions button.btn';
    var _buttonSpin = 'glyphicon glyphicon-refresh spin-infinite'.split(/\s+/);
    var _xsrf = '__RequestVerificationToken';

    return {
        jqModal: $('#datatable-success-error-modal').dialog({
            autoOpen: false, height: 276, width: 476
        }),
        jqModalOK: function(msg) {
            var success = 'Request Processed Successfully';
            var html = "<h1><span class='glyphicon glyphicon-ok green'></span></h1>"
                + '<div>' + (msg || success) + '</div>';
            configTable.jqModal.html(html)
                .dialog({title: success})
                .dialog('open');

        },
        jqModalError: function(msg) {
            var err = 'Error Processing Your Request'
            var html = "<h1><span class='glyphicon glyphicon-flag red'></span></h1>"
                + '<div>' + (msg || err) + '</div>';
            configTable.jqModal.html(html)
                .dialog({title: err})
                .dialog('open');
        },
        /* -----------------------------------------------------------------
            selectors and DOM elements
        */
        getTableId: function() { return _tableId; },
        getCheckAllId: function() { return _checkAllId; },
        setTable: function(table) {
            _table = table;
            return this;
        },
        setConfigValues: function(config) {
            _configValues = config;
            return this;
        },
        getLoadingElement: function() {
            return "<h1 class='dataTablesLoading'>Loading data <span class='glyphicon glyphicon-refresh spin-infinite' /></h1>";
        },
        getInvalidUrlMessage: function() {
            return '<h2>Invalid URL</h2>Please contact the application administrators.';
        },
        getSearchBoxSelector: function() { return _searchBoxSelector; },
        getCheckedSelector: function() { return 'input[type="checkbox"]:checked'; },
        getUncheckedSelector: function() { return 'input[type="checkbox"]:not(:checked)'; },
        /* -----------------------------------------------------------------
            helper functions
        */
        reload: function() { _table.ajax.reload(); },
        clearCheckAll: function() {
            // ajax call only updates tbody
            var n = document.querySelector(_checkAllId);
            if (n !== null) n.checked = false;
        },
        clearSearchColumns: function() { _table.search('').columns().search(''); },
        clearSearchBoxes: function () {
            var nodes = document.querySelectorAll(_searchBoxSelector);
            for (i = 0; i < nodes.length; ++i) nodes[i].value = '';

            configTable.clearSearchColumns();
        },
        search: function() {
            var searchCount = 0;
            var nodes = document.querySelectorAll('input[type=text]');
            for (i = 0; i < nodes.length; ++i) {
                var searchText = nodes[i].value;
                // search only if non-whitespace
                if (searchText !== '' && !/^\s+$/.test(searchText)) {
                    ++searchCount;
                    _table.column(nodes[i].dataset.columnNumber).search(searchText);
                }
                /* explicitly clear individual input, or will save last value 
                   if user backspaces.
                */
                else {
                    nodes[i].value = '';
                    _table.column(nodes[i].dataset.columnNumber).search('');
                }
            }
            if (searchCount > 0) {
                configTable.clearCheckAll();
                _table.draw();
            }
        },
        getSelectedRowIds: function() {
            var selectedIds = [];
            _table.rows().every(function(rowIdx, tableLoop, rowLoop) {
                var cb = this.node().querySelector(_checkboxChecked);

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
        showSpin: function(element, doAdd) {
            var span = element.querySelector('span');
            if (span) {
                if (doAdd) {
                    _buttonSpin.forEach(function(i) { span.classList.add(i) });
                }
                else {
                    _buttonSpin.forEach(function(i) { span.classList.remove(i) });
                }
            }
        },
        sendXhr: function(element, url, data) {
            configTable.showSpin(element, true);
            $.ajax({
                url: url,
                headers: configTable.getXsrfToken(),
                data: data ? data : null,
                type: 'POST'
            })
            .done(function(data, textStatus, jqXHR) {
                configTable.jqModalOK(data);

                // redisplay UI on row delete
                if (url === _configValues.deleteRowUrl) {
                    _table.draw(false);
                }
            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                configTable.jqModalError(jqXHR.data);
            })
            // http://api.jquery.com/deferred.always/
            .always(function() { 
                configTable.showSpin(element)
            });
        },
        /* -----------------------------------------------------------------
            event listeners
        */
        clickActionButton: function(e) {
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
        clickCheckAll: function(e) {
            if (e.target.checked) {
                var nodes = document.querySelectorAll(
                    configTable.getUncheckedSelector()
                );
                for (i = 0; i < nodes.length; ++i) nodes[i].click();
            } else {
                var nodes = document.querySelectorAll(
                    configTable.getCheckedSelector()
                );
                for (i = 0; i < nodes.length; ++i) nodes[i].click();
            }
        },
        // search icons in <span>
        clickSearch: function(e) {
            var target = e.target;
            if (target.classList.contains('glyphicon-search')) {
                configTable.search();
            }
            else if (target.classList.contains('glyphicon-repeat')) {
                configTable.clearSearchBoxes();
                configTable.reload();
            }
        },
        // search when ENTER key pressed in <input> textbox
        keyupSearch: function(e) {
            if (e.which === 13) configTable.search();
        },
        clickTable: function(e) {
            var target = e.target;
            // single checkbox click
            if (target.type === 'checkbox') {
                var row = target.parentNode.parentNode;
                if (row && row.tagName.toLowerCase() === 'tr') {
                    if (target.checked) {
                        row.classList.add(_selectRowClass);
                    } else {
                        row.classList.remove(_selectRowClass);
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
                        target, _configValues.deleteRowUrl, { id: _table.row(row).data()[0] }
                    );

                    configTable.clearCheckAll();
                }
                else if (target.classList.contains('glyphicon-edit')) {
                    document.location.href = _configValues.editRowUrl
                        + '/'
                        + _table.row(row).data()[0];
                }
            }
        },

        /* -----------------------------------------------------------------
            initialize DataTable and event listeners
        */
        init: function() {
            // allow ENTER in search boxes, otherwise possible form submit
            document.onkeypress = function(e) {
                if ((e.which === 13) && (e.target.type === 'text')) { return false; }
            };

            // action buttons
            var buttons = document.querySelectorAll(_actionButtonSelector);
            for (i = 0 ; i < buttons.length ; i++) {
                buttons[i].addEventListener('click', configTable.clickActionButton, false);
            }

            // 'check all' checkbox
            var checkAll = document.querySelector(_checkAllId);
            if (checkAll != null) checkAll.addEventListener('click', configTable.clickCheckAll, false);

            // datatable clicks
            var clickTable = document.querySelector(_tableId);
            if (clickTable != null) clickTable.addEventListener('click', configTable.clickTable, false);

            // inject search icons & add event listeners
            var footers = document.querySelectorAll(_tableId + ' tfoot th');
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
            var footerSearchBoxes = document.querySelectorAll(_tableId + ' tfoot th');
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