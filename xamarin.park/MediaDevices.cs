using System;
using MonoTouch.AVFoundation;
using MonoTouch.CoreVideo;
using MonoTouch.CoreMedia;
using MonoTouch.Foundation;

namespace Xamarin.pArk
{
    public static class MediaDevices
    {
        private static AVCaptureDevice frontCamera = null;
        public static AVCaptureDevice FrontCamera
        {
            get
            {
                if ( frontCamera == null )
                {
                    frontCamera = getCamera("Front Camera");
                }
                return frontCamera;
            }
        }

        private static AVCaptureDevice backCamera = null;
        public static AVCaptureDevice BackCamera
        {
            get
            {
                if ( backCamera == null )
                {
                    backCamera = getCamera("Back Camera");
                }
                return backCamera;
            }
        }

        private static AVCaptureDevice microphone = null;
        public static AVCaptureDevice Microphone
        {
            get
            {
                if ( microphone == null )
                {
                    microphone = getMicrophone();
                }
                return microphone;
            }
        }

        // TODO - need better method of device detection than localized string
        private static AVCaptureDevice getCamera( string localizedDeviceName )
        {
            var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Video);
            foreach ( AVCaptureDevice device in devices )
            {
                if ( string.Compare( device.LocalizedName, localizedDeviceName, true ) == 0 )
                {
                    return device;
                }
            }
            return null;
        }

        // TODO - need better method of device detection than localized string
        private static AVCaptureDevice getMicrophone()
        {
            var devices = AVCaptureDevice.DevicesWithMediaType(AVMediaType.Audio);
            foreach ( AVCaptureDevice device in devices )
            {
                if ( device.LocalizedName.ToLower().Contains("microphone") == true )
                {
                    return device;
                }
            }
            return null;
        }

    }}

