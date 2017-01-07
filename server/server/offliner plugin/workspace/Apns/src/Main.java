import com.notnoop.apns.APNS;
import com.notnoop.apns.ApnsService;


public class Main {

	public static void main(String[] args) {
		String simplePayload = APNS.newPayload()
			    .alertBody("anton zabyl vesla").badge(35).sound("default").build();

			String client = "72814f5fb65be062673220ce1af316166e1d4942";

			ApnsService service = APNS.newService()
			    .withCert("D:\\MegaTalk_APNS_Development1.p12", "123123")
			    .withSandboxDestination()
			    .build();

			service.push(client, simplePayload);
	}
}
