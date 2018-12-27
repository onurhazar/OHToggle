using System;
using UIKit;
using CoreGraphics;

namespace OHToggleLib
{
    public class OHToggle : UIControl, IUIGestureRecognizerDelegate
    {
        #region Private Props

        const double animationDuration = 0.3;
        nfloat borderMargin = 17;
        CGRect defaultFrame = new CGRect(0, 0, 50, 30);

        UIView backgroundView;
        OHToggleThumb thumbView;
        CGPoint startTrackingPoint = CGPoint.Empty;
        CGRect startThumbFrame = CGRect.Empty;
        bool switchValue;
        bool ignoreTap;
        UITapGestureRecognizer tapGestureRecognizer;

        CGRect _originalThumbRect;

        #endregion

        #region Public Props

        //set toggle on/off
        public bool On
        {
            get
            {
                return switchValue;
            }
            set
            {
                switchValue = value;
                SetOn(switchValue, false);
            }
        }

        public UIColor ThumbTintColor
        {
            get
            {
                return thumbView.ThumbTintColor;
            }
            set
            {
                thumbView.ThumbTintColor = value;
            }
        }

        public UIColor OnThumbTintColor
        {
            get
            {
                return thumbView.OnThumbTintColor;
            }
            set
            {
                thumbView.OnThumbTintColor = value;
            }
        }

        public UIColor ShadowColor
        {
            get
            {
                return thumbView.ShadowColor;
            }
            set
            {
                thumbView.ShadowColor = value;
            }
        }

        public nfloat MaxThumbOffset
        {
            get
            {
                return Bounds.Width - thumbView.Frame.Width - borderMargin * 2;
            }
        }

        public CGRect OriginalThumbRect
        {
            get
            {
                var squareRect = new CGRect(0, 0, Bounds.Height, Bounds.Height);
                var insets = new UIEdgeInsets(borderMargin, borderMargin, borderMargin, borderMargin);
                _originalThumbRect = insets.InsetRect(squareRect);
                //_originalThumbRect = InsetRect(squareRect, insets);
                return _originalThumbRect;
            }
            set
            {
                _originalThumbRect = value;
            }
        }

        #endregion

        #region Ctor

        public OHToggle(CGRect frame)
        {
            Frame = frame.IsEmpty ? defaultFrame : frame;
            SetUp();
        }

        #endregion

        #region Initialization

        private void SetUp()
        {
            backgroundView = new UIView(Bounds);
            backgroundView.BackgroundColor = UIColor.White;
            backgroundView.Layer.CornerRadius = (nfloat)(Frame.Height * 0.5);
            backgroundView.UserInteractionEnabled = false;
            backgroundView.ClipsToBounds = true;
            AddSubview(backgroundView);

            thumbView = new OHToggleThumb(OriginalThumbRect);
            AddSubview(thumbView);

            On = false;

            tapGestureRecognizer = new UITapGestureRecognizer(HandleTapGesture);
            tapGestureRecognizer.Delegate = this;
            AddGestureRecognizer(tapGestureRecognizer);
        }

        void HandleTapGesture()
        {
            if (ignoreTap)
            {
                return;
            }

            if (tapGestureRecognizer.State == UIGestureRecognizerState.Ended)
            {
                thumbView.Toggle(true);
                SetOn(!On, true);
            }
        }

        public void SetOn(bool value, bool animated)
        {
            switchValue = value;
            if (On)
            {
                ShowOn(animated);
            }
            else
            {
                ShowOff(animated);
            }
        }

        //Updates the looks of the toggle to be in the on position
        void ShowOn(bool animated)
        {
            thumbView.EndTracking(true);

            UIView.Animate(animated ? animationDuration : 0, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState, HandleShowOnAnimationAction, null);
        }

        void HandleShowOnAnimationAction()
        {
            _originalThumbRect.Offset(MaxThumbOffset, 0);
            thumbView.Frame = _originalThumbRect;
        }

        //Updates the looks of the toggle to be in the off position
        void ShowOff(bool animated)
        {
            thumbView.EndTracking(false);

            UIView.Animate(animated ? animationDuration : 0, 0, UIViewAnimationOptions.CurveEaseIn | UIViewAnimationOptions.BeginFromCurrentState, HandleShowOffAnimationAction, null);
        }

        void HandleShowOffAnimationAction()
        {
            thumbView.Frame = OriginalThumbRect;
        }

        #endregion

        #region Override Methods

        public override bool BeginTracking(UITouch uitouch, UIEvent uievent)
        {
            startTrackingPoint = uitouch.LocationInView(this);
            startThumbFrame = thumbView.Frame;
            thumbView.StartTracking();

            return true;
        }

        public override bool ContinueTracking(UITouch uitouch, UIEvent uievent)
        {
            ignoreTap = true;

            var lastPoint = uitouch.LocationInView(this);
            var thumbMinPosition = OriginalThumbRect.X;
            var thumbMaxPosition = OriginalThumbRect.X + MaxThumbOffset;
            var touchXOffset = lastPoint.X - startTrackingPoint.X;

            startThumbFrame.Offset(touchXOffset, 0);
            var desiredFrame = startThumbFrame;
            desiredFrame.X = (nfloat)Math.Min(Math.Max(desiredFrame.X, thumbMinPosition), thumbMaxPosition);
            thumbView.Frame = desiredFrame;


            if (On)
            {
                thumbView.AnimationProgress = (thumbMaxPosition - desiredFrame.X) / MaxThumbOffset;
            }
            else
            {
                thumbView.AnimationProgress = (desiredFrame.X - thumbMinPosition) / MaxThumbOffset;
            }

            return true;
        }

        public override void EndTracking(UITouch uitouch, UIEvent uievent)
        {
            base.EndTracking(uitouch, uievent);

            if (thumbView.Center.X > Bounds.GetMidX())
            {
                SetOn(true, true);
            }
            else
            {
                SetOn(false, true);
            }

            ignoreTap = false;
        }

        public override void CancelTracking(UIEvent uievent)
        {
            base.CancelTracking(uievent);

            if (!ignoreTap)
            {
                return;
            }

            //just animate back to the original value
            if (On)
            {
                ShowOn(true);
            }
            else
            {
                ShowOff(true);
            }

            ignoreTap = false;
        }

        #endregion
    }
}
