function JQueryConfirm() {
    Object.defineProperty(this, 'modalElementId', {
        value: 'jquery-confirm-modal'
    });
}

JQueryConfirm.prototype = {
    constructor: JQueryConfirm,
    confirmed: function() { return this._confirmed; },
    show: function (innerHTML, confirmCallback) {
        var self = this;
        $(self.getModalElement()).html(innerHTML).dialog({
            resizable: false,
            height: 'auto',
            width: 400,
            modal: true,
            title: 'Confirm Action',
            buttons: {
                'Yes': function () {
                    confirmCallback();
                    $(this).dialog('close');
                },
                'No': function () {
                    $(this).dialog('close');
                }
            }
        });
    },
    getModalElement: function() {
        var modalElement;

        if (!(modalElement = document.querySelector('#' + this.modalElementId))) {
            var modalElement = document.createElement('div');
            modalElement.id = this.modalElementId;
            modalElement.style.display = 'none';
            document.body.appendChild(modalElement);
        }

        return modalElement;
    }
}