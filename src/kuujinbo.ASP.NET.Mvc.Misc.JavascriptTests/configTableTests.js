/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/lib/DataTables/jquery.dataTables.js" />
/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/lib/DataTables/dataTables.bootstrap.js" />
/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/lib/jquery-ui-1.11.4.js" />
/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/jQueryDataTable/configTable.js" />
'use strict';

describe('configTable', function () {
    beforeEach(function () {
        // jasmine.Ajax.install();
        var configValues = {
            dataUrl: '/',
            deleteRowUrl: '/delete',
            editRowUrl: '/edit',
            allowMultiColumnSorting: true
        };
        configTable.setConfigValues(configValues);
    });

    /* ===================================================================
    some tests manipulate the DOM and must be reset, so other tests 
    are not affected.
    =================================================================== */
    function removeTemplateChildren(template) {
        while (template.hasChildNodes())
            template.removeChild(template.lastChild);
    }

    describe('selectors and DOM', function () {
        it('should initialize the table objects', function () {
            expect(configTable).toBeDefined();
            expect(configTable.getTableId()).toEqual('#jquery-data-table');
            expect(configTable.getCheckAllId()).toEqual('#datatable-check-all');

            // setTable() && setConfigValues() return 'this' to allow chaining
            expect(
                configTable.setTable({}).setConfigValues({})
            ).toEqual(configTable);

            expect(configTable.getLoadingElement()).toEqual(
                "<h1 class='dataTablesLoading'>Loading data "
                + "<span class='glyphicon glyphicon-refresh spin-infinite' /></h1>"
            );
        });

        it('should have the correct CSS selectors', function () {
            expect(configTable.getActionButtonSelector()).toEqual('#data-table-actions button.btn');
            expect(configTable.getSearchBoxSelector()).toEqual('tfoot input[type=text]');
            expect(configTable.getCheckedSelector()).toEqual('input[type="checkbox"]:checked');
            expect(configTable.getUncheckedSelector()).toEqual('input[type="checkbox"]:not(:checked)');
        });

        it('should have the correct class names', function () {
            expect(configTable.getSelectedRowClass()).toEqual('datatable-select-row');

            var spinClasses = configTable.getSpinClasses();
            expect(spinClasses.length).toEqual(3);
            expect(spinClasses[0]).toEqual('glyphicon');
            expect(spinClasses[1]).toEqual('glyphicon-refresh');
            expect(spinClasses[2]).toEqual('spin-infinite');
        });
    });

    describe('clearCheckAll', function () {
        it('should uncheck checkAll', function () {
            var template = document.createElement('div');
            template.innerHTML = '<input id="'
                + configTable.getCheckAllId()
                + '" type="checkbox" />';
            document.body.appendChild(template);
            var checkbox = template.firstChild;

            configTable.clearCheckAll();

            expect(checkbox.checked).toEqual(false);
        });
    });

    // jasmine-jquery
    describe('clearSearchBoxes', function () {
        it('should clear all textbox values', function () {
            setFixtures(
                '<tfoot><tr>'
                + "<th><input type='text' value='00' /></th>"
                + "<th><input type='text' value='11' /></th>"
                + '</tr></tfoot>'
            );
            spyOn(configTable, 'clearSearchColumns');

            var textboxes = document.querySelectorAll(
                configTable.getSearchBoxSelector()
            );
            configTable.clearSearchBoxes();

            expect(textboxes.length).toEqual(2);
            expect(textboxes[0].value).toEqual('');
            expect(textboxes[1].value).toEqual('');
            expect(configTable.clearSearchColumns).toHaveBeenCalled();
        });
    });

    // jasmine-jquery
    describe('getXsrfToken', function () {
        it('should return null when hidden field not in DOM', function () {
            expect(configTable.getXsrfToken()).toBeNull()
        });

        it('should return token when hidden field in DOM', function () {
            setFixtures(
                "<input name='__RequestVerificationToken' type='hidden' value='XXX' />"
            );

            var xsrf = configTable.getXsrfToken();

            expect(xsrf).not.toBeNull();
            expect(xsrf.__RequestVerificationToken).toEqual('XXX');
        });
    });

    // jasmine-jquery
    describe('search', function () {
        beforeEach(function () {
            spyOn(configTable, 'setSearchColumn');
            spyOn(configTable, 'drawAndGoToPage1');
        });


        it('should not search when textboxes are empty or whitespace', function () {
            var textboxes = setFixtures(
                "<input type='text' placeholder='Search' data-column-number='1' />"
                + "<input type='text' placeholder='Search' data-column-number='2' value='   ' />"
            );
            var resultTextboxes = textboxes.find('input');

            configTable.search();

            expect(configTable.setSearchColumn.calls.count()).toEqual(2);
            expect(configTable.drawAndGoToPage1.calls.count()).toEqual(0);
            expect(resultTextboxes.length).toEqual(2);
            expect(resultTextboxes[0].value).toEqual('');
            expect(resultTextboxes[1].value).toEqual('');
        });

        it('should search when any textbox is not empty or whitespace', function () {
            spyOn(configTable, 'clearCheckAll');

            var textboxes = setFixtures(
                "<input type='text' placeholder='Search' data-column-number='1' />"
                + "<input type='text' placeholder='Search' data-column-number='2' value='   ' />"
                + "<input type='text' placeholder='Search' data-column-number='3' value='03' />"
                + "<input type='text' placeholder='Search' data-column-number='4' value='04' />"
            );
            var resultTextboxes = textboxes.find('input');

            configTable.search();

            expect(configTable.setSearchColumn.calls.count()).toEqual(4);
            expect(configTable.clearCheckAll.calls.count()).toEqual(1);
            expect(configTable.drawAndGoToPage1.calls.count()).toEqual(1);
            expect(resultTextboxes.length).toEqual(4);
            expect(resultTextboxes[0].value).toEqual('');
            expect(resultTextboxes[1].value).toEqual('');
            expect(resultTextboxes[2].value).toEqual('03');
            expect(resultTextboxes[3].value).toEqual('04');
        });
    });

    // add / remove processing spinner (jasmine-jquery)
    describe('showSpin', function () {
        var spinClasses;
        beforeEach(function () {
            spinClasses = configTable.getSpinClasses();
        });

        it('should add the expected spin classes', function () {
            var container = setFixtures('<div><span></span></div>');
            configTable.showSpin(container[0], true);
            var span = container.find('span').first();

            expect(spinClasses.length).toEqual(3);
            for (var i = 0; i < spinClasses.length; ++i) {
                expect(span.hasClass(spinClasses[i])).toEqual(true);
            }
        });

        it('should remove the expected spin classes', function () {
            var container = setFixtures(
                '<div><span class="' + spinClasses.join(' ') + '"></span></div>'
            );
            configTable.showSpin(container[0]);
            var span = container.find('span').first();

            for (var i = 0; i < spinClasses.length; ++i) {
                expect(span.hasClass(spinClasses[i])).toEqual(false);
            }
        });
    });

    describe('sendXhr', function () {
        var deferred, element;
        beforeEach(function () {
            deferred = new jQuery.Deferred();
            element = document.createElement('div');
            spyOn(jQuery, 'ajax').and.returnValue(deferred);
            spyOn(configTable, 'showSpin');
            spyOn(configTable, 'getXsrfToken');
            configTable.sendXhr(element, '/', '');
        });

        it('should call jQuery.ajax()', function () {
            var expectedArgs = {
                url: '/', headers: undefined, data: null, type: 'POST'
            };

            expect(jQuery.ajax.calls.count()).toEqual(1);
            expect(jQuery.ajax).toHaveBeenCalledWith(expectedArgs);
        });

        it('should call showSpin before sending the XHR', function () {
            // mock XHR has **NOT** returned
            expect(deferred.state()).toEqual("pending");
            expect(configTable.showSpin.calls.count()).toEqual(1);
            expect(configTable.showSpin).toHaveBeenCalledWith(element, true);
            expect(configTable.getXsrfToken).toHaveBeenCalled();
        });

        it('should call jqModalOK and showSpin when promise is fulfilled', function () {
            var httpResponseMsg = 'HTTP response success';
            spyOn(configTable, 'jqModalOK');

            deferred.resolve(httpResponseMsg);

            expect(configTable.jqModalOK.calls.count()).toEqual(1);
            expect(configTable.jqModalOK).toHaveBeenCalledWith(httpResponseMsg);
            expect(configTable.showSpin.calls.count()).toEqual(2);
            expect(configTable.getXsrfToken).toHaveBeenCalled();
            // ajax.always()
            expect(configTable.showSpin).toHaveBeenCalledWith(element);
        });

        it('should call jqModalError and showSpin when promise is rejected', function () {
            var httpResponseMsg = 'HTTP response error';
            spyOn(configTable, 'jqModalError');
            var jqXHR = { data: httpResponseMsg }

            deferred.reject(jqXHR);

            expect(configTable.jqModalError.calls.count()).toEqual(1);
            expect(configTable.jqModalError).toHaveBeenCalledWith(httpResponseMsg);
            expect(configTable.showSpin.calls.count()).toEqual(2);
            expect(configTable.getXsrfToken).toHaveBeenCalled();
            // ajax.always()
            expect(configTable.showSpin).toHaveBeenCalledWith(element);
        });
    });

    /* ========================================================================
       event listeners
       ========================================================================
    */
    describe('clickActionButton', function () {
        var template, event;
        beforeEach(function () {
            template = document.createElement('div');
            event = {
                preventDefault: jasmine.createSpy()
            };
            spyOn(configTable, 'sendXhr');
        });

        it('should be an error when the button does not have a data URL', function () {
            spyOn(configTable, 'getSelectedRowIds')
            spyOn(configTable, 'jqModalError');
            template.innerHTML = '<button class="btn btn-primary">Batch Update<span></span></button>';
            event.target = template.firstChild;

            var result = configTable.clickActionButton(event);

            expect(result).toEqual(false);
            expect(event.preventDefault).toHaveBeenCalled();
            expect(configTable.getSelectedRowIds).not.toHaveBeenCalled();
            expect(configTable.sendXhr).not.toHaveBeenCalled();
            expect(configTable.jqModalError).toHaveBeenCalledWith(
                configTable.getInvalidUrlMessage()
            );
        });

        it('should be an error when no rows are selected', function () {
            spyOn(configTable, 'getSelectedRowIds').and.returnValue([]);
            spyOn(configTable, 'jqModalError');
            template.innerHTML = '<button class="btn btn-primary" data-url="/action">Batch Update<span></span></button>';
            event.target = template.firstChild;

            var result = configTable.clickActionButton(event);

            expect(result).toEqual(false);
            expect(event.preventDefault).toHaveBeenCalled();
            expect(configTable.getSelectedRowIds).toHaveBeenCalled();
            expect(configTable.sendXhr).not.toHaveBeenCalled();
            expect(configTable.jqModalError.calls.mostRecent().args[0]).toMatch('<h2>No Records Selected</h2>');
        });

        it('should send XHR when rows are selected', function () {
            spyOn(configTable, 'getSelectedRowIds').and.returnValue([1, 2]);
            spyOn(configTable, 'jqModalOK');
            template.innerHTML = '<button class="btn btn-primary" data-url="/action">Batch Update<span></span></button>';
            event.target = template.firstChild;

            var result = configTable.clickActionButton(event);

            expect(result).toEqual(false);
            expect(event.preventDefault).toHaveBeenCalled();
            expect(configTable.getSelectedRowIds).toHaveBeenCalled();
            expect(configTable.sendXhr).toHaveBeenCalledWith(
                event.target, '/action', { ids: [1, 2] }
            );
        });
    });

    // click 'datatable-check-all' checkbox - [un]check all checkboxes 
    // (jasmine-jquery)
    describe('clickCheckAll', function () {
        var event, checked, unchecked;
        beforeEach(function () {
            event = {};
            checked = configTable.getCheckedSelector();
            unchecked = configTable.getUncheckedSelector();
            setFixtures('<div>'
                + "<input type='checkbox' />"
                + "<input type='checkbox' checked='checked' />"
                + '</div>'
            );
            spyOnEvent(checked, 'click');
            spyOnEvent(unchecked, 'click');
        });

        it('should only fire click event on checked checkboxes when clickAll is unchecked', function () {
            event.target = $("<input id='datatable-check-all' type='checkbox' />")[0];

            configTable.clickCheckAll(event);

            expect('click').toHaveBeenTriggeredOn(checked);
            expect('click').not.toHaveBeenTriggeredOn(unchecked);
        });

        // jQuery object => native DOM element - index [0]
        it('should only fire click event on unchecked checkboxes when clickAll is checked', function () {
            event.target = $("<input id='datatable-check-all' type='checkbox' checked='checked' />")[0];

            configTable.clickCheckAll(event);

            expect('click').not.toHaveBeenTriggeredOn(checked);
            expect('click').toHaveBeenTriggeredOn(unchecked);
        });
    });

    describe('clickSearch', function () {
        var template, event;
        beforeEach(function () {
            template = document.createElement('div');
            event = {};
        });

        it('should search when icon is clicked', function () {
            spyOn(configTable, 'search');
            template.innerHTML = "<span class='search-icons glyphicon glyphicon-search' title='Search'></span>";
            event.target = template.firstChild;

            configTable.clickSearch(event);
            expect(configTable.search).toHaveBeenCalled();
        });

        it('should clear the search and reload data when icon is clicked', function () {
            spyOn(configTable, 'clearSearchBoxes');
            spyOn(configTable, 'reload');
            template.innerHTML = "<span class='search-icons glyphicon glyphicon-repeat title='Clear Search'></span>";
            event.target = template.firstChild;

            configTable.clickSearch(event);
            expect(configTable.clearSearchBoxes).toHaveBeenCalled();
            expect(configTable.reload).toHaveBeenCalled();
        });
    });

    // (jasmine-jquery)
    describe('clickTable link', function () {
        var event, config, html, row, span, recordId;
        beforeEach(function () {
            event = {};
            config = configTable.getConfigValues();
            html = setFixtures('<tr><td>'
                + "<span class='glyphicon glyphicon-edit green link-icons'></span>"
                + "<span class='glyphicon glyphicon-remove-circle red link-icons'><span></span></span>"
                + '</td></tr>'
            );
            row = html.find('tr').first();
            recordId = 1;
            spyOn(configTable, 'getRowData').and.returnValue(recordId);
            spyOn(configTable, 'getConfigValues').and.returnValue(config);
        });

        it('should delete the selected record', function () {
            spyOn(configTable, 'sendXhr');
            spyOn(configTable, 'clearCheckAll');

            span = html.find('span.red');
            // jQuery object => native DOM element - index [0]
            event.target = span[0];
            configTable.clickTable(event);

            expect(event.target.tagName.toLowerCase()).toEqual('span');
            expect(configTable.sendXhr).toHaveBeenCalledWith(
                span[0], config.deleteRowUrl, { id: recordId }
            );
            expect(configTable.getConfigValues).toHaveBeenCalled();
            expect(configTable.getRowData).toHaveBeenCalledWith(row[0]);
            expect(configTable.clearCheckAll).toHaveBeenCalled();
        });

        it('should redirect to the edit page', function () {
            spyOn(configTable, 'redirect');

            span = html.find('span.green');
            event.target = span[0];
            configTable.clickTable(event);

            expect(event.target.tagName.toLowerCase()).toEqual('span');
            expect(configTable.getConfigValues).toHaveBeenCalled();
            expect(configTable.getRowData).toHaveBeenCalledWith(row[0]);
            expect(configTable.redirect).toHaveBeenCalledWith(
                config.editRowUrl + '/' + recordId
            );
        });
    });

    // (jasmine-jquery)
    describe('clickTable checkbox', function () {
        var event, selectedClass;
        beforeEach(function () {
            event = {};
            selectedClass = configTable.getSelectedRowClass();
            spyOn(configTable, 'getSelectedRowClass')
                .and.returnValue(selectedClass);
        });

        it('should add selected row class when checkbox is checked', function () {
            var html = setFixtures('<tr>'
                + "<td><input type='checkbox' checked='checked' /></td>"
                + "<td></td>"
                + '</tr>'
            );
            var row = html.find('tr').first();
            var checkbox = html.find('input[type="checkbox"]:checked');
            // jQuery object => native DOM element - index [0]
            event.target = checkbox[0];

            configTable.clickTable(event);

            expect(event.target.type).toEqual('checkbox');
            expect(event.target.checked).toEqual(true);
            expect(configTable.getSelectedRowClass).toHaveBeenCalled();
            expect(row.hasClass(selectedClass)).toEqual(true);
        });

        it('should remove selected row class when checkbox is not', function () {
            var html = setFixtures('<tr class="' + selectedClass + '">'
                + "<td><input type='checkbox' /></td>"
                + "<td></td>"
                + '</tr>'
            );
            var row = html.find('tr').first();
            var checkbox = html.find('input[type="checkbox"]:not(:checked)');
            event.target = checkbox[0];

            configTable.clickTable(event);

            expect(event.target.type).toEqual('checkbox');
            expect(event.target.checked).toEqual(false);
            expect(configTable.getSelectedRowClass).toHaveBeenCalled();
            expect(row.attr('class')).toEqual('');
            expect(row.hasClass(selectedClass)).toEqual(false);
        });
    });

    describe('keyupSearch', function () {
        var event;
        beforeEach(function () {
            spyOn(configTable, 'search');
            event = {};
        });

        it('should search when KeyboardEvent.which is carriage return', function () {
            event.which = 13;

            configTable.keyupSearch(event);
            expect(configTable.search).toHaveBeenCalled();
        });

        it('should not search when KeyboardEvent.which is not carriage return', function () {
            event.which = 0;

            configTable.keyupSearch(event);
            expect(configTable.search).not.toHaveBeenCalled();
        });
    });
});