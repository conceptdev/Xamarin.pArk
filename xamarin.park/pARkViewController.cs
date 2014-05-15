using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.CoreLocation;

namespace Xamarin.pArk
{
    public class pARkViewController : UIViewController
    {
        ARView _arView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            // Perform any additional setup after loading the view, typically from a nib.
            View = _arView = new ARView();

            var placesOfInterest = new List<PlaceOfInterest>();


            var label = new UILabel
            {
                AdjustsFontSizeToFitWidth = false,
                Opaque = false,
                BackgroundColor = new UIColor(0.1f, 0.1f, 0.1f, 0.5f),
                Center = new PointF(200.0f, 200.0f),
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Text = "Central Park NY",
                Hidden = true
            };
            var size = label.StringSize(label.Text, label.Font);
            label.Bounds = new RectangleF(0.0f, 0.0f, size.Width, size.Height);

            var poi = new PlaceOfInterest
            {
                Location = new CLLocation(new CLLocationCoordinate2D(40.7711329, -73.9741874), 0.0, 0, 0, NSDate.Now),
                View = label
            };

            placesOfInterest.Add(poi);

            placesOfInterest.Add(MakePOI("Golden Gate Bridge", 37.8197, -122.4786));

            placesOfInterest.Add(MakePOI("Golden Gate Park", 37.7697, -122.4769));

            placesOfInterest.Add(MakePOI("Coit Tower", 37.8025, -122.4058));

            placesOfInterest.Add(MakePOI("Bay Bridge", 37.8181, -122.3467));

            placesOfInterest.Add(MakePOI("Transamerica Pyramid", 37.7952, -122.4028));

            placesOfInterest.Add(MakePOI("Ferry Building", 37.7955, -122.3937));

            placesOfInterest.Add(MakePOI("AT&T Park", 37.7786, -122.3892));


            _arView.PlacesOfInterest = placesOfInterest;

            _arView.Start();
        }


        PlaceOfInterest MakePOI(string name, double latitude, double longitude) {
            var label = new UILabel
            {
                AdjustsFontSizeToFitWidth = false,
                Opaque = false,
                BackgroundColor = new UIColor(0.1f, 0.1f, 0.1f, 0.5f),
                Center = new PointF(200.0f, 200.0f),
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Text = name,
                Hidden = true
            };
            var size = label.StringSize(label.Text, label.Font);
            label.Bounds = new RectangleF(0.0f, 0.0f, size.Width, size.Height);
            var poi = new PlaceOfInterest
            {
                Location = new CLLocation(new CLLocationCoordinate2D(latitude, longitude), 0.0, 0, 0, NSDate.Now),
                View = label
            };
            return poi;
        }



        public override void ViewDidDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            _arView.Stop();
        }
    }
}
