package com.telexfree.restapi;

import org.jivesoftware.openfire.container.Plugin;
import org.jivesoftware.openfire.container.PluginManager;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.File;

/**
 * @author ITRex Group
 */
public class RestApiPlugin implements Plugin {

    private static final Logger log = LoggerFactory.getLogger(RestApiPlugin.class);

    @Override
    public void initializePlugin(PluginManager pluginManager, File file) {
        log.info("initializing TelexFree api plugin");
    }

    @Override
    public void destroyPlugin() {
        log.info("destroying TelexFree api plugin");
    }

}
