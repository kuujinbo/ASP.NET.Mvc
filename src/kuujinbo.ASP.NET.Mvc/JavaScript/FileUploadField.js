function FileUploadField() {
    Object.defineProperty(this, 'maxSizeExceeded', {
        value: 'Max File Upload Size Exceeded'
    });
}

FileUploadField.prototype = {
    constructor: FileUploadField,
    toMB: function(sizeInBytes) {
        var multiplier = 1024;
        var i = Math.floor(Math.log(sizeInBytes) / Math.log(multiplier))
        return parseFloat((sizeInBytes / Math.pow(multiplier, i)).toFixed(2)) + " MB";
    },
    clearUpload: function(e) {
        this.clearUploadUpateDom(e.target);
    },
    clearUploadUpateDom: function(button) {
        button.parentNode.parentNode.firstElementChild.firstElementChild.firstElementChild.value = '';
        button.parentNode.previousElementSibling.value = '';
        button.parentNode.style.display = 'none';
    },
    processFile: function(e) {
        var inputFile = e.target;
        var files = this.processFileGetFiles(inputFile);
        if (files && files.length > 0) {
            var file = files[0];
            var maxuploadSize = inputFile.dataset.maxSize * 1024; // convert from KB
            if (maxuploadSize >= file.size) {
                this.processFileUpdateDom(inputFile, file.name);
            } else {
                var errorLines = [
                    '[' + file.name + '] is ' + this.toMB(file.size)
                    , 'Maximum allowed file upload size is ' + this.toMB(maxuploadSize)
                    , 'Please select another file.'
                ];
                if (typeof jQuery !== 'undefined'
                    && typeof jQuery.ui !== 'undefined'
                    && typeof jQuery.ui.dialog === 'function') {
                    var error = "<h1><span style='color:red' class='glyphicon glyphicon-flag'></span>"
                        + this.maxSizeExceeded + '</h1>';
                    for (var i = 0; i < errorLines.length; ++i) {
                        error += '<p>' + errorLines[i] + '</p>'
                    }
                    this.showDialog(error);
                } else {
                    alert(errorLines.join('\n\n'));
                }

                // **MUST** explicitly clear file inputFile
                inputFile.value = '';
            }
        }
    },
    showDialog: function(message) {
        $('<div></div>').html(message)
        .dialog({
            width: 'auto', modal: true, title: this.maxSizeExceeded
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

        var file = document.querySelector('input[type=file]');
        if (file != null) {
            file.addEventListener('change', this.processFile.bind(this), false);
        }
    }
}