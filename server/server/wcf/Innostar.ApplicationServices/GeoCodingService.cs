using System.Linq;
using Application.Core.ApplicationServices;
using GeoCoding;
using GeoCoding.Google;
using Innostar.Dal.Repositories;
using Innostar.Models;

namespace Innostar.ApplicationServices
{
	public class GeoCodingService : ApplicationService, IGeoCodingService
	{
		public void RefreshCoordinates()
		{
			using (var repositoriesContainer = new InnostarRepositoriesContainer())
			{
				var organizationsList = repositoriesContainer.RepositoryFor<Organization>().GetAll().ToList();

				IGeoCoder geoCoder = new GoogleGeoCoder();

				foreach (var mapObject in organizationsList)
				{
					var organization = mapObject;

					var addresses = geoCoder.GeoCode(organization.Address).ToList();

					if (addresses.Any())
					{
						organization.Latitude = addresses.First().Coordinates.Latitude;
						organization.Longitude = addresses.First().Coordinates.Longitude;
					}
				}

				repositoriesContainer.RepositoryFor<Organization>().Save(organizationsList);
				repositoriesContainer.ApplyChanges();
			}
		}

		public GeoPoint DetermineAddressCoordinate(string address)
		{
			IGeoCoder geoCoder = new GoogleGeoCoder();

			var result = new GeoPoint();

			var addressCoordinates = geoCoder.GeoCode(address).FirstOrDefault();

			if (addressCoordinates != null)
			{
				result.Latitude = addressCoordinates.Coordinates.Latitude;
				result.Longitude = addressCoordinates.Coordinates.Longitude;
			}

			return result;
		}
	}
}
