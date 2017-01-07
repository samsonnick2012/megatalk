package xx;

import com.notnoop.apns.APNS;
import com.notnoop.apns.ApnsService;


public class Start {

	public static void main(String[] args) {
		String simplePayload = APNS.newPayload()
			    .alertBody("anton zabyl vesla").badge(45).sound("default").build();

			String client = "7772d88c69cfcf60743c7474d9923dab4d5e39d4897b9ea5374aa0f952973b1c";

			ApnsService service = APNS.newService()
			    .withCert("D:\\MegaTalk_APNS_Development1.p12", "123123")
			    .withSandboxDestination()
			    .build();

			service.push(client, simplePayload);

	}

}
