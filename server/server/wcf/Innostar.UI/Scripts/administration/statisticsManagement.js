$(document).ready(function () {
    $.xmpp.connect({
        jid: $("#XmppAdminLogin").val(),
        password: $("#XmppAdminPassword").val(),
        url: $("#HttpBindAddress").val(),
        onConnect: function () {
            $.xmpp.setPresence(null);
            var msg = "<iq from='"+ $.xmpp.jid +"'"+
            "id='get-online-users-num-1'"+
            "to='megatalk'"+
            "type='set'"+
            "xml:lang='en'>"+
              "<command xmlns='http://jabber.org/protocol/commands'" +
                "action='execute'" +
                "node='http://jabber.org/protocol/admin#get-online-users-num'/>" +
            "</iq>";
            $.xmpp.sendCommand(msg, null);
        },
        onIq: function (iq) {
            $("#OnlineCounter").text($(iq).find('field[label|="Number of Online Users"]').children()[0].innerText);
        },
        onError: function (error) {
            console.log("Error: " + error);
        }
    });
});