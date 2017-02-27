using CoreGraphics;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Tekcent.Forms.Buttons;
using Tekcent.Forms.Buttons.Enums;
using Tekcent.Forms.Buttons.iOS.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ImageButton), typeof(ImageButtonRenderer))]
namespace Tekcent.Forms.Buttons.iOS.Controls
{
    public class ImageButtonRenderer : ButtonRenderer
    {
        private const int CONTROL_PADDING = 2;

        private const string IPAD = "iPad";

        private ImageButton ImageButton
        {
            get { return (ImageButton)Element; }
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            var imageButton = ImageButton;
            var targetButton = Control;
            if (imageButton != null && targetButton != null && imageButton.Source != null)
            {
                // Matches Android ImageButton behavior
                targetButton.LineBreakMode = UILineBreakMode.WordWrap;

                var width = GetWidth(imageButton.ImageWidthRequest);

                var height = GetHeight(imageButton.ImageHeightRequest);
                
                await SetupImages(imageButton, targetButton, width, height);
                
                switch (imageButton.Orientation)
                {
                    case ImageOrientation.ImageToLeft:
                        AlignToLeft(targetButton);
                        break;
                    case ImageOrientation.ImageToRight:
                        AlignToRight(imageButton.ImageWidthRequest, targetButton);
                        break;
                    case ImageOrientation.ImageOnTop:
                        AlignToTop(imageButton.ImageHeightRequest, imageButton.ImageWidthRequest, targetButton, imageButton.WidthRequest);
                        break;
                    case ImageOrientation.ImageOnBottom:
                        AlignToBottom(imageButton.ImageHeightRequest, imageButton.ImageWidthRequest, targetButton);
                        break;
                }
            }
        }
        
        protected async override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            
            if (e.PropertyName == ImageButton.SourceProperty.PropertyName ||
                e.PropertyName == ImageButton.DisabledSourceProperty.PropertyName ||
                e.PropertyName == ImageButton.ImageTintColorProperty.PropertyName ||
                e.PropertyName == ImageButton.DisabledImageTintColorProperty.PropertyName)
            {
                var sourceButton = Element as ImageButton;
                if (sourceButton != null && sourceButton.Source != null)
                {
                    var imageButton = ImageButton;
                    var targetButton = Control;
                    if (imageButton != null && targetButton != null && imageButton.Source != null)
                    {
                        await SetupImages(imageButton, targetButton, imageButton.ImageWidthRequest, imageButton.ImageHeightRequest);
                    }
                }
            }
        }

        private async Task SetupImages(ImageButton imageButton, UIButton targetButton, int width, int height)
        {
            UIColor tintColor = imageButton.ImageTintColor == Color.Transparent ? null : imageButton.ImageTintColor.ToUIColor();

            UIColor disabledTintColor = imageButton.DisabledImageTintColor == Color.Transparent ? null : imageButton.DisabledImageTintColor.ToUIColor();

            await SetImageAsync(imageButton.Source, width, height, targetButton, UIControlState.Normal, tintColor);

            if (imageButton.DisabledSource != null || disabledTintColor != null)
            {
                await SetImageAsync(imageButton.DisabledSource ?? imageButton.Source, width, height, targetButton, UIControlState.Disabled, disabledTintColor);
            }
        }

        private async Task SetImageAsync(ImageSource source, int widthRequest, int heightRequest, UIButton targetButton,
            UIControlState state = UIControlState.Normal, UIColor tintColor = null)
        {
            var handler = GetHandler(source);
            using (UIImage image = await handler.LoadImageAsync(source))
            {
                if (image == null)
                {
                    throw new Exception($"Image named [{((FileImageSource)source).File}] is not available");
                }

                UIImage scaled = image;

                if (heightRequest > 0 && widthRequest > 0 && (image.Size.Height != heightRequest || image.Size.Width != widthRequest))
                {
                    scaled = scaled.Scale(new CGSize(widthRequest, heightRequest));
                }

                if (tintColor != null)
                {
                    targetButton.TintColor = tintColor;
                    targetButton.SetImage(scaled.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), state);
                }
                else
                    targetButton.SetImage(scaled.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), state);
            }
        }

        private static void AlignToLeft(UIButton targetButton)
        {
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Left;

            var titleInsets = new UIEdgeInsets(0, CONTROL_PADDING, 0, -1 * CONTROL_PADDING);
            targetButton.TitleEdgeInsets = titleInsets;
        }

        private static void AlignToRight(int widthRequest, UIButton targetButton)
        {
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Right;

            var titleInsets = new UIEdgeInsets(0, 0, 0, widthRequest + CONTROL_PADDING);

            targetButton.TitleEdgeInsets = titleInsets;
            var imageInsets = new UIEdgeInsets(0, widthRequest, 0, -1 * widthRequest);
            targetButton.ImageEdgeInsets = imageInsets;
        }

        private static void AlignToTop(int heightRequest, int widthRequest, UIButton targetButton, double buttonWidthRequest)
        {
            targetButton.VerticalAlignment = UIControlContentVerticalAlignment.Top;
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Center;
            targetButton.TitleLabel.LineBreakMode = UIKit.UILineBreakMode.WordWrap;

            targetButton.SizeToFit();

            var titleWidth = targetButton.TitleLabel.IntrinsicContentSize.Width;
            CGSize titleSize = targetButton.TitleLabel.Frame.Size;

            UIEdgeInsets titleInsets;
            UIEdgeInsets imageInsets;

            if (UIDevice.CurrentDevice.Model.Contains(IPAD))
            {
                titleInsets = new UIEdgeInsets(heightRequest, Convert.ToInt32(-1 * widthRequest / 2), -1 * heightRequest, Convert.ToInt32(widthRequest / 2));
                imageInsets = new UIEdgeInsets(0, Convert.ToInt32(titleWidth / 2), 0, -1 * Convert.ToInt32(titleWidth / 2));
            }
            else
            {
                titleInsets = new UIEdgeInsets(heightRequest, Convert.ToInt32(-1 * widthRequest / 2), -1 * heightRequest, Convert.ToInt32(widthRequest / 2));
                imageInsets = new UIEdgeInsets(0, Convert.ToInt32(targetButton.IntrinsicContentSize.Width / 2 - widthRequest / 2 - titleSize.Width / 2), 0, Convert.ToInt32(-1 * (targetButton.IntrinsicContentSize.Width / 2 - widthRequest / 2 + titleSize.Width / 2)));
            }

            targetButton.TitleEdgeInsets = titleInsets;
            targetButton.ImageEdgeInsets = imageInsets;
        }

        private static void AlignToBottom(int heightRequest, int widthRequest, UIButton targetButton)
        {
            targetButton.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
            targetButton.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
            targetButton.TitleLabel.TextAlignment = UITextAlignment.Center;
            targetButton.SizeToFit();
            var titleWidth = targetButton.TitleLabel.IntrinsicContentSize.Width;

            UIEdgeInsets titleInsets;
            UIEdgeInsets imageInsets;

            if (UIDevice.CurrentDevice.Model.Contains(IPAD))
            {
                titleInsets = new UIEdgeInsets(-1 * heightRequest, Convert.ToInt32(-1 * widthRequest / 2), heightRequest, Convert.ToInt32(widthRequest / 2));
                imageInsets = new UIEdgeInsets(0, titleWidth / 2, 0, -1 * titleWidth / 2);
            }
            else
            {
                titleInsets = new UIEdgeInsets(-1 * heightRequest, -1 * widthRequest, heightRequest, widthRequest);
                imageInsets = new UIEdgeInsets(0, 0, 0, 0);
            }

            targetButton.TitleEdgeInsets = titleInsets;
            targetButton.ImageEdgeInsets = imageInsets;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (ImageButton.Orientation == ImageOrientation.ImageToRight)
            {
                var imageInsets = new UIEdgeInsets(0, Control.Frame.Size.Width - CONTROL_PADDING - ImageButton.ImageWidthRequest, 0, 0);
                Control.ImageEdgeInsets = imageInsets;
            }
        }

        private static IImageSourceHandler GetHandler(ImageSource source)
        {
            IImageSourceHandler returnValue = null;
            if (source is UriImageSource)
            {
#if WINDOWS_UWP || WINDOWS_APP || WINDOWS_PHONE_APP
                returnValue = new UriImageSourceHandler();
#else
                returnValue = new ImageLoaderSourceHandler();
#endif
            }
            else if (source is FileImageSource)
            {
                returnValue = new FileImageSourceHandler();
            }
            else if (source is StreamImageSource)
            {
#if WINDOWS_UWP || WINDOWS_APP || WINDOWS_PHONE_APP
                returnValue = new StreamImageSourceHandler();
#else
                returnValue = new StreamImagesourceHandler();
#endif
            }
            return returnValue;
        }

        private int GetWidth(int requestedWidth)
        {
            const int DefaultWidth = 50;
            return requestedWidth <= 0 ? DefaultWidth : requestedWidth;
        }

        private int GetHeight(int requestedHeight)
        {
            const int DefaultHeight = 50;
            return requestedHeight <= 0 ? DefaultHeight : requestedHeight;
        }
    }
}
