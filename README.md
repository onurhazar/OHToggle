# OHToggle
An animated toggle written in C#

![Demo](https://github.com/onurhazar/OHToggle/blob/master/OHToggle.gif)

## Requirements
iOS 8.3+
Xcode 8.0+

## How to Use
```C#
var ohToggle = new OHToggle(new CGRect(30, 246, 315, 175))
{
    ThumbTintColor = UIColor.Brown,
    OnThumbTintColor = UIColor.Purple,
    ShadowColor = UIColor.LightGray
};
View.BackgroundColor = UIColor.Orange;
View.AddSubview(ohToggle);

```
