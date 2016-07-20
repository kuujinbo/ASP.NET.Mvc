var customFilter = function () {
    return {
        writeCustomFilter: function () {
            var filter = $('th > input[data-column-number=3]');
            $.ajax({
                url: "/jQueryDataTables/CustomOfficeFilter"
            })
            .done(function (data, textStatus, jqXHR) {
                var changed = "<select class='form-control input-sm' data-column-number='3'>\n"
                    + "<option selected='selected' value=''></option>";
                if (data !== null) {
                    for (var i = 0; i < data.length; ++i) {
                        changed += "<option value='" 
                            + data[i]
                            + "'>"
                            + data[i]
                            + '</option>';
                    }
                }
                changed += '</select>';
                filter.parent().html(changed);
                console.log(data);
            })
            .fail(function (jqXHR, textStatus, errorThrown) {
                console.log('ERROR')
            });
        }
    };
}();

customFilter.writeCustomFilter();