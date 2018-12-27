using System;
using CoreGraphics;
using OHToggleLib;
using UIKit;

namespace Sample
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var ohToggle = new OHToggle(new CGRect(30, 246, 315, 175))
            {
                ThumbTintColor = UIColor.Brown,
                OnThumbTintColor = UIColor.Purple,
                ShadowColor = UIColor.LightGray
            };
            View.BackgroundColor = UIColor.Orange;
            View.AddSubview(ohToggle);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
