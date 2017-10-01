/* ########################################################################
SIDE NOTES:
---------------------------------------------------------------------------
1. IIS/ASP.NET has inconsistent behaviour depending on config settings:
    i. <httpRuntime maxRequestLength            => 500 HTTP response 
    ii. <requestLimits maxAllowedContentLength  => 404.13 HTTP response

the default <requestLimits maxAllowedContentLength is ~30MB, and may
require a server administrator to set correctly, even if explicitly set
in web.config.  best bet is to keep the directive 'chunk-size' to well-under
30MB, or leave at default of 4mb. (purpose of chunking is not to use large
file size anyway)
######################################################################## */
'use strict';

var m = angular.module('angular1.PluploadWrapper', []);
m.directive('pluploadWrapper', ['$compile', 'XSRF',
    function($compile, XSRF) {
        // start directive sanity checking
        var disableOnError = function(msg) {
            alert(msg + '. plupload-wrapper DISABLED.');
            throw new Error(msg);
        };

        var initPlupload = function() {
            if (typeof plupload === 'undefined') {
                disableOnError('Plupload API not loaded');
            }
        };

        var validateScopeParams = function(params) {
            var missing = [];

            for (var param in params) {
                if (!params[param]) missing.push(param);
            }

            if (missing.length > 0) {
                var msg = 'missing required parameter(s): ('
                    + missing.join()
                    + ')'
                ;
                disableOnError(msg);
            }
        };
        // end directive sanity checking

        // unique DOM Ids - directive can be used multiple times in a view
        var setUniqueIds = function(scope) {
            scope.uploadQueueId = plupload.guid('upload-queue-');
            scope.pluploadContainerId = plupload.guid('plupload-container-');
            scope.browseFilesId = plupload.guid('browse-files-'); 
            scope.uploadFilesId = plupload.guid('upload-files-');
        }

        // directive HTML
        var getTemplate = function(scope) {
            return "<div id='"
                    + scope.uploadQueueId
                    + "'>Your browser doesn't have HTML5 support.</div>"
                + "<div id='" + scope.pluploadContainerId + "' class='btn-toolbar'>"
                + "<button id='" + scope.browseFilesId
                    + "' class='btn btn-info'>Browse <icon class='glyphicon glyphicon-search' /></button>"
                + "<button id='" + scope.uploadFilesId
                    + "' class='collapse btn btn-primary'>Upload <icon class='glyphicon glyphicon-upload' /></button>"
                + "</div>"
            ;
        };

        // remove files from plupload queue
        var deleteHandler = function(uploader, file) {
            return function(event) {
                event.preventDefault();
                $(this).closest('div').remove();
                uploader.removeFile(file);
            };
        };

        // start directive code
        return {
            restrict: 'E',
            scope: {
                uploadUrl: '@',
                deleteUrl: '@',
                chunkSize: '@'
            },
            link: function(scope, element, attr)
            {
                initPlupload();
                validateScopeParams({
                    'upload-url': scope.uploadUrl, 'delete-url': scope.deleteUrl
                });
                setUniqueIds(scope);

                var html = $compile(getTemplate(scope))(scope);
                element.append(html);

                var uploadFilesLink = $('#' + scope.uploadFilesId);
                var fileQueueContainer = $('#' + scope.uploadQueueId);

                var csrfToken = $('input[name=' + XSRF.TOKEN_NAME + ']');
                var csrfHeaders = {};
                csrfHeaders[XSRF.XHR_HEADER_NAME] = XSRF.XHR_HEADER_VALUE;
                csrfHeaders[XSRF.TOKEN_NAME] = csrfToken.length > 0 ? csrfToken[0].value : '';

                var uploader = new plupload.Uploader({
                    runtimes: 'html5,html4',
                    container: scope.pluploadContainerId,
                    browse_button: scope.browseFilesId,
                    url: scope.uploadUrl,
                    headers: csrfHeaders,
                    chunk_size: scope.chunkSize || '4mb',
                    unique_names: false,
                    // TODO: allow **any** file type?!?!
                    filters: {
                        prevent_duplicates: true,
                        mime_types: [
                            { title: "All files", extensions: "*" }
                        ]
                    },

                    init: {
                        PostInit: function(up) {
                            fileQueueContainer.html('');
                            document.getElementById(scope.uploadFilesId).onclick = function () {
                                uploader.start();
                                return false;
                            };
                        },

                        // after file(s) added to queue
                        FilesAdded: function(up, files) {
                            uploadFilesLink.show();

                            plupload.each(files, function(file) {
                                var deleteId = 'deleteId' + file.id;
                                fileQueueContainer.append($(
                                    '<div id="' + file.id + '">'
                                    + file.name
                                    + ' ('
                                    + plupload.formatSize(file.size)
                                    + ') <b></b> '
                                    + "<a href='#self' id='" + deleteId + "'>"
                                    + "<icon class='glyphicon glyphicon-remove-circle' style='color:red;' />"
                                    + '</a>'
                                    + '</div>'
                                ));

                                $('#' + deleteId).click(deleteHandler(up, file));
                            });
                        },

                        //  when file removed from queue, hide upload button if queue empty
                        FilesRemoved: function (up, files) {
                            if (!fileQueueContainer.text())  uploadFilesLink.hide();
                        },

                        // while a file is being uploaded
                        UploadProgress: function(up, file) {
                            var queuedFile = document.getElementById(file.id);
                            if (queuedFile) {
                                queuedFile.getElementsByTagName('b')[0].innerHTML = file.percent + '%';
                            }
                        },

                        // file successfully uploaded
                        FileUploaded: function(up, file, response) {
                            var deleteDataId = JSON.parse(response.response);
                            var queuedFile = $('#' + file.id).find('a').first();
                            var icon = queuedFile.find('icon').first();
                            if (queuedFile && icon) {
                                queuedFile.attr('delete-data-id', deleteDataId);
                                icon.css('color', 'green')
                                    .attr('class', 'glyphicon glyphicon-ok')
                                ;
                            }
                        },

                        // after **ALL** files uploaded or failed
                        UploadComplete: function (up, files) {
                            uploadFilesLink.hide();
                        },

                        Error: function (up, error) {
                            switch (error.code) {
                                case plupload.FILE_DUPLICATE_ERROR:
                                    console.log(error.file.name);
                                    alert(error.file.name + ' is already in upload queue.');
                                    break;
                                case plupload.HTTP_ERROR:
                                    var errorFile = $('#' + error.file.id);
                                    var icon = errorFile.find('icon').first();
                                    var status = errorFile.find('b').first();
                                    if (errorFile && icon && status) {
                                        status.text('Upload failed');
                                        icon.css('color', 'red')
                                            .attr('class', 'glyphicon glyphicon-alert')
                                        ;
                                        $(element).append(errorFile.detach());
                                    }
                                    break;
                                default:
                                    alert('Error uploading one or more files.');
                                    break;
                            }
                        }
                    }
                });
                uploader.init();
            }
        };
    }
]);