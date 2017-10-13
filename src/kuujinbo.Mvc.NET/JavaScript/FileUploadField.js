function FileUploadField() {
    Object.defineProperty(this, 'maxSizeExceeded', {
        value: 'Max File Upload Size Exceeded'
    });
    Object.defineProperty(this, 'invalidFile', {
        value: 'Invalid File'
    });

    this._jQueryUI = typeof jQuery !== 'undefined'
            && typeof jQuery.ui !== 'undefined'
            && typeof jQuery.ui.dialog === 'function';
}

FileUploadField.prototype = {
    constructor: FileUploadField,
    toMB: function(sizeInBytes) {
        var multiplier = 1024;
        var i = Math.floor(Math.log(sizeInBytes) / Math.log(multiplier))
        return parseFloat((sizeInBytes / Math.pow(multiplier, i)).toFixed(2)) + " MB";
    },
    fileInputSelector: function() {
        return document.querySelector('input[type=file][data-max-size]');
    },
    clearUpload: function(e) {
        this.clearUploadUpateDom(e.target);
    },
    clearUploadUpateDom: function(button) {
        button.parentNode.parentNode.firstElementChild.firstElementChild.firstElementChild.value = '';
        button.parentNode.previousElementSibling.value = '';
        button.parentNode.style.display = 'none';
    },
    validateFile: function (inputFile, file) {
        var maxuploadSize = inputFile.dataset.maxSize * 1024; // convert from KB
        if (maxuploadSize < file.size) {
            var errorLines = [
                '[' + file.name + '] is ' + this.toMB(file.size)
                , 'Maximum allowed file upload size is ' + this.toMB(maxuploadSize)
                , 'Please select another file.'
            ];
            return { title: this.maxSizeExceeded, errorLines: errorLines };
        } else {
            var accept = inputFile.getAttribute('accept');
            if (accept) {
                var extension = ('.' + file.name.split('.').pop()).toLowerCase();
                var accepted = accept.toLowerCase().split(',');
                // IE sucks - no support for new Set(iterable)
                var testSet = new Set();
                for (var i = 0; i < accepted.length; ++i) { testSet.add(accepted[i]); }
                if (!testSet.has(extension)) {
                    var errorLines = [
                        '[' + file.name + '] is not an allowed file type.'
                        , 'Only files with the following extensions are allowed ['
                                + accept
                                + ']'
                        , 'Please select another file.'
                    ];
                    return { title: this.invalidFile, errorLines: errorLines };
                }
            }
        }
        
        return;
    },
    processFile: function(e) {
        var inputFile = e.target;
        var files = this.processFileGetFiles(inputFile);
        if (files && files.length > 0) {
            var file = files[0];

            var validationError = this.validateFile(inputFile, file);
            if (!validationError) {
                this.processFileUpdateDom(inputFile, file.name);
            } else {
                var errorLines = validationError.errorLines;
                if (this._jQueryUI) {
                    var error = '';
                    for (var i = 0; i < errorLines.length; ++i) {
                        error += '<p>' + errorLines[i] + '</p>'
                    }
                    this.showDialog(validationError.title, error);
                } else {
                    alert(errorLines.join('\n\n'));
                }

                files = null;
                // **MUST** explicitly clear file inputFile
                inputFile.value = null;
            }
        }
    },
    showDialog: function(title, message) {
        message = "<h1><span style='color:red' class='glyphicon glyphicon-flag'></span>"
                  + title
                  + '</h1>'
                  + message;
        $('<div></div>').html(message)
        .dialog({
            width: 'auto', modal: true, title: title
        });
    },
    processFileGetFiles: function(inputFile) {
        return inputFile.files;
    },
    processFileUpdateDom: function(inputFile, filename) {
        var inputText = inputFile.parentNode.parentNode.nextElementSibling;
        inputText.value = filename;
        inputText.nextElementSibling.style.display = 'block';
    },
    addListeners: function() {
        var button = document.querySelector('button.fileUploadFieldButton');
        if (button != null) {
            button.addEventListener('click', this.clearUpload.bind(this), false);
        }

        var fileInput = this.fileInputSelector();
        if (fileInput != null) {
            fileInput.addEventListener('change', this.processFile.bind(this), false);
        }
    }
}