function fileUpload(upload) {
    if (upload.files.length > 0) {
        filename = upload.parentNode.parentNode.nextElementSibling;
        filename.value = upload.files[0].name;
        filename.nextElementSibling.style.display = 'block';
    }
}
function clearUpload(button) {
    button.parentNode.parentNode.firstElementChild.firstElementChild.firstElementChild.value = '';
    button.parentNode.previousElementSibling.value = '';
    button.parentNode.style.display = 'none';
}