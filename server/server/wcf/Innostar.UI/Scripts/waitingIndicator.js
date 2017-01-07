var spinner = {
    hide: function (spinnerObject) {
        spinnerObject.stop();
    },

    show: function (selector, top, left, length, width, radius) {
        if (top == null) {
            top = 'auto';
        }
        if (left == null) {
            left = 'auto';
        }
        if (length == null) {
            length = 11;
        }
        if (width == null) {
            width = 2;
        }
        if (radius == null) {
            radius = 8;
        }
        var opts = {
            lines: 11,
            length: length,
            width: width,
            radius: radius,
            corners: 1,
            rotate: 0,
            direction: 1,
            color: '#3574FC',
            speed: 1,
            trail: 60,
            shadow: false,
            hwaccel: false,
            className: 'spinner',
            zIndex: 2e9,
            top: top,
            left: left
        };

        var spinner = new Spinner(opts).spin($(selector)[0]);

        return spinner;
    },
};