package com.telexfree.restapi;

import com.sun.jersey.api.core.PackagesResourceConfig;
import com.sun.jersey.spi.container.servlet.ServletContainer;
import org.jivesoftware.admin.AuthCheckFilter;

import javax.servlet.ServletException;
import java.util.HashMap;
import java.util.Map;

/**
 * @author ITRex Group
 */
public class JerseyServletWrapper extends ServletContainer {

    private static final String SERVLET_URL = "telexapi/rest/*";
    private static final String SCAN_PACKAGE_KEY = "com.sun.jersey.config.property.packages";
    private static final String SCAN_PACKAGE_DEFAULT = JerseyServletWrapper.class.getPackage().getName();

    private static final String RESOURCE_CONFIG_CLASS_KEY = "com.sun.jersey.config.property.resourceConfigClass";
    private static final String RESOURCE_CONFIG_CLASS = "com.sun.jersey.api.core.PackagesResourceConfig";

    private static Map<String, Object> config;
    private static PackagesResourceConfig prc;

    static{
        config = new HashMap<String, Object>();
        config.put(RESOURCE_CONFIG_CLASS_KEY, RESOURCE_CONFIG_CLASS);
        config.put(SCAN_PACKAGE_KEY, SCAN_PACKAGE_DEFAULT);
        prc = new PackagesResourceConfig(SCAN_PACKAGE_DEFAULT);
        prc.setPropertiesAndFeatures(config);

        prc.getClasses().add(Hello.class);
        prc.getClasses().add(SystemMessageSendService.class);
    }

    public JerseyServletWrapper() {
        super(prc);
    }

    @Override
    public void init() throws ServletException {
        super.init();
        AuthCheckFilter.addExclude(SERVLET_URL);
    }

    @Override
    public void destroy() {
        super.destroy();
        AuthCheckFilter.removeExclude(SERVLET_URL);
    }
}
