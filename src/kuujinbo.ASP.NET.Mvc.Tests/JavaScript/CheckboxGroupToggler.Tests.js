/// <reference path="./../../../src/kuujinbo.ASP.NET.Mvc/JavaScript/CheckboxGroupToggler.js" />

'use strict';

describe('CheckboxGroupToggler', function() {
    var toggler, clickEvent;
    var groupSelectorId = 'checkbox-group';
    var selector = '#' + groupSelectorId;

    function getGroupElement() {
        return document.querySelector(selector);
    }

    function getGroupCheckboxes() {
        return getGroupElement().querySelectorAll('input[type=checkbox]');
    }

    beforeEach(function() {
        setFixtures(
            '<div id="' + groupSelectorId + '">'
            + "<input type='checkbox' />"
            + "<input type='checkbox' />"
            + "<input type='checkbox' checked />"
            + '</div>'
        );
        toggler = new CheckboxGroupToggler(selector);
        clickEvent = {};
    });

    describe('CheckboxGroupToggler object', function() {
        it('throws when CSS selector is not passed to new()', function() {
            expect(function() { new CheckboxGroupToggler(); })
                .toThrow(new CheckboxGroupToggler('x').initError);
        });

        it('initializes object when CSS selector passed to new()', function() {
            expect(toggler).toBeDefined();
            expect(toggler.getCheckAllHtml).toEqual(jasmine.any(Function));
            expect(toggler.getUncheckAllHtml).toEqual(jasmine.any(Function));
            expect(toggler.getCheckGroup).toEqual(jasmine.any(Function));
            expect(toggler.addToggleElement).toEqual(jasmine.any(Function));
            expect(toggler.clickToggleElement).toEqual(jasmine.any(Function));
        });
    });

    describe('getCheckGroup', function() {
        it('throws when CSS selector does not match', function() {
            var badToggler = new CheckboxGroupToggler('#does-not-match');

            expect(function() { badToggler.getCheckGroup(); })
                .toThrow(toggler.containerError);
        });

        it('returns Element when CSS selector matches', function() {
            var containerElement = toggler.getCheckGroup();

            expect(containerElement).toEqual(jasmine.any(Element));
        });
    });

    describe('addToggleElement', function() {
        it('adds toggler before by default', function() {
            spyOn(toggler, 'getCheckAllHtml');
            spyOn(toggler, 'getCheckGroup').and.callThrough();

            toggler.addToggleElement();
            var groupElement = getGroupElement().previousSibling;

            expect(toggler.getCheckAllHtml).toHaveBeenCalledTimes(1);
            expect(toggler.getCheckGroup).toHaveBeenCalledTimes(1);
            expect(groupElement.tagName).toBe('SPAN');
            expect(groupElement.children.length).toBe(1);
            expect(groupElement.children[0].tagName).toBe('BUTTON');
            expect(groupElement.children[0].className).toBe(toggler.buttonCheckClassList);
        });

        it('adds toggler before when explicitly called', function() {
            spyOn(toggler, 'getCheckAllHtml');
            spyOn(toggler, 'getCheckGroup').and.callThrough();

            toggler.addToggleElement(true);
            var groupElement = getGroupElement().nextSibling;

            expect(toggler.getCheckAllHtml).toHaveBeenCalledTimes(1);
            expect(toggler.getCheckGroup).toHaveBeenCalledTimes(1);
            expect(groupElement.tagName).toBe('SPAN');
            expect(groupElement.children.length).toBe(1);
            expect(groupElement.children[0].tagName).toBe('BUTTON');
            expect(groupElement.children[0].className).toBe(toggler.buttonCheckClassList);
        });

        it('updates the button and checks all when button has [checked] class', function() {
            spyOn(toggler, 'getUncheckAllHtml');
            toggler.addToggleElement();
            clickEvent.target = getGroupElement().previousSibling.firstChild;

            toggler.clickToggleElement(clickEvent);
            var checkboxes = getGroupElement().querySelectorAll('input[type="checkbox"]:checked');

            expect(clickEvent.target.className).toBe(toggler.buttonUncheckClassList);
            expect(toggler.getUncheckAllHtml).toHaveBeenCalledTimes(1);
            expect(checkboxes.length).toBe(getGroupCheckboxes().length);
        });


        it('updates the button and unchecks all when button does NOT have [checked] class', function() {
            spyOn(toggler, 'getCheckAllHtml');
            toggler.addToggleElement();
            var button = getGroupElement().previousSibling.firstChild;
            button.classList.remove('checked');
            clickEvent.target = button;

            toggler.clickToggleElement(clickEvent);
            var checkboxes = getGroupElement().querySelectorAll('input[type="checkbox"]:not(:checked)');

            expect(button.className).toBe(toggler.buttonCheckClassList);
            // called in addToggleElement() && clickToggleElement()
            expect(toggler.getCheckAllHtml).toHaveBeenCalledTimes(2);
            expect(checkboxes.length).toBe(getGroupCheckboxes().length);
        });

    });

    describe('XXX', function() {
    });
});