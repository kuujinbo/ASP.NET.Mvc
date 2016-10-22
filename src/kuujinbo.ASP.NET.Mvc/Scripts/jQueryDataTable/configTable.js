﻿var configTable = function() {
    var _table;
    var _configValues = {};
    var _infoEditDelete = '';
    // MS @Html.AntiForgeryToken() **IGNORES** HTML4 naming standards:
    // https://www.w3.org/TR/html4/types.html#type-id ('name' token)
    var _xsrf = '__RequestVerificationToken';
    var _idNo = 0;

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
                }
                else {
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
                }
                else {
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
            }
            else if (target.classList.contains('glyphicon-repeat')) {
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
            UI widget for multi-value column search terms
        ----------------------------------------------------------------- */
        getNewWidgetId: function() {
            return 'columnFilterId__' + _idNo++;
        },
        // store widget id => multiple widgets in DOM
        getWidgetIdName: function() { return '_widgetIdName_'; },
        // store selectable filter values
        getFilterStringArrayName: function() { return '_filterStringArrayName_'; },
        // store column search term input field selector
        getFilterSelectorName: function() { return '_filterSelectorName_'; },

        addColumnFilterInput: function(selector, stringArray) {
            var el = document.querySelector(selector);
            if (el !== null
                && stringArray !== null
                && Array.isArray(stringArray)
                && stringArray.length > 0) {
                // non-typed languages are great - create properties on the fly
                var newId = configTable.getNewWidgetId();
                el[configTable.getWidgetIdName()] = newId;
                el[configTable.getFilterStringArrayName()] = stringArray;
                el[configTable.getFilterSelectorName()] = selector;

                el.addEventListener('focus', configTable.enterColumnFilterInput, false);
                el.addEventListener('blur', configTable.leaveColumnFilterInput, false);
            }
        },
        enterColumnFilterInput: function(e) {
            var target = e.target;
            var multiValueId = target[configTable.getWidgetIdName()];

            if (document.querySelector('#' + multiValueId) !== null) return;

            var values = target[configTable.getFilterStringArrayName()];
            var inner = '';
            values.forEach(function(item) {
                inner += "<div class='columnFilterBox'>" + item + '</div>';
            });

            inner += "<input type='button' style='margin:8px;' value='Select' />";
            var div = document.createElement('div');
            div.innerHTML = inner;

            var selectList = div.children;
            for (var i = 0; i < selectList.length - 1; i++) {
                selectList[i].addEventListener(
                    'click', configTable.swapSelectedColumnFilterValue, false
                );
            }
            // can't use negative index
            selectList[selectList.length - 1].addEventListener(
                'click', configTable.columnFilterButtonClick, false
            );
            selectList[selectList.length - 1][configTable.getFilterSelectorName()] =
                target[configTable.getFilterSelectorName()];

            configTable.styleMultiFilterWidget(div, target.getBoundingClientRect());
            div.id = multiValueId;
            document.body.appendChild(div);
        },
        leaveColumnFilterInput: function(e) {
            var multiValueId = e.target[configTable.getWidgetIdName()];
            var focusEl = document.activeElement;
            console.log(focusEl.tagName);

            if (// DOM standard: https://developer.mozilla.org/en-US/docs/Web/API/Document/activeElement
                focusEl === null
                // as usual IE is **NON-COMPLIANT**
                || focusEl.id === multiValueId
                || focusEl.parentElement.id === multiValueId && focusEl.type !== 'button'
                || focusEl.tagName.toLowerCase() === 'body'
                )
            { return; }

            configTable.cleanUpWidget(multiValueId);
        },
        cleanUpWidget: function(selector) {
            console.log('in cleanup');
            // clean up all references
            var el = document.querySelector('#' + selector);
            if (el !== null) {
                el[configTable.getWidgetIdName()] = null;
                el[configTable.getFilterStringArrayName()] = null;
                el[configTable.getFilterSelectorName()] = null;

                var selectList = el.children;
                // compliant browsers handle when element removed from DOM 
                // below; IE doesn't qualify as standards compliant....
                for (var i = 0; i < selectList.length - 1; i++) {
                    selectList[i].removeEventListener(
                        'click', configTable.swapSelectedColumnFilterValue, false
                    );
                }
                selectList[selectList.length - 1].removeEventListener(
                    'click', configTable.columnFilterButtonClick, false
                );

                el.parentNode.removeChild(el);
            }
        },
        columnFilterButtonClick: function(e) {
            var target = e.target;
            var filterField = document.querySelector(target[configTable.getFilterSelectorName()]);
            if (filterField !== null) {
                var el = document.querySelector('#' + filterField[configTable.getWidgetIdName()]);
                if (el !== null) {
                    var values = [];
                    var childEls = el.children;
                    for (var i = 0; i < childEls.length - 1; i++) {
                        if (childEls[i].classList.contains('dataTableSelected')) {
                            values.push(childEls[i].textContent);
                        }
                    }
                    if (values.length > 0) {
                        filterField.value = values.join('|');
                    }
                }

                configTable.cleanUpWidget(filterField[configTable.getWidgetIdName()]);
            }
        },
        styleMultiFilterWidget: function(div, rect) {
            div.style.position = 'absolute';
            div.style.zIndex = '88888888';
            div.style.backgroundColor = '#fff';
            div.style.border = '1px ridge #eaeaea';
            div.style.fontSize = '0.8em';
            div.style.margin = '0';
            div.style.padding = '0';
            div.style.left = rect.left + 'px';
            div.style.top = rect.bottom + 'px';
            div.style.minWidth = rect.width + 'px';
        },
        swapSelectedColumnFilterValue: function(e) {
            e.target.classList.toggle('dataTableSelected');
        }
    }
}();