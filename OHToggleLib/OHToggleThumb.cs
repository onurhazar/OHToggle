using System;
using CoreAnimation;
using Foundation;
using UIKit;
using CoreGraphics;
using ObjCRuntime;

namespace OHToggleLib
{
    public class OHToggleThumb : UIView
    {
        #region Private Props

        const double animationDuration = 0.3;
        const float thumbSignWidth = 7;
        nfloat borderMargin = 17;
        CGRect defaultFrame = new CGRect(0, 0, 50, 30);

        UIView backgroundView;
        UIView thumbSignView1;
        UIView thumbSignView2;
        CAAnimationGroup animationGroup1;
        CAAnimationGroup animationGroup2;
        CAAnimationGroup thumbAnimationGroup;
        bool isOn;
        bool isTracking;

        nfloat _animationProgress;
        UIColor _thumbTintColor = UIColor.White;
        UIColor _onThumbTintColor = UIColor.White;
        UIColor _shadowColor = UIColor.Gray;

        #endregion

        #region Public Props

        public nfloat AnimationProgress
        {
            get
            {
                return _animationProgress;
            }
            set
            {
                _animationProgress = value;
                if (_animationProgress > 1)
                {
                    _animationProgress = 1;
                }
                if (_animationProgress < 0)
                {
                    _animationProgress = 0;
                }

                thumbSignView1.Layer.TimeOffset = animationDuration * AnimationProgress;
                thumbSignView2.Layer.TimeOffset = animationDuration * AnimationProgress;
                backgroundView.Layer.TimeOffset = animationDuration * AnimationProgress;
            }
        }

        public UIColor ThumbTintColor
        {
            get
            {
                return _thumbTintColor;
            }
            set
            {
                _thumbTintColor = value;
                backgroundView.BackgroundColor = isOn ? _onThumbTintColor : value;
            }
        }

        public UIColor OnThumbTintColor
        {
            get
            {
                return _onThumbTintColor;
            }
            set
            {
                _onThumbTintColor = value;
                backgroundView.BackgroundColor = isOn ? value : _thumbTintColor;
            }
        }

        public UIColor ShadowColor
        {
            get
            {
                return _shadowColor;
            }
            set
            {
                _shadowColor = value;
                backgroundView.Layer.ShadowColor = value.CGColor;
            }
        }

        #endregion

        #region Ctor

        public OHToggleThumb(CGRect frame)
        {
            if (frame.IsEmpty)
            {
                var squareRect = new CGRect(0, 0, defaultFrame.Height, defaultFrame.Height);
                var insets = new UIEdgeInsets(borderMargin, borderMargin, borderMargin, borderMargin);
                var frameWithInset = insets.InsetRect(squareRect);
            }
            else
            {
                Frame = frame;
            }

            Setup();
        }

        #endregion

        #region Initialization

        void Setup()
        {
            backgroundView = new UIView(Bounds);
            backgroundView.Layer.CornerRadius = (nfloat)(Frame.Height * 0.5);
            backgroundView.Layer.ShadowPath = UIBezierPath.FromRoundedRect(Bounds, backgroundView.Layer.CornerRadius).CGPath;
            backgroundView.Layer.ShadowRadius = 4;
            backgroundView.Layer.ShadowOpacity = 0;
            backgroundView.Layer.ShadowColor = _shadowColor.CGColor;
            backgroundView.Layer.ShadowOffset = new CGSize(0, 7);
            backgroundView.Layer.MasksToBounds = false;

            AddSubview(backgroundView);
            UserInteractionEnabled = false;

            SetupSignViews();
        }

        public void Toggle(bool animated)
        {
            ResetAnimations();
            isTracking = false;
            isOn = !isOn;

            thumbSignView1.Layer.Speed = animated ? 1 : 0;
            thumbSignView2.Layer.Speed = animated ? 1 : 0;
            backgroundView.Layer.Speed = animated ? 1 : 0;

            thumbSignView1.Layer.AddAnimation(animationGroup1, "tickAnimation");
            thumbSignView2.Layer.AddAnimation(animationGroup2, "tickAnimation");
            backgroundView.Layer.AddAnimation(thumbAnimationGroup, "thumbAnimation");
        }

        public void StartTracking()
        {
            if (isTracking)
            {
                return;
            }

            ResetAnimations();

            isTracking = true;

            thumbSignView1.Layer.Speed = 0;
            thumbSignView2.Layer.Speed = 0;
            backgroundView.Layer.Speed = 0;

            thumbSignView1.Layer.AddAnimation(animationGroup1, "tickAnimation");
            thumbSignView2.Layer.AddAnimation(animationGroup2, "tickAnimation");
            backgroundView.Layer.AddAnimation(thumbAnimationGroup, "thumbAnimation");
        }

        public void EndTracking(bool value)
        {
            isOn = value;

            if (!isTracking)
            {
                return;
            }

            isTracking = false;

            thumbSignView1.Layer.TimeOffset = animationDuration;
            thumbSignView2.Layer.TimeOffset = animationDuration;
            backgroundView.Layer.TimeOffset = animationDuration;
        }

        //X sign
        void SetupSignViews()
        {
            thumbSignView1 = CreateThumbSignView();
            thumbSignView1.Transform = CGAffineTransform.MakeRotation((nfloat)(Math.PI / 4));
            AddSubview(thumbSignView1);

            thumbSignView2 = CreateThumbSignView();
            thumbSignView2.Transform = CGAffineTransform.MakeRotation(-(nfloat)(Math.PI / 4));
            AddSubview(thumbSignView2);
        }

        UIView CreateThumbSignView()
        {
            var thumbSignHeight = Bounds.Height / 2 - thumbSignWidth;
            var thumbSignView = new UIView(new CGRect(Bounds.Width / 2 - 4, Bounds.Height / 2 - thumbSignHeight / 2, thumbSignWidth, thumbSignHeight));
            thumbSignView.Layer.CornerRadius = thumbSignWidth / 2;
            thumbSignView.BackgroundColor = UIColor.White;
            return thumbSignView;
        }

        //X -> √
        void ResetAnimations()
        {
            thumbSignView1.Layer.RemoveAllAnimations();
            thumbSignView2.Layer.RemoveAllAnimations();
            backgroundView.Layer.RemoveAllAnimations();

            thumbSignView1.Layer.TimeOffset = 0;
            thumbSignView2.Layer.TimeOffset = 0;
            backgroundView.Layer.TimeOffset = 0;

            var fillMode = CAFillMode.Forwards;
            var thumbSignHeight = Bounds.Height / 2 - thumbSignWidth;

            var rotationAnimation1 = BaseAnimation("transform.rotation", NSNumber.FromDouble(Math.PI / 4), NSNumber.FromDouble(-Math.PI + Math.PI / 4));
            var scaleAnimation1 = BaseAnimation("bounds.size.height", NSNumber.FromNFloat(thumbSignHeight), NSNumber.FromNFloat(thumbSignHeight + 7));
            var moveAnimation1 = BaseAnimation("position", NSValue.FromCGPoint(thumbSignView1.Center), NSValue.FromCGPoint(new CGPoint(thumbSignView2.Center.X + 10, thumbSignView2.Center.Y + 4)));

            animationGroup1 = new CAAnimationGroup();
            animationGroup1.Duration = animationDuration;
            animationGroup1.Animations = new CAAnimation[] { rotationAnimation1, scaleAnimation1, moveAnimation1 };
            animationGroup1.RemovedOnCompletion = false;
            animationGroup1.FillMode = fillMode;

            var rotationAnimation2 = BaseAnimation("transform.rotation", NSNumber.FromDouble(-(Math.PI / 4)), NSNumber.FromDouble(-Math.PI - Math.PI / 4));
            var scaleAnimation2 = BaseAnimation("bounds.size.height", NSNumber.FromNFloat(thumbSignHeight), NSNumber.FromNFloat(thumbSignHeight / 2 + 6));
            var moveAnimation2 = BaseAnimation("position", NSValue.FromCGPoint(thumbSignView2.Center), NSValue.FromCGPoint(new CGPoint(thumbSignView2.Center.X - 25, thumbSignView2.Center.Y + 17)));

            animationGroup2 = new CAAnimationGroup();
            animationGroup2.Duration = animationDuration;
            animationGroup2.Animations = new CAAnimation[] { rotationAnimation2, scaleAnimation2, moveAnimation2 };
            animationGroup2.RemovedOnCompletion = false;
            animationGroup2.FillMode = fillMode;

            var shadowOpacityAnimation = BaseAnimation("shadowOpacity", NSNumber.FromDouble(0), NSNumber.FromDouble(0.5));
            var bgColorAnimation = BaseAnimation("backgroundColor", Runtime.GetINativeObject<NSValue>(_thumbTintColor.CGColor.Handle, true), Runtime.GetINativeObject<NSValue>(_onThumbTintColor.CGColor.Handle, true));

            thumbAnimationGroup = new CAAnimationGroup();
            thumbAnimationGroup.Duration = animationDuration;
            thumbAnimationGroup.Animations = new CAAnimation[] { shadowOpacityAnimation, bgColorAnimation };
            thumbAnimationGroup.RemovedOnCompletion = false;
            thumbAnimationGroup.FillMode = fillMode;
        }

        CABasicAnimation BaseAnimation(string keyPath, NSValue fromValue, NSValue toValue)
        {
            var animation = CABasicAnimation.FromKeyPath(keyPath);
            animation.Duration = animationDuration;
            if (isOn)
            {
                animation.SetFrom(toValue);
                animation.SetTo(fromValue);
            }
            else
            {
                animation.SetFrom(fromValue);
                animation.SetTo(toValue);
            }
            return animation;
        }

        #endregion
    }
}
