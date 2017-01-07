package com.telexfree.restapi;

import org.jivesoftware.openfire.XMPPServer;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.xmpp.packet.Message;

import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.MediaType;

/**
 * Service for sending system messages to telex users
 *
 * @author ITRex Group
 */
@Path("/telexapi/rest/message")
public class SystemMessageSendService {

    private static final Logger log = LoggerFactory.getLogger(SystemMessageSendService.class);

    @POST
    @Path("/send")
    @Produces(MediaType.TEXT_PLAIN)
    public String send() {
        log.info("Sending message");

        Message message = new Message();
        message.setTo("user1@192.168.30.44");
        message.setFrom("user2@192.168.30.44");
        message.setSubject("subj");
        message.setBody("test message");

        log.info("Getting message route");
        XMPPServer.getInstance().getMessageRouter().route(message);
        log.info("Message sent");

        return "sent";
    }

    public static void main(String[] args) {
        Message message = new Message();
        message.setTo("user1@192.168.30.44");
        message.setFrom("user2@192.168.30.44");
        message.setSubject("subj");
        message.setBody("test message");

        message.addChildElement("telex", "")
                .addAttribute("type", "file")
                .addElement("fileType").addText("photo")
                .getParent()
                .addElement("identifier").addText("BQKWN214AS");

        System.out.println(message.toString());
    }

}
