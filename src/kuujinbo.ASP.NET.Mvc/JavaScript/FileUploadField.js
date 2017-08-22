function fileUpload(upload) {
    if (upload.files.length > 0) {
        var file = upload.files[0];
        var maxuploadSize = upload.dataset.maxSize;
        if (maxuploadSize >= file.size) {
            filename = upload.parentNode.parentNode.nextElementSibling;
            filename.value = file.name;
            filename.nextElementSibling.style.display = 'block';
        } else {
            alert(
                'File [' + file.name + '] is '
                + (Math.round((file.size / Math.pow(1024, 2)) * 10) / 10).toFixed(1) + 'MB.\n\n'
                + 'Maximum allowed upload size is ' + maxuploadSize / 1024 + 'MB.\n\n'
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