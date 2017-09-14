var autoComplete = new jQueryAutoComplete('#searchText',
    function (event, ui) {
        console.log('Selected label: ' + ui.item.label + ' value: ' + ui.item.value);
    }
);

autoComplete.autocomplete();