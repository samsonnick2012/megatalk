$.extend({
    isIE: function () {
        var myNav = navigator.userAgent.toLowerCase();

        if (myNav.indexOf('msie') != -1) {
            return parseInt(myNav.split('msie')[1]);
        } else if (!!navigator.userAgent.match(/Trident\/7\./)) {
            return 11;
        } else {
            return -1;
        }
    },
    generateUniqID: function (idlength) {
        var charstoformid = '_0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz'.split('');
        if (idlength == null) {
            idlength = 5;
        }
        var uniqid = '';
        for (var i = 0; i < idlength; i++) {
            uniqid += charstoformid[Math.floor(Math.random() * charstoformid.length)];
        }

        return uniqid;
    },
    getUrlQuery: function (url, params) {
        return url + "?" + $.param(params);
    },
    existsObject: function (chekingObject) {
        return chekingObject != null && chekingObject.length > 0;
    },
    executeFunctionByName: function (functionName) {
        var context = window;
        var args = Array.prototype.slice.call(arguments, 2);
        var namespaces = functionName.split(".");
        var func = namespaces.pop();
        for (var i = 0; i < namespaces.length; i++) {
            context = context[namespaces[i]];
        }
        return context[func].apply(context, args);
    },
    infoDialog: function (header, body) {
        var that = this;
        var dialogId = that.generateUniqID();

        $("body").append("<div class=\"modal fade\" id=\"" + dialogId + "\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myModalLabel\" aria-hidden=\"true\"></div>");
        var dialogContainer = $("#" + dialogId);
        var bootstrapDialogMarkup =
                "<div class=\"modal-dialog\">" +
                "<div class=\"modal-content\">" +
                "<div class=\"modal-header\">" +
                "<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\">&times;</button>" +
                "<h4 class=\"modal-title\" id=\"myModalLabel\">" + header + "</h4>" +
                "</div>" +
                "<div class=\"modal-body\">" +
                "<div id=\"bodyContainer\" class=\"content\">" + body + "</div>" +
                "</div>" +
                "<div class=\"modal-footer\">" +
                "<input type=\"button\" id=\"saveButton\" value=\"Ok\" class=\"btn btn-primary\" />" +
                "</div>" +
                "</div>" +
                "</div>";

        $(dialogContainer).html(bootstrapDialogMarkup);

        $("#" + dialogId).on('hidden.bs.modal', function (element) {
            if ($(element.target).hasClass("modal")) {
                $("#" + dialogId).remove();
            }
        });

        $("#" + dialogId).on('shown.bs.modal', function (element) {
            $("#saveButton").handle("click", function () {
                $("#" + dialogId).modal('hide');
            });
        });

        $(dialogContainer).modal();

        return dialogId;
    },
    datetimeDialog: function (header, body, url, data, hiddenCallback) {
        var that = this;
        var dialogId = that.generateUniqID();

        $("body").append("<div class=\"modal fade\" id=\"" + dialogId + "\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"myModalLabel\" aria-hidden=\"true\"></div>");
        var dialogContainer = $("#" + dialogId);
        var bootstrapDialogMarkup =
                "<div class=\"modal-dialog\">" +
                "<div class=\"modal-content\">" +
                "<div class=\"modal-header\">" +
                "<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-hidden=\"true\">&times;</button>" +
                "<h4 class=\"modal-title\" id=\"myModalLabel\">" + header + "</h4>" +
                "</div>" +
                "<div class=\"modal-body\">" +
                "<div id=\"bodyContainer\" class=\"content\">" +
                    "<div>" + body + "</div>" +
                    "<input type='text' class='span2 datepicker' value='' id='modalDatepicker'>" +
                    "</div>" +
                "</div>" +
                "<div class=\"modal-footer\">" +
                "<input type=\"button\" id=\"saveButton\" value=\"Ok\" class=\"btn btn-primary\" />" +
                "</div>" +
                "</div>" +
                "</div>";

        $(dialogContainer).html(bootstrapDialogMarkup);

        $("#" + dialogId).on('hidden.bs.modal', function (element) {
            if ($(element.target).hasClass("modal")) {
                $("#" + dialogId).remove();
            }
            hiddenCallback();
        });

        $("#" + dialogId).on('shown.bs.modal', function (element) {
            $('#modalDatepicker').datepicker({
                language: 'ru',
                format: 'dd.mm.yyyy'
            });
            $('#modalDatepicker').datepicker("setDate", new Date());
            $("#saveButton").handle("click", function () {
                data.endTime = $("#modalDatepicker").val();
                $.post(url, data, function () {
                    $("#" + dialogId).modal('hide');
                });
            });
        });

        $(dialogContainer).modal();

        return dialogId;
    },
    processAjaxData: function (urlPath) {
        window.history.pushState("", "", urlPath);
    },
    elementId: function (id) {
        return "#" + id;
    },
    showValidationErrorMessage: function (element, isValid) {
        var validatingElements = [];

        if (element != null) {
            validatingElements.push(element);;
        } else {
            validatingElements = $("body .input-validation-error");
        }

        $.each(validatingElements, function (index, validatingElement) {
            $(validatingElement).tooltip('destroy');
            if (!isValid || $(validatingElement).hasClass(".input-validation-error")) {
                var validationMessage = $(element).attr($(validatingElement).getValidationAttribute());
                $(validatingElement).tooltip({ title: validationMessage, placement: 'right' });
            }
        });
    }
});
$.fn.extend({
    handle: function (eventName, handler) {
        $(this).off(eventName).on(eventName, handler);
    },
    getValidationAttribute: function () {
        var result = [];
        $(this[0].attributes).each(function (index, attribute) {
            if (attribute.name.match("^data-val-")) {
                result.push(attribute.name);
            }
        });

        return result[0];
    },
    getDataAttributes: function () {
        var result = [];

        $(this[0].attributes).each(function (index, attribute) {
            if (attribute.name.match("^data-")) {
                result[attribute.name.substring(5, attribute.name.length)] = attribute.value;
            }
        });

        return result;
    },
    showWaiting: function (options) {
        $(this[0]).waitingIndicator(options);
        var waitingIndicator = $(this[0]).data("omertex-waitingIndicator");
        waitingIndicator.show(true);
    },
    hideWaiting: function () {
        var waitingIndicator = $(this[0]).data("omertex-waitingIndicator");
        waitingIndicator.hide(true);
    },
    addCloseButtonInAlert: function () {
        var closeButton = $('<a href="#"><i class="icon-white icon-remove close"></i></a>');
        closeButton.click(function (e) {
            e.preventDefault();
            $(this).parent('div').slideUp(function () {
                $(this).hide();
                shared.bodyHeightChanged();
            });
        });

        $(this[0]).prepend(closeButton);
    },
    formatString: function () {
        var args = [].slice.call(arguments);
        if (this.toString() != '[object Object]') {
            args.unshift(this.toString());
        }

        var pattern = new RegExp('{([1-' + args.length + '])}', 'g');
        return String(args[0]).replace(pattern, function (match, index) { return args[index]; });
    }
});