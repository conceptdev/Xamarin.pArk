using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.CoreLocation;
using System.Net;
using System.Json;
using System.IO;

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

            var request = HttpWebRequest.Create("http://xamarinparkdata.azurewebsites.net/api/PointsOfInterest");
            request.ContentType = "application/json";
            request.Method = "GET";

            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
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
                }
                else
                {
                    using(StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();

                        var obj = JsonObject.Parse(content);
                        if (obj != null)
                        {
                            for(int i = 0; i < obj.Count; i++)
                            {
                                var label = new UILabel
                                {
                                    AdjustsFontSizeToFitWidth = false,
                                    Opaque = false,
                                    BackgroundColor = new UIColor(0.1f, 0.1f, 0.1f, 0.5f),
                                    Center = new PointF(200.0f, 200.0f),
                                    TextAlignment = UITextAlignment.Center,
                                    TextColor = UIColor.White,
                                    Text = obj[i]["Name"],
                                    Hidden = true
                                };
                                var size = label.StringSize(label.Text, label.Font);
                                label.Bounds = new RectangleF(0.0f, 0.0f, size.Width, size.Height);

                                var poi = new PlaceOfInterest
                                {
                                    Location = new CLLocation(new CLLocationCoordinate2D(double.Parse(obj[i]["Latitude"].ToString()), double.Parse(obj[i]["Longitude"].ToString())), 0.0, 0, 0, NSDate.Now),
                                    View = label
                                };

                                placesOfInterest.Add(poi);
                            }
                        }
                    }
                }
            }

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

