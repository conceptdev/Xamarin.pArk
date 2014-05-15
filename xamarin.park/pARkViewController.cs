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

        public pARkViewController() : base("pARkViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            // Perform any additional setup after loading the view, typically from a nib.
            View = _arView = new ARView();

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

            var placesOfInterest = new List<PlaceOfInterest>();
            placesOfInterest.Add(poi);

            _arView.PlacesOfInterest = placesOfInterest;

            _arView.Start();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            _arView.Stop();
        }
    }
}

