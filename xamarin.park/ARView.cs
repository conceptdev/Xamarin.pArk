using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.AVFoundation;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreLocation;
using MonoTouch.CoreMotion;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Xamarin.pArk
{
    public class ARView : UIView
    {
        UIView captureView;

        AVCaptureSession captureSession;
        AVCaptureVideoPreviewLayer captureLayer;

        CLLocationManager locationManager;

        CMMotionManager motionManager;

        CADisplayLink displayLink;

        float[] projectionTransform;
        float[] cameraTransform;
        List<float[]> placesOfInterestCoordinates;

        public List<PlaceOfInterest> PlacesOfInterest
        {
            get;
            set;
        }

        public ARView()
        {
            captureView = new UIView(UIScreen.MainScreen.Bounds);
            AddSubview(captureView);
            SendSubviewToBack(captureView);

            projectionTransform = new float[16];
            MathHelpers.CreateProjectionMatrix(ref projectionTransform, (float)(UIScreen.MainScreen.Bounds.Size.Width * 1.0 / UIScreen.MainScreen.Bounds.Size.Height), 0.25f, 1000.0f);
        }

        public void Start()
        {
            StartCameraPreview();
            StartLocation();
            StartDeviceMotion();
            //            StartDisplayLink();
        }
            
        public void Stop()
        {
            StopCameraPreview();
            StopLocation();
            StopDeviceMotion();
            //            StopDisplayLink();
        }

       
        private void StartCameraPreview()
        {
            captureSession = new AVCaptureSession();
            captureSession.AddInput(AVCaptureDeviceInput.FromDevice(MediaDevices.BackCamera));

            captureLayer = new AVCaptureVideoPreviewLayer(captureSession);
            captureLayer.Frame = captureView.Bounds;

            captureView.Layer.AddSublayer(captureLayer);

            captureSession.StartRunning();
        }

        private void StartLocation()
        {
            locationManager = new CLLocationManager
            {
                DistanceFilter = 1, 
                Delegate = new LocationDelegate(this)
            };
            locationManager.StartUpdatingLocation();
        }

        class DistanceAndIndex
        {
            public float distance;
            public int index;
        }

        public void UpdatePlacesOfInterestCoordinates(CLLocation newLocation)
        {
            double myX = 0.0, myY = 0.0, myZ = 0.0;
            MathHelpers.LatLonToEcef(newLocation.Coordinate.Latitude, newLocation.Coordinate.Longitude, 0.0, ref myX, ref myY, ref myZ);
            var orderedDistances = new List<DistanceAndIndex>();

            placesOfInterestCoordinates = new List<float[]>();

            for (int i = 0; i < PlacesOfInterest.Count;i++)
            {
                var poi = PlacesOfInterest[i];

                double poiX = 0.0, poiY = 0.0, poiZ = 0.0, e = 0.0, n = 0.0, u = 0.0;
                MathHelpers.LatLonToEcef(poi.Location.Coordinate.Latitude, poi.Location.Coordinate.Longitude, 0.0, ref poiX, ref poiY, ref poiZ);
                MathHelpers.EcefToEnu(poi.Location.Coordinate.Latitude, poi.Location.Coordinate.Longitude, myX, myY, myZ, poiX, poiY, poiZ, ref e, ref n, ref u);

                var p = new float[4];
                p[0] = (float)n;
                p[1] = -(float)e;
                p[2] = 0.0f;
                p[3] = 1.0f;

                placesOfInterestCoordinates.Add(p);

                var distance = new DistanceAndIndex
                {
                    distance = (float)Math.Sqrt(n*n + e*e),
                    index = i
                };
                orderedDistances.Add(distance);               
            }

            orderedDistances.Sort((A, B) =>
            {
                return 0;
            });

            foreach (var dai in orderedDistances)
            {
                AddSubview(PlacesOfInterest[dai.index].View);
            }
        }

        public override void Draw(RectangleF rect)
        {
            if (placesOfInterestCoordinates == null)
            {
                return;
            }

            var projectionCameraTransform = new float[16];
            MathHelpers.MultiplyMatrixAndMatrix(ref projectionCameraTransform, projectionTransform, cameraTransform);

            for (int i = 0; i < PlacesOfInterest.Count; i++)
            {
                var poi = PlacesOfInterest[i];

                var v = new float[4];
                MathHelpers.MultiplyMatrixAndVector(ref v, projectionCameraTransform, placesOfInterestCoordinates[i]);

                float x = (v[0] / v[3] + 1.0f) * 0.5f;
                float y = (v[1] / v[3] + 1.0f) * 0.5f;

                if (v[2] < 0.0f)
                {
                    poi.View.Center = new PointF(x * Bounds.Size.Width, Bounds.Size.Height - y * Bounds.Size.Height);
                    poi.View.Hidden = false;
                }
                else
                {
                    poi.View.Hidden = true;
                }
            }
        }

        private void StartDeviceMotion()
        {
            motionManager = new CMMotionManager
            {
                ShowsDeviceMovementDisplay = true,
                DeviceMotionUpdateInterval = 1.0/60.0
            };
            //motionManager.StartDeviceMotionUpdates(CMAttitudeReferenceFrame.XTrueNorthZVertical);
            motionManager.StartDeviceMotionUpdates(NSOperationQueue.CurrentQueue, (motion, error) =>
            {
                if(motion != null)
                {
                    cameraTransform = new float[16];
                    MathHelpers.TransformFromCMRotationMatrix(ref cameraTransform, motion.Attitude.RotationMatrix);
                    SetNeedsDisplay();
                }
            });
        }

        private void StartDisplayLink()
        {
            displayLink = CADisplayLink.Create(UpdateDisplay);
            displayLink.FrameInterval = 1;
            displayLink.AddToRunLoop(NSRunLoop.Main, NSRunLoop.UITrackingRunLoopMode);
        }

        void UpdateDisplay()
        {
            CMDeviceMotion motion = motionManager.DeviceMotion;
            if (motion != null)
            {
                cameraTransform = new float[4];
                MathHelpers.TransformFromCMRotationMatrix(ref cameraTransform, motion.Attitude.RotationMatrix);

                SetNeedsDisplay();
            }
        }

        private void StopCameraPreview()
        {
            captureSession.StopRunning();
        }

        private void StopLocation()
        {
            locationManager.StopUpdatingLocation();
        }

        private void StopDeviceMotion()
        {
            motionManager.StopDeviceMotionUpdates();
        }

        private void StopDisplayLink()
        {
            displayLink.Invalidate();
        }
    }
}