var tagsCloud = {
    create: function () {
        $.get("home/gettags", null, function (result) {
            $.each(result, function (index, element) {
                element.html = { style: 'font-size:' + tagsCloud.getFontSize(element.fontSize) + 'px;' + 'color:' + element.color };
                element.afterWordRender = function () {
                    $(this).find("a").handle("click", function (e) {
                        $.processAjaxData($(this).attr("href"));
                        $("#search-input").val($(this).text());
                        $('#search-input').trigger("input");
                        mapManagement.find();
                        $("#tags-container-background").remove();
                        $("#tags-container").remove();

                        e.preventDefault();
                    });
                };
            });

            $("#tags").jQCloud(result, {
                delayedMode: true,
            });

            $('#tags').position({
                of: $(window)
            });

            $('#tags').css("top", 45);
        });
    },
    
    hide:function() {
        $("#tags-container").hide();
        $("#tags-container-background").hide();
    },

    getFontSize: function (fontsize) {
        return window.screen.height * fontsize / 100;
    }
};
