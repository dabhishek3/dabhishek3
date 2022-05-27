using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace GeoFrench
{
	public class GetLocationService
	{
		readonly bool stopping = false;
		public GetLocationService()
		{
		}

		public async Task Run(CancellationToken token)
		{
			await Task.Run(async () => {
				while (!stopping)
				{
					token.ThrowIfCancellationRequested();
					try
					{
						await Task.Delay(2000);

						var request = new GeolocationRequest(GeolocationAccuracy.High);
						var location = await Geolocation.GetLocationAsync(request);
						if (location != null)
						{
							var message = new LocationMessage
							{
								Latitude = location.Latitude,
								Longitude = location.Longitude
							};

							long geofenceDistance = 1500;
							var lat = 22.7255;
							var log = 75.8886;
							Location geofenceCentercordinate = new Location(lat, log);
							Location userLocation = new Location(message.Latitude, message.Longitude);
							var distance = (long)Location.CalculateDistance(userLocation, geofenceCentercordinate, DistanceUnits.Kilometers);
							double finaldistance = Math.Round((distance) * 1000.00, 2);
							if (finaldistance < geofenceDistance)
							{
								Console.WriteLine($"Welcome to my den: {finaldistance}");
							}


							Device.BeginInvokeOnMainThread(() =>
							{
								MessagingCenter.Send(message, "Location");
							});
						}
					}
					catch (Exception ex)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							var errormessage = new LocationErrorMessage();
							MessagingCenter.Send(errormessage, "LocationError");
						});
					}
				}
				return;
			}, token);
		}
	}
}
