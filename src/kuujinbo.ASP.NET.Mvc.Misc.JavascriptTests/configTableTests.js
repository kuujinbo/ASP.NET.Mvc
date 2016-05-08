/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/lib/DataTables/jquery.dataTables.js" />
/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/lib/DataTables/dataTables.bootstrap.js" />
/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/lib/jquery-ui-1.11.4.js" />
/// <reference path="./../../src/kuujinbo.ASP.NET.Mvc.Misc/Scripts/jQueryDataTable/configTable.js" />
'use strict';

describe('configTable', function() {
    beforeEach(function() {
        // jasmine.Ajax.install();
        var configValues = {
            dataUrl: '/',
            deleteRowUrl: '/delete',
            editRowUrl: '/edit',
            allowMultiColumnSorting: true
        };
        configTable.setConfigValues(configValues);
        // spyOn(xhrButtonService, 'validateBeforeSend').and.callThrough();
    });

    /* ===================================================================
    some tests manilupate the DOM and must be reset, so other tests 
    are not affected.
    =================================================================== */
    function removeTemplateChildren(template) {
        while (template.hasChildNodes())
            template.removeChild(template.lastChild);
    }

    describe('selectors and DOM', function() {
        it('should TEST', function() {
            expect(configTable).toBeDefined();
            expect(configTable.getTableId()).toEqual('#jquery-data-table');
            // setTable() && setConfigValues() return 'this' to allow chaining
            expect(
                configTable.setTable({}).setConfigValues({})
            ).toEqual(configTable);
            expect(configTable.getLoadingElement()).toEqual(
                "<h1 class='dataTablesLoading'>Loading data "
                + "<span class='glyphicon glyphicon-refresh spin-infinite' /></h1>"
            );
        });
    });
    
    describe('clearCheckAll', function() {
        it('should uncheck checkAll', function() {
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

    describe('clearSearchBoxes', function () {
        var template;
        afterEach(function () {
            removeTemplateChildren(template);
        });

        it('should clear all textbox values', function () {
            spyOn(configTable, 'clearSearchColumns');
            template = document.createElement('tfoot');
            template.innerHTML = "<tr><th><input type='text' value='00' /></th>"
                + "<th><input type='text' value='11' /></th></tr>";
            document.body.appendChild(template);
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
    
    describe('getXsrfToken', function() {
        var template;
        afterEach(function () {
            if (template) removeTemplateChildren(template);
        });

        it('should return null when hidden field not in DOM', function() {
            expect(configTable.getXsrfToken()).toBeNull()
        });

        it('should return token when hidden field in DOM', function() {
            template = document.createElement('div');
            template.innerHTML = "<input name='__RequestVerificationToken' type='hidden' value='XXX' />";
            document.body.appendChild(template);

            var xsrf = configTable.getXsrfToken();
            expect(xsrf).not.toBeNull();
            expect(xsrf.__RequestVerificationToken).toEqual('XXX');
        });
    });

    // add / remove the processing spinner from buttons
    describe('buttonProcessing', function() {
        var template, classes, classString;
        beforeEach(function() {
            template = document.createElement('div');
            classString = 'glyphicon glyphicon-refresh spin-infinite';
            classes = classString.split(/\s+/);
        });

        it('should add the expected classes', function() {
            template.innerHTML = '<span></span>';
            configTable.buttonProcessing(template, true);
            var span = template.firstChild;

            expect(classes.length).toEqual(3);
            for (var i = 0; i < classes.length; ++i) {
                expect(span.classList.contains(classes[i])).toEqual(true);
            }
        });

        it('should remove the expected classes', function() {
            template.innerHTML = '<span class="' + classString + '"></span>';
            configTable.buttonProcessing(template);
            var span = template.firstChild;

            for (var i = 0; i < classes.length; ++i) {
                expect(span.classList.contains(classes[i])).toEqual(false);
            }
        });
    });

    describe('sendXhr', function() {
        var deferred, element;
        beforeEach(function() {
            deferred = new jQuery.Deferred();
            element = document.createElement('div');
            spyOn(jQuery, 'ajax').and.returnValue(deferred);
            spyOn(configTable, 'buttonProcessing');
            configTable.sendXhr(element, '/', '');
        });

        it('should call jQuery.ajax()', function() {
            var expectedArgs = {
                url: '/', headers: null, data: null, type: 'POST'
            };

            expect(jQuery.ajax.calls.count()).toEqual(1);
            expect(jQuery.ajax).toHaveBeenCalledWith(expectedArgs);
        });

        it('should call buttonProcessing before sending the XHR', function() {
            // mock XHR has **NOT** returned
            expect(deferred.state()).toEqual("pending");
            expect(configTable.buttonProcessing.calls.count()).toEqual(1);
            expect(configTable.buttonProcessing).toHaveBeenCalledWith(element, true);
        });

        it('should call jqModalOK and buttonProcessing when promise is fulfilled', function () {
            var httpResponseMsg = 'HTTP response success';
            spyOn(configTable, 'jqModalOK');

            deferred.resolve(httpResponseMsg);

            expect(configTable.jqModalOK.calls.count()).toEqual(1);
            expect(configTable.jqModalOK).toHaveBeenCalledWith(httpResponseMsg);
            expect(configTable.buttonProcessing.calls.count()).toEqual(2);
            // ajax.always()
            expect(configTable.buttonProcessing).toHaveBeenCalledWith(element);
        });

        it('should call jqModalError and buttonProcessing when promise is rejected', function () {
            var httpResponseMsg = 'HTTP response error';
            spyOn(configTable, 'jqModalError');
            var jqXHR = { data: httpResponseMsg }

            deferred.reject(jqXHR);

            expect(configTable.jqModalError.calls.count()).toEqual(1);
            expect(configTable.jqModalError).toHaveBeenCalledWith(httpResponseMsg);
            expect(configTable.buttonProcessing.calls.count()).toEqual(2);
            // ajax.always()
            expect(configTable.buttonProcessing).toHaveBeenCalledWith(element);
        });
    });

    describe('actionButtonClick', function () {
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

            var result = configTable.actionButtonClick(event);

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

            var result = configTable.actionButtonClick(event);

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

            var result = configTable.actionButtonClick(event);

            expect(result).toEqual(false);
            expect(event.preventDefault).toHaveBeenCalled();
            expect(configTable.getSelectedRowIds).toHaveBeenCalled();
            expect(configTable.sendXhr).toHaveBeenCalledWith(
                event.target, '/action', { ids: [1, 2] }
            );
        });
    });

    describe('searchIconsClick', function () {
        var template, event;
        beforeEach(function () {
            template = document.createElement('div');
            event = {};
        });

        it('should search when icon is clicked', function () {
            spyOn(configTable, 'doSearch');
            template.innerHTML = "<span class='search-icons glyphicon glyphicon-search' title='Search'></span>";
            event.target = template.firstChild;

            configTable.searchIconsClick(event);
            expect(configTable.doSearch).toHaveBeenCalled();
        });

        it('should clear the search and reload data when icon is clicked', function () {
            spyOn(configTable, 'clearSearchBoxes');
            spyOn(configTable, 'reload');
            template.innerHTML = "<span class='search-icons glyphicon glyphicon-repeat title='Clear Search'></span>";
            event.target = template.firstChild;

            configTable.searchIconsClick(event);
            expect(configTable.clearSearchBoxes).toHaveBeenCalled();
            expect(configTable.reload).toHaveBeenCalled();
        });
    });

    describe('searchBoxKeyup', function () {
        var event;
        beforeEach(function () {
            spyOn(configTable, 'doSearch');
            event = {};
        });

        it('should search when KeyboardEvent.which is carriage return', function () {
            event.which = 13;

            configTable.searchBoxKeyup(event);
            expect(configTable.doSearch).toHaveBeenCalled();
        });

        it('should not search when KeyboardEvent.which is not carriage return', function () {
            event.which = 0;

            configTable.searchBoxKeyup(event);
            expect(configTable.doSearch).not.toHaveBeenCalled();
        });
    });
});