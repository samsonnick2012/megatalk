package com.telexfree.offlinemessageplugin;

import org.jivesoftware.util.JiveGlobals;

/**
 * @author ITRex Group
 */
public class Config {

    private static final String ENABLED = "plugin.telexfree.pushoffline.enabled";
    private static final String API_KEY = "plugin.telexfree.api.key";
    private static final String API_URL = "plugin.telexfree.api.url";

    private Config() {}

    public static Config getInstance() {
        return ConfigInstanceHolder.INSTANCE;
    }

    private static class ConfigInstanceHolder {
        private static final Config INSTANCE = new Config();
    }

    public boolean isEnabled() {
        return JiveGlobals.getBooleanProperty(ENABLED, true);
    }

    public void setEnabled(boolean enabled) {
        JiveGlobals.setProperty(ENABLED, Boolean.valueOf(enabled).toString());
    }

    public String getTelexApiKey() {
        return JiveGlobals.getProperty(API_KEY, "k6FgFDtwvpcm8htTHDX3m8GTDE6w6e6WWc8ZPtGUvx7ElIb0mXXzSeuNMNArZlgj");
    }

    public void setTelexApiKey(String telexApiKey) {
        JiveGlobals.setProperty(API_KEY, telexApiKey);
    }

    public String getTelexApiUrl() {
        return JiveGlobals.getProperty(API_URL, "http://127.0.0.1:8080/api/v2/push");
    }

    public void setTelexApiUrl(String telexApiUrl) {
        JiveGlobals.setProperty(API_URL, telexApiUrl);
    }

}
