package com.telexfree.offlinemessageplugin;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.StatusLine;
import org.apache.http.client.ClientProtocolException;
import org.apache.http.client.ResponseHandler;
import org.apache.http.client.fluent.Request;
import org.apache.http.entity.ContentType;
import org.apache.http.util.EntityUtils;
import org.jivesoftware.openfire.OfflineMessageListener;
import org.jivesoftware.openfire.OfflineMessageStrategy;
import org.jivesoftware.openfire.container.Plugin;
import org.jivesoftware.openfire.container.PluginManager;
import org.json.JSONObject;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.xmpp.packet.Message;

import java.io.File;
import java.io.IOException;

/**
 * @author ITRex Group
 */
public class PushOfflineMessagePlugin implements Plugin, OfflineMessageListener {

    private static final Logger log = LoggerFactory.getLogger(PushOfflineMessagePlugin.class);

    @Override
    public void initializePlugin(PluginManager pluginManager, File file) {
        log.info("initializing TelexFree offline message plugin");
        OfflineMessageStrategy.addListener(this);
        log.debug("listener OfflineMessage added");
    }

    @Override
    public void destroyPlugin() {
        log.info("destroying TelexFree offline message plugin");
        OfflineMessageStrategy.removeListener(this);
        log.debug("removed OfflineMessage listener");
    }

    @Override
    public void messageBounced(Message message) {
        log.debug("Message bounced {}", message);
    }

    @Override
    public void messageStored(Message message) {
        log.info("Message: {}", message);
        if(Config.getInstance().isEnabled()) {
            //groupchat is not supported by openfire api
            if(Message.Type.chat.equals(message.getType()) || Message.Type.normal.equals(message.getType())) {
                String body = buildBody(message, Config.getInstance().getTelexApiKey());
                boolean result = sendNotification(body, Config.getInstance().getTelexApiUrl());
                if(result) {
                    log.info("Successfully sent to " + message.getTo().getNode());
                }
            }
        }
    }


    /*
    {
        api_key: "r213",
        message: "text",
        to: "user@telexfree.com",
        from: ""
    }
    */
    private String buildBody(Message message, String apiKey) {
        return new JSONObject()
                .put("api_key", apiKey)
                .put("message", message.getBody())
                .put("to", message.getTo().getNode() + "@telexfree.com") //maybe move such logic (get telex user by openfire login) to webservice
                .put("from", message.getFrom().getNode() + "@telexfree.com")
                .toString();
    }

    private boolean sendNotification(final String body, final String url) {
        log.debug("Sending message to url: {}, body: {}", url, body);
        try {
            return Request.Post(url)
                    .bodyString(body, ContentType.APPLICATION_JSON)
                    .execute()
                    .handleResponse(new ResponseHandler<Boolean>() {
                        @Override
                        public Boolean handleResponse(HttpResponse response) throws ClientProtocolException, IOException {
                            final StatusLine statusLine = response.getStatusLine();
                            final HttpEntity entity = response.getEntity();

                            if (entity != null) {
                                String responseText = new String(EntityUtils.toByteArray(entity));

                                if (statusLine.getStatusCode() >= 300) {
                                    log.info("Call was performed unsuccessful - " +
                                            "Request: URL: {}, Body: {}" +
                                            "Response: StatusCode: {}, Reason: {}, Text: {}",
                                            new String[] {
                                                    url,
                                                    body,
                                                    String.valueOf(statusLine.getStatusCode()),
                                                    statusLine.getReasonPhrase(),
                                                    responseText}
                                    );
                                } else {
                                    log.debug("Response: {}", responseText);
                                    return Boolean.TRUE;
                                }
                            }
                            return Boolean.FALSE;
                        }
                    });
        } catch (IOException e) {
            log.info("Exception while sending message", e);
        }
        return false;
    }
}
