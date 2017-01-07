var usersManagement = {
    sleep: function (milliseconds) {
        var start = new Date().getTime();
        for (var i = 0; i < 1e7; i++) {
            if ((new Date().getTime() - start) > milliseconds) {
                break;
            }
        }
    },

    processMessageBody: function (loginValue, messageBody, data) {
        var buf1 = messageBody.replace('%userDisplayName%', data.userDisplayName);
        var buf2 = buf1.replace('%userEmail%', data.userEmail);
        var buf3 = buf2.replace('%userConfirmRequest%', data.userConfirmRequest);
        var result = buf3.replace('%userPasswordRequest%', data.userPasswordRequest);
        return result;
    },

    handleSafeCheckboxes: function () {
        $("input:checkbox[class='SafeCbx']").on('click', function () {
            if (this.checked) {
                $.datetimeDialog('Активация безопасного режим',
                    'Дата окончания безопасного режима :',
                    'EnableSafeMode',
                    { id: $(this).data('id') },
                    function () {
                        var pageNumber = $("#page-number") != undefined ? $("#page-number").val() : "0";
                        $("#listContainer").empty();
                        $.get("_UsersList", { pageNumber: pageNumber, safeMode: $(this).data('type') > 0 }, function (data) {
                            $("#listContainer").html(data);
                            usersManagement.handleSafeCheckboxes();
                            usersManagement.handlePageButtons();
                        });
                    });
            } else {
                $.post('DisableSafeMode', { id: $(this).data('id') }, function () {
                    var pageNumber = $("#page-number") != undefined ? $("#page-number").val() : "0";
                    $("#listContainer").empty();
                    $.get("_UsersList", { pageNumber: pageNumber, safeMode: $(this).data('type') > 0 }, function (data) {
                        $("#listContainer").html(data);
                        usersManagement.handleSafeCheckboxes();
                        usersManagement.handlePageButtons();
                    });
                });
            }
        });
    },

    handlePageButtons: function () {
        $(".page-button-for-users").click(function () {
            var pageNumber = $(this).data('number');
            usersManagement.executeSearch(pageNumber);
        });
    },

    executeSearch: function(pageNumber) {
        var text = $("#searchInput").val();
        pageNumber = pageNumber == undefined || pageNumber == 0 ? 1 : pageNumber;
        $("#listContainer").empty();
        $.get("_UsersList", { pageNumber: pageNumber, safeMode: $('#filter-menu').attr('data-type') > 0, text: text }, function (data) {
            $("#listContainer").html(data);
            usersManagement.handleSafeCheckboxes();
            usersManagement.handlePageButtons();
        });
    }
};

$(document).ready(function () {

    $("#searchButton").click(function() {
        usersManagement.executeSearch();
    });

    $("#searchInput").keypress(function (event) {
        if (event.keyCode == 13) {
            usersManagement.executeSearch();
        }
    });

    $("#filter-menu-container .dropdown-menu li a").click(function () {
        var selText = $(this).text();
        $("#searchInput").val("");
        $(this).parents('#filter-menu').find('.dropdown-toggle').html(selText + ' <span class="caret"></span>');
        if ($('#filter-menu').attr('data-type') != $(this).data('type')) {
            $('#filter-menu').attr('data-type', $(this).data('type'));
            $("#listContainer").empty();
            $.get("_UsersList", { pageNumber: "1", safeMode: $(this).data('type') > 0 }, function (data) {
                $("#listContainer").html(data);
                usersManagement.handleSafeCheckboxes();
                usersManagement.handlePageButtons();
            });
        }
    });

    $("#template-menu-container .dropdown-menu li a").click(function () {
        var selText = $(this).text();
        $(this).parents('#template-menu').find('.dropdown-toggle').html(selText + ' <span class="caret"></span>');
        $.get("_TextTemplate", { id: $(this).data('id') }, function (data) {
            $("#MessageContainer").val(data);
        });
        if ($(this).data('id') == 1 || $(this).data('id') == 2) {
            $("#selected-template-type").val($(this).data('id'));
        } else {
            $("#selected-template-type").val(0);
        }
    });

    usersManagement.handlePageButtons();
    usersManagement.handleSafeCheckboxes();

    $("#MessageSender").on('click', function () {
        $.infoDialog("Информационное сообщение", "<div>Начата рассылка писем.</div><div>Ожидайте завершения операции.</div>");
        $("#saveButton").hide();
        usersManagement.sleep(100);
        if ($("#allSelectedCbx").is(':checked')) {
            $.xmpp.connect({
                jid: $("#XmppAdminLogin").val(),
                password: $("#XmppAdminPassword").val(),
                url: $("#HttpBindAddress").val(),
                onConnect: function () {
                    $.xmpp.setPresence(null);
                    $.get("GetAllXmppLogins", { safeMode: $('#filter-menu').attr('data-type') > 0 }, function (data) {
                        var logins = data;
                        $.xmpp.setPresence(null);
                        var messageBody = $("#MessageContainer").val();
                        $.each(logins, function (index, element) {
                            $.get("_MessageVariables", { login: element }, function (data) {
                                var processedBody = usersManagement.processMessageBody(element, messageBody, data);
                                $.xmpp.sendMessage({
                                    body: processedBody,
                                    to: element
                                });
                                usersManagement.sleep(100);
                            });
                        });
                        $("#bodyContainer").text("Рассылка писем завершилась.");
                        $("#saveButton").show();
                    });
                },
                onError: function (error) {
                    console.log("Error: " + error);
                }
            });
        } else {
            var selectedCheckBoxes = $("input:checkbox[class='selectedCbx']:checked");
            var logins = [];
            $.each(selectedCheckBoxes, function (index, element) {
                logins.push($(element).data("login"));
            });
            if (logins.length > 0) {
                $.xmpp.connect({
                    jid: $("#XmppAdminLogin").val(),
                    password: $("#XmppAdminPassword").val(),
                    url: $("#HttpBindAddress").val(),
                    onConnect: function () {
                        $.xmpp.setPresence(null);
                        var messageBody = $("#MessageContainer").val();
                        $.each(logins, function (index, element) {
                            $.get("_MessageVariables", { login: element }, function (data) {
                                var processedBody = usersManagement.processMessageBody(element, messageBody, data);
                                $.xmpp.sendMessage({
                                    body: processedBody,
                                    to: element
                                });
                                usersManagement.sleep(100);
                            });
                        });
                        $("#bodyContainer").text("Рассылка писем завершилась.");
                        $("#saveButton").show();
                    },
                    onError: function (error) {
                        console.log("Error: " + error);
                    }
                });
            } else {
                $("#bodyContainer").text("Не выбрано ни одного пользователя.");
                $("#saveButton").show();
            }
        }
    });
});