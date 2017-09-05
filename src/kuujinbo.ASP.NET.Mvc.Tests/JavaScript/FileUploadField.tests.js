/// <reference path="./../../../src/kuujinbo.ASP.NET.Mvc/JavaScript/FileUploadField.js" />

'use strict';

describe('FileUploadField', function() {
    var fileUploadField, inputFile, button, MB4, MB10;
    beforeEach(function() {
        MB4 = 4194304;
        MB10 = 10485760;
        setFixtures(getFixture());
        inputFile = document.querySelector('input[type=file]');
        button = document.querySelector('button.fileUploadFieldButton');
        fileUploadField = new FileUploadField();
    });

    // **MUST** update if kuujinbo.ASP.NET.Mvc.FileUploadFieldExtension.cs changes
    function getFixture() {
        return "<div class='input-group input-group-sm'><span class='input-group-btn'>"
            + "<label class='btn btn-success' type='button'>"
            + "<input id='fileUploadField' name='fileUploadField' type='file' style='display:none;' data-max-size='4096' />Browse...."
            + "</label></span>"
            + "<input tabindex='-1' style='pointer-events:none;background-color:#eee' type='text' required class='form-control' />"
            + "<span class='input-group-btn' style='display:none;'>"
            + "<button class='fileUploadFieldButton btn btn-danger' type='button'><span class='glyphicon glyphicon-remove'></span></button>"
            + "</span></div>"
    }

    describe('DOM manipulation', function() {
        it('adds event listeners when calling addListeners()', function() {
            spyOn(fileUploadField, 'clearUpload');
            spyOn(fileUploadField, 'processFile');

            fileUploadField.addListeners();
            button.dispatchEvent(new Event('click'));
            inputFile.dispatchEvent(new Event('change'));

            expect(fileUploadField.clearUpload).toHaveBeenCalledTimes(1);
            expect(fileUploadField.clearUpload).toHaveBeenCalledWith(jasmine.any(Event));
            expect(fileUploadField.processFile).toHaveBeenCalledTimes(1);
            expect(fileUploadField.clearUpload).toHaveBeenCalledWith(jasmine.any(Event));
        });

        describe('clearUpload', function() {
            it('updates the DOM when the button is clicked', function() {
                spyOn(fileUploadField, 'clearUpload').and.callThrough();
                spyOn(fileUploadField, 'clearUploadUpateDom');

                fileUploadField.addListeners();
                button.dispatchEvent(new Event('click'));

                expect(fileUploadField.clearUpload).toHaveBeenCalledTimes(1);
                expect(fileUploadField.clearUploadUpateDom).toHaveBeenCalledTimes(1);
                expect(fileUploadField.clearUploadUpateDom).toHaveBeenCalledWith(button);
            });
        });

        describe('processFile', function() {
            it('is a no-op when no files are selected', function() {
                spyOn(window, 'alert');
                spyOn(fileUploadField, 'toMB').and.callThrough();
                spyOn(fileUploadField, 'processFileGetFiles');

                fileUploadField.addListeners();
                inputFile.dispatchEvent(new Event('change'));

                expect(fileUploadField.processFileGetFiles).toHaveBeenCalledTimes(1);
                expect(fileUploadField.processFileGetFiles).toHaveBeenCalledWith(inputFile);
                expect(fileUploadField.toMB).not.toHaveBeenCalled();
                expect(window.alert).not.toHaveBeenCalled();
            });

            it('shows error message and clears input[type=file].value when file size exceeds max', function() {
                spyOn(window, 'alert');
                spyOn(fileUploadField, 'toMB').and.callThrough();

                var filename = "test.txt";
                spyOn(fileUploadField, 'processFileGetFiles').and.returnValue([{
                    name: filename, size: MB10
                }]);

                fileUploadField.addListeners();
                inputFile.dispatchEvent(new Event('change'));

                expect(fileUploadField.processFileGetFiles).toHaveBeenCalledTimes(1);
                expect(fileUploadField.processFileGetFiles).toHaveBeenCalledWith(inputFile);
                expect(fileUploadField.toMB).toHaveBeenCalledTimes(2);
                // max upload size - getFixture()
                expect(fileUploadField.toMB).toHaveBeenCalledWith(MB4);
                // uploaded file size
                expect(fileUploadField.toMB).toHaveBeenCalledWith(MB10);
                expect(window.alert).toHaveBeenCalledTimes(1);
                expect(window.alert.calls.mostRecent().args[0]).toMatch(/10 MB/);
                expect(window.alert.calls.mostRecent().args[0]).toMatch(new RegExp(filename));
                expect(inputFile.value).toBe('');
            });

            it('shows jQuery UI dialog error message when jQuery UI is available', function() {
                spyOn(window, 'alert');
                spyOn(fileUploadField, 'toMB').and.callThrough();
                spyOn(fileUploadField, 'showDialog').and.callThrough;
                window.jQuery = {
                    ui: {
                        dialog: function () { console.log('stub jQuery.ui.dialog'); }
                    }
                }
                var filename = "test.jpg";
                spyOn(fileUploadField, 'processFileGetFiles').and.returnValue([{
                    name: filename, size: MB10
                }]);

                fileUploadField.addListeners();
                inputFile.dispatchEvent(new Event('change'));


                expect(fileUploadField.processFileGetFiles).toHaveBeenCalledTimes(1);
                expect(fileUploadField.processFileGetFiles).toHaveBeenCalledWith(inputFile);
                expect(fileUploadField.toMB).toHaveBeenCalledTimes(2);
                // max upload size - getFixture()
                expect(fileUploadField.toMB).toHaveBeenCalledWith(MB4);
                // uploaded file size
                expect(fileUploadField.toMB).toHaveBeenCalledWith(MB10);
                expect(window.alert).toHaveBeenCalledTimes(0);
                expect(fileUploadField.showDialog).toHaveBeenCalledTimes(1);
            });

            it('updates the DOM when file size is valid', function() {
                spyOn(fileUploadField, 'processFileUpdateDom');
                spyOn(window, 'alert');
                spyOn(fileUploadField, 'toMB').and.callThrough();

                var filename = "test.jpg";
                spyOn(fileUploadField, 'processFileGetFiles').and.returnValue([{
                    name: filename, size: MB4
                }]);

                fileUploadField.addListeners();
                inputFile.dispatchEvent(new Event('change'));

                expect(fileUploadField.processFileGetFiles).toHaveBeenCalledTimes(1);
                expect(fileUploadField.processFileGetFiles).toHaveBeenCalledWith(inputFile);
                expect(fileUploadField.processFileUpdateDom).toHaveBeenCalledTimes(1);
                expect(fileUploadField.processFileUpdateDom).toHaveBeenCalledWith(inputFile, filename);
                expect(fileUploadField.toMB).not.toHaveBeenCalled();
                expect(window.alert).not.toHaveBeenCalled();
                expect(inputFile.value).toBe('');
            });
        });
    });

    describe('toMB', function() {
        it('returns a string formatted in MB when a number of bytes is passed in', function() {
            var bytes = 4194304;
            var result = fileUploadField.toMB(bytes);

            expect(result).toBe('4 MB');
        });
    });
});