using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Xamarin.pArk
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private UIWindow _window;
        private pARkViewController _viewController;
        public override void FinishedLaunching(UIApplication application)
        {
            _window = new UIWindow(UIScreen.MainScreen.Bounds);

            _viewController = new pARkViewController();

            _window.RootViewController = _viewController;

            _window.MakeKeyAndVisible();
        }
    }
}

