function FileUploadField() { }

FileUploadField.prototype = {
    constructor: FileUploadField,
    toMB: function(sizeInBytes) {
        var multiplier = 1024;
        var i = Math.floor(Math.log(sizeInBytes) / Math.log(multiplier))
        return parseFloat((sizeInBytes / Math.pow(multiplier, i)).toFixed(2)) + " MB";
    },
    clearUpload: function (e) {
        this.clearUploadUpateDom(e.target);
    },
    clearUploadUpateDom: function (button) {
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
                alert(
                    '[' + file.name + '] is ' + this.toMB(file.size) + '\n\n'
                    + 'Maximum allowed inputFile size is ' + this.toMB(maxuploadSize) + '.\n\n'
                    + 'Please select another file.'
                );
                // **MUST** explicitly clear file inputFile
                inputFile.value = '';
            }
        }
    },
    processFileGetFiles: function (inputFile) {
        return inputFile.files;
    },
    processFileUpdateDom: function (inputFile, filename) {
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