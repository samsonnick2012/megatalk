jQuery(function ($) {
    $(document).ready(function () {
        var menuarrow = $('.menuarrow');
        var panelbutton = $("#s_panel, #button");
        menuarrow.attr('src', '/Content/Images/next.png');
        $('#pmenu').toggle(function () {
            menuarrow.attr('src', '/Content/Images/back.png');
            panelbutton.animate({ left: "+=334px" }, 300);
        }, function () {
            menuarrow.attr('src', '/Content/Images/next.png');
            panelbutton.animate({ left: "-=334px" }, 300);
        });

        $("#browser").treeview({
            persist: "location"
        });

        $('.selected').parent().addClass("fileselected");

        $('#s_panel').alternateScroll();
    });
});



