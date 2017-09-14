var addButton = document.querySelector('#add-users');
addButton.addEventListener(
    'click',
    function(e) {
        var target = e.target;
        $.ajax({
            url: target.dataset.url
            , data: { users: ["1", "2"] }
            , type: 'POST'
            , headers: new Xhr().getXsrfToken(target)
        })
        .done(function (data, textStatus, jqXHR) {
            $('<div></div>').html(data).dialog({ width: 'auto', modal: true, title: 'Users Added' });
        })
        .fail(function (jqXHR, textStatus, errorThrown) {
            alert('Error');
        })
        .always(function () {
            //
        });
    }
);

var userContainer = document.querySelector('#user-list').firstElementChild;
console.log(userContainer);

function addToDom(id, name, office) {
    if (!userContainer.querySelector('[data-id="' + id + '"]')) {
        var div = document.createElement('div');
        div.innerHTML = "<div class='badge small' data-id='" + id + "'>"
            + name + ' - ' + office + '</div>';
        userContainer.appendChild(div);
    }
}

function removeFromDom(element) {
    element.parentElement.removeChild(element);
}

var autoComplete = new jQueryAutoComplete(
    '#searchText',
    // callback **MUST** name parameters **EXACTLY** same as below
    function (event, ui) {
        addToDom(ui.item.value, ui.item.label, ui.item.office)
        console.log('Selected label: ' + ui.item.label + ' value: ' + ui.item.value);
    }
);
autoComplete.autocomplete();