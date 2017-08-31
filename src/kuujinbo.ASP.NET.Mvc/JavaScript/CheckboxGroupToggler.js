function CheckboxGroupToggler(groupSelector) {
    Object.defineProperty(this, 'initError', {
        value: 'CSS selector for the checkbox group parent DOM element required.'
    });

    Object.defineProperty(this, 'containerError', {
        value: 'checkbox group parent DOM element not found. verify CSS selector is correct'
    });

    Object.defineProperty(this, 'buttonUncheckClassList', {
        value: 'btn btn-default'
    });
    Object.defineProperty(this, 'buttonCheckClassList', {
        value: 'btn btn-primary checked'
    });

    if (!groupSelector || !groupSelector.match(/\w+/)) throw this.initError;

    this._checkGroupSelector = groupSelector;
}

CheckboxGroupToggler.prototype = {
    constructor: CheckboxGroupToggler,
    getCheckAllHtml: function() {
        return "<span class='glyphicon glyphicon-check'></span> Check All";
    },
    getUncheckAllHtml: function() {
        return "<span class='glyphicon glyphicon-unchecked'></span> Uncheck All";
    },
    getCheckGroup: function() {
        var element = document.querySelector(this._checkGroupSelector);
        if (!element) throw this.containerError;

        return element;
    },
    // toggle widget added **BEFORE** container element by default
    addToggleElement: function(addAfter) {
        var toggler = document.createElement('span');
        toggler.innerHTML = "<button type='button' class='"
            + this.buttonCheckClassList
            + "' style='margin:8px 0;text-align:left;width:120px'>"
            + this.getCheckAllHtml()
            + '</button>';
        var checkGroup = this.getCheckGroup();
        checkGroup.parentNode.insertBefore(toggler, !addAfter ?  checkGroup : checkGroup.nextSibling);

        toggler.addEventListener(
            'click', this.clickToggleElement.bind(this), false
        );
    },
    clickToggleElement: function(e) {
        var checkGroup = this.getCheckGroup();
        var toggler = e.target;
        if (toggler.className === this.buttonCheckClassList) {
            var elements = checkGroup.querySelectorAll('input[type="checkbox"]:not(:checked)');
            for (i = 0; i < elements.length; ++i) elements[i].checked = true;
            toggler.className = this.buttonUncheckClassList;
            toggler.innerHTML = this.getUncheckAllHtml();
        } else {
            var elements = checkGroup.querySelectorAll('input[type="checkbox"]:checked');
            for (i = 0; i < elements.length; ++i) elements[i].checked = false;
            toggler.className = this.buttonCheckClassList;
            toggler.innerHTML = this.getCheckAllHtml();
        }
    }
}