using System;
using MonoTouch.CoreLocation;

namespace Xamarin.pArk
{
    public class LocationDelegate : CLLocationManagerDelegate
    {
        private ARView _arView;

        public LocationDelegate(ARView arView)
        {
            _arView = arView;
        }
            
        [Obsolete ("Deprecated in iOS 6.0")]
        public override void UpdatedLocation(CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
        {
            _arView.UpdatePlacesOfInterestCoordinates(newLocation);
        }

        public override void LocationsUpdated(CLLocationManager manager, CLLocation[] locations)
        {
            _arView.UpdatePlacesOfInterestCoordinates(locations[locations.Length - 1]);
        }
    }
}