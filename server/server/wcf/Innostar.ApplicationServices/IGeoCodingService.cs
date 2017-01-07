using Application.Core.ApplicationServices;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
    public interface IGeoCodingService : IApplicationService
    {
        void RefreshCoordinates();

	    GeoPoint DetermineAddressCoordinate(string address);
    }
}
