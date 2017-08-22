function toMB(bytes) {
    var multiplier = 1024;
    var i = Math.floor(Math.log(bytes) / Math.log(multiplier))
    return parseFloat((bytes / Math.pow(multiplier, i)).toFixed(2)) + " MB";
}

function fileUpload(upload) {
    if (upload.files.length > 0) {
        var file = upload.files[0];
        var maxuploadSize = upload.dataset.maxSize * 1024; // convert from KB
        if (maxuploadSize >= file.size) {
            filename = upload.parentNode.parentNode.nextElementSibling;
            filename.value = file.name;
            filename.nextElementSibling.style.display = 'block';
        } else {
            alert(
                '[' + file.name + '] is ' + toMB(file.size) + '\n\n'
                + 'Maximum allowed upload size is ' + toMB(maxuploadSize) + '.\n\n'
                + 'Please select another file.'
            );
            // must explicitly clear file upload
            upload.value = '';
        }
    }
}
function clearUpload(button) {
    button.parentNode.parentNode.firstElementChild.firstElementChild.firstElementChild.value = '';
    button.parentNode.previousElementSibling.value = '';
    button.parentNode.style.display = 'none';
}