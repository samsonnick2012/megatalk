using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace XChat.Service
{
    public class UploadFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new MyServiceHost(serviceType, baseAddresses);
        }

        class MyRawMapper : WebContentTypeMapper
        {
            public override WebContentFormat GetMessageFormatForContentType(string contentType)
            {
                return WebContentFormat.Raw;
            }
        }

        public class MyServiceHost : ServiceHost
        {
            public MyServiceHost(Type serviceType, Uri[] baseAddresses)
                : base(serviceType, baseAddresses) { }

            protected override void OnOpening()
            {
                base.OnOpening();

                CustomBinding binding = new CustomBinding(new WebHttpBinding());
                binding.Elements.Find<WebMessageEncodingBindingElement>().ContentTypeMapper = new MyRawMapper();
                ServiceEndpoint endpoint = this.AddServiceEndpoint(typeof(IUploader), binding, "");
                endpoint.Behaviors.Add(new WebHttpBehavior());
            }
        }
    }
}