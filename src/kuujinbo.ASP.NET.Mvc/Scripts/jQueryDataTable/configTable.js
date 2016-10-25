var configTable = function() {
    var _table;
    var _configValues = {};
    var _infoEditDelete = '';
    // MS @Html.AntiForgeryToken() **IGNORES** HTML4 naming standards:
    // https://www.w3.org/TR/html4/types.html#type-id ('name' token)
    var _xsrf = '__RequestVerificationToken';
    var _idNo = 0;
    // per column multi-value search terms
    var _ieGTE10 = document.documentMode >= 10;

    return {
        jqPartialViewModal: $('#datatable-partial-modal').dialog({
            autoOpen: false, width: 'auto'
            // DO NOT SET THE FOLLOWING; IE's focus() is HORRIBLY broken
            //, modal: true
        }),
        jqPartialViewModalOK: function(html, title) {
            configTable.jqPartialViewModal.html(html)
                .dialog({ title: title })
                .dialog('open');
        },
        jqBulkActionModal: $('#datatable-success-error-modal').dialog({
            autoOpen: false, height: 276, width: 476
        }),
        jqModalOK: function(msg) {
            var success = 'Request Processed Successfully';
            var html = "<h1><span class='glyphicon glyphicon-ok green'></span></h1>"
                + '<div>' + (msg || success) + '</div>';
            configTable.jqBulkActionModal.html(html)
                .dialog({ title: success })
                .dialog('open');
        },
        jqModalError: function(msg) {
            var err = 'Error Processing Your Request'
            var html = "<h1><span class='glyphicon glyphicon-flag red'></span></h1>"
                + '<div>' + (msg || err) + '</div>';
            configTable.jqBulkActionModal.html(html)
                .dialog({ title: err })
                .dialog('open');
        },
        /* -----------------------------------------------------------------
            selectors and DOM elements
        ----------------------------------------------------------------- */
        getTableId: function() { return '#jquery-data-table'; },
        getSaveAsId: function() { return '#datatable-save-as'; },
        getCheckAllId: function() { return '#datatable-check-all'; },
        setTable: function(table) {
            _table = table;
            return this;
        },
        getConfigValues: function() { return _configValues; },
        setConfigValues: function(config) {
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
        getXhrErrorMessage: function() {
            return 'There was a problem processing your request. If the problem continues, please contact the application administrators';
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
        getAjaxParams: function() { return _table.ajax.params(); },
        clearSearchColumns: function() { _table.search('').columns().search(''); },
        draw: function() { _table.draw(false); },
        drawAndGoToPage1: function() { _table.draw(); },
        getRowData: function(row) {
            return _table.row(row).data()[0];
        },
        reload: function() { _table.ajax.reload(); },
        setSearchColumn: function(element) {
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
            _table.rows().every(function(rowIdx, tableLoop, rowLoop) {
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
        redirect: function(url) {
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
        saveAs: function(before, fail, always) {
            var params = configTable.getAjaxParams();
            params.saveAs = true;
            var config = configTable.getConfigValues();
            params.columnNames = JSON.stringify(config.columnNames);

            // return binary content via XHR => see ~/Scripts/jQueryAjax/
            $().downloadFile(
                config.dataUrl,
                params,
                configTable.getXsrfToken(),
                null,
                before, fail, always
            );
        },
        showSpin: function(element, doAdd) {
            var span = element.querySelector('span');
            if (span) {
                if (doAdd) {
                    configTable.getSpinClasses()
                        .forEach(function(i) { span.classList.add(i) });
                } else {
                    configTable.getSpinClasses()
                        .forEach(function(i) { span.classList.remove(i) });
                }
            }
        },
        sendXhr: function(element, url, requestData, requestType) {
            configTable.showSpin(element, true);
            $.ajax({
                url: url,
                headers: configTable.getXsrfToken(),
                data: requestData, // bulk action button => record Id array
                type: requestType || 'POST'
            })
            .done(function(data, textStatus, jqXHR) {
                if (requestData !== null) {
                    configTable.draw();
                    configTable.jqModalOK(data);
                } else {
                    configTable.jqPartialViewModalOK(
                        data,               // HTML from partial view
                        element.textContent // button text for modal title
                    );
                }
            })
            .fail(function(jqXHR, textStatus, errorThrown) {
                configTable.jqModalError(
                    jqXHR.responseJSON
                    || (jqXHR.status !== 500 ? jqXHR.statusText : configTable.getXhrErrorMessage())
                );
            })
            .always(function() {
                configTable.showSpin(element)
            });
        },
        /* -----------------------------------------------------------------
            event listeners
        ----------------------------------------------------------------- */
        clickActionButton: function(e) {
            e.preventDefault();
            var target = e.target;
            var url = target.dataset.url;
            var isModal = target.hasAttribute('data-modal');

            if (url) {
                if (isModal) {
                    configTable.sendXhr(target, url, null, 'GET');
                } else {
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
            } else {
                configTable.jqModalError(configTable.getInvalidUrlMessage());
            }

            return false;
        },
        // send binary content via XHR
        clickSaveAs: function(e) {
            // explicitly show/hide 'processing' element; since XHR is not
            // sent via jQuery DataTables API, need to handle this here
            var before, always;
            var n = document.querySelector('div.dataTables_processing');
            if (n !== null) {
                before = function() { n.style.display = 'block'; }
                always = function() { n.style.display = 'none'; }
            }
            // and handle response errors
            var fail = function(msg) {
                configTable.jqModalError(msg || configTable.getXhrErrorMessage());
            }

            configTable.saveAs(before, fail, always);
        },
        clickCheckAll: function(e) {
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
        clickSearch: function(e) {
            var target = e.target;
            if (target.classList.contains('glyphicon-search')) {
                configTable.search();
            } else if (target.classList.contains('glyphicon-repeat')) {
                configTable.clearSearchFilters();
                configTable.reload();
            }
        },
        clickTable: function(e) {
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
            } else if (action) { // info, edit, & delete links
                var row = target.parentNode.parentNode;
                if (action === configTable.getDeleteAction()) {
                    // delete record from dataset...
                    configTable.sendXhr(
                        target,
                        configTable.getConfigValues().deleteRowUrl,
                        { id: configTable.getRowData(row) }
                    );
                    configTable.clearCheckAll();
                } else if (action === configTable.getEditAction()) {
                    configTable.redirect(
                        configTable.getConfigValues().editRowUrl
                        + '/'
                        + configTable.getRowData(row)
                    );
                } else if (action === configTable.getInfoAction()) {
                    configTable.redirect(
                        configTable.getConfigValues().infoRowUrl
                        + '/'
                        + configTable.getRowData(row)
                    );
                }
            }
        },
        // search when ENTER key pressed in <input> text
        keyupSearch: function(e) {
            if (e.key === 'Enter') configTable.search();
        },
        addListeners: function(tableId) {
            // allow ENTER in search boxes, otherwise possible form submit
            document.onkeypress = function(e) {
                if ((e.which === 13) && (e.target.type === 'text')) { return false; }
            };

            // action buttons
            var buttons = document.querySelectorAll(
                configTable.getActionButtonSelector()
            );
            for (i = 0 ; i < buttons.length ; i++) {
                buttons[i].addEventListener('click', configTable.clickActionButton, false);
            }

            // saveAs button
            var saveAs = document.querySelector(configTable.getSaveAsId());
            if (saveAs != null) {
                saveAs.addEventListener('click', configTable.clickSaveAs, false);
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
        },
        /* -----------------------------------------------------------------
            UI widget for per-column multi-value filter
        ----------------------------------------------------------------- */
        getValuePickerId: function() {
            return 'valuePickerId__' + _idNo++;
        },
        // store widget id => multiple widgets in DOM
        getValuePickerIdName: function() { return '_valuePickerIdName:_'; },
        // store column search term input field selector
        getSearchInputSelectorName: function() { return '_searchInputSelectorName_'; },

        addValuePicker: function(selector, stringArray) {
            var searchInput = document.querySelector(selector);
            if (searchInput !== null
                && stringArray !== null
                && Array.isArray(stringArray)
                && stringArray.length > 0) {
                // non-typed languages are great - create properties on the fly
                var newId = configTable.getValuePickerId();
                searchInput[configTable.getValuePickerIdName()] = newId;
                searchInput[configTable.getSearchInputSelectorName()] = selector;

                searchInput.addEventListener('focus', configTable.enterSearchInput, false);
                searchInput.addEventListener('blur', configTable.leaveSearchInput, false);

                // first 
                var inner = '';
                stringArray.forEach(function(item) {
                    inner += "<div class='valuePickerItem'>" + item + '</div>';
                });

                inner += "<input type='button' style='margin:8px;' value='Add / Clear' />";
                var div = document.createElement('div');
                div.innerHTML = inner;

                var selectList = div.children;
                for (var i = 0; i < selectList.length - 1; i++) {
                    selectList[i].addEventListener(
                        'click', configTable.toggleValuePickerItem, false
                    );
                }
                // can't use negative index
                selectList[selectList.length - 1].addEventListener(
                    'click', configTable.valuePickerButtonClick, false
                );
                selectList[selectList.length - 1][configTable.getSearchInputSelectorName()] =
                    searchInput[configTable.getSearchInputSelectorName()];


                if (_ieGTE10) div.tabIndex = '0';
                div.id = newId;
                div.classList.add('valuePicker');
                div.style.minWidth = searchInput.getBoundingClientRect().width + 'px';
                div.style.display = 'none';
                if (stringArray.length > 10) {
                    div.style.overflowY = 'auto';
                    div.style.height = '276px';
                };
                searchInput.parentNode.appendChild(div);

                div.addEventListener('blur', configTable.leaveSearchInput, false);
            }
        },
        enterSearchInput: function(e) {
            var target = e.target;
            var multiValueId = target[configTable.getValuePickerIdName()];

            var w = document.querySelector('#' + multiValueId);
            if (document.querySelector('#' + multiValueId) !== null) {
                w.style.display = 'block';
            }
        },
        leaveSearchInput: function(e) {
            // https://developer.mozilla.org/en-US/docs/Web/API/Document/activeElement
            // https://developer.mozilla.org/en-US/docs/Web/Events/blur
            // IE10 >= sets document.activeElement to element focus moves to,
            // which allows fine-grained show/hide of the filter widget. all
            // other browsers set document.activeElement to document.body, and
            // must manually close the widget
            if (_ieGTE10) {
                var focusEl = document.activeElement;
                var target = e.target;
                var targetName = target.tagName.toLowerCase();
                var multiValueId = '';
                if (targetName === 'input') {
                    multiValueId = target[configTable.getValuePickerIdName()];
                } else if (targetName === 'div') {
                    multiValueId = target.previousSibling[configTable.getValuePickerIdName()];
                }

                if (multiValueId || focusEl !== null) {
                    if (focusEl.id === multiValueId
                        || focusEl.parentElement.id === multiValueId && focusEl.type !== 'button'
                        || focusEl.parentElement.id === multiValueId && focusEl.type === 'button'
                    ) { return; }
                    configTable.resetValuePicker(multiValueId);
                }
            }
        },
        resetValuePicker: function(selector) {
            var el = document.querySelector('#' + selector);
            if (el !== null) {
                var divChildren = el.children;
                for (var i = 0; i < divChildren.length; ++i) divChildren[i].classList.remove('dataTableSelected');
                el.style.display = 'none';
                return;
            }
        },
        valuePickerButtonClick: function(e) {
            var target = e.target;
            var filterField = document.querySelector(target[configTable.getSearchInputSelectorName()]);
            if (filterField !== null) {
                var valuePickerId = filterField[configTable.getValuePickerIdName()];
                var el = document.querySelector('#' + valuePickerId);
                if (el !== null) {
                    var values = [];
                    var childEls = el.children;
                    for (var i = 0; i < childEls.length - 1; i++) {
                        if (childEls[i].classList.contains('dataTableSelected')) {
                            values.push(childEls[i].textContent);
                        }
                    }
                    filterField.value = values.length > 0
                        ? values.join(configTable.getConfigValues().multiValueFilterSeparator) : '';
                }

                configTable.resetValuePicker(valuePickerId);
            }
        },
        toggleValuePickerItem: function(e) {
            e.target.classList.toggle('dataTableSelected');
            if (_ieGTE10) e.target.parentNode.focus();
        }
    }
}();