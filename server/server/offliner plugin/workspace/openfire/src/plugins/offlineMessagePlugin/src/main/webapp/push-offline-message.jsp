<%@ page
        import="com.telexfree.offlinemessageplugin.Config,
           org.jivesoftware.util.ParamUtils,
           java.util.HashMap,
           java.util.Map"
        errorPage="error.jsp"%>

<%@ taglib uri="http://java.sun.com/jstl/core_rt" prefix="c"%>
<%@ taglib uri="http://java.sun.com/jstl/fmt_rt" prefix="fmt"%>

<%
    boolean save = request.getParameter("save") != null;
    boolean enabled = ParamUtils.getBooleanParameter(request, "enabled", false);
    String apiKey = ParamUtils.getParameter(request, "apiKey");
    String apiUrl = ParamUtils.getParameter(request, "apiUrl");

    Config conf = Config.getInstance();

    Map<String, String> errors = new HashMap<String, String>();
    if (save) {
        if (apiKey == null || apiKey.trim().length() < 1) {
            errors.put("missingApiKey", "ApiKey is required");
        }

        if (apiUrl == null || apiUrl.trim().length() < 1) {
            errors.put("missingApiUrl", "ApiUrl is required");
        }

        if (errors.size() == 0) {
            conf.setEnabled(enabled);
            conf.setTelexApiKey(apiKey);
            conf.setTelexApiUrl(apiUrl);

            response.sendRedirect("push-offline-message.jsp?settingsSaved=true");
            return;
        }
    }

    enabled = conf.isEnabled();
    apiKey = conf.getTelexApiKey();
    apiUrl = conf.getTelexApiUrl();
%>

<html>
<head>
    <title>TelexFREE push notification plugin</title>
    <meta name="pageID" content="telex-plugin" http-equiv="Content-Type"
          content="text/html; charset=UTF-8"/>
</head>
<body>
<form action="push-offline-message.jsp?save" method="post">

    <div class="jive-contentBoxHeader">TelexFREE push notification plugin</div>
    <div class="jive-contentBox">

        <% if (ParamUtils.getBooleanParameter(request, "settingsSaved")) { %>

        <div class="jive-success">
            <table cellpadding="0" cellspacing="0" border="0">
                <tbody>
                <tr>
                    <td class="jive-icon"><img src="images/success-16x16.gif" width="16" height="16" border="0"></td>
                    <td class="jive-icon-label">Successfully saved</td>
                </tr>
                </tbody>
            </table>
        </div>

        <% } %>

        <table cellpadding="3" cellspacing="0" border="0" width="100%">
            <tbody>
            <tr>
                <td width="1%" align="center" nowrap>
                    <input type="checkbox" name="enabled" <%=enabled ? "checked" : "" %>>
                </td>
                <td width="99%" align="left">Enable Plugin</td>
            </tr>
            </tbody>
        </table>

        <br><br>
        <p>Plugin settings:</p>

        <table cellpadding="3" cellspacing="0" border="0" width="100%">
            <tbody>
            <tr>
                <td width="5%" valign="top">API Key:&nbsp;</td>
                <td width="95%"><input type="text" name="apiKey" size="120" value="<%= apiKey %>"></td>
                <% if (errors.containsKey("missingApiKey")) { %>
                <span class="jive-error-text"><%= errors.get("missingApiKey") %></span>
                <% } %>
            </tr>
            <tr>
                <td width="5%" valign="top">API URL:&nbsp;</td>
                <td width="95%"><input type="text" name="apiUrl" size="120" value="<%= apiUrl %>"></td>
                <% if (errors.containsKey("missingApiUrl")) { %>
                <span class="jive-error-text"><%= errors.get("missingApiUrl") %></span>
                <% } %>
            </tr>
            </tbody>
        </table>
    </div>
    <input type="submit" value="Save"/>
</form>

</body>
</html>
