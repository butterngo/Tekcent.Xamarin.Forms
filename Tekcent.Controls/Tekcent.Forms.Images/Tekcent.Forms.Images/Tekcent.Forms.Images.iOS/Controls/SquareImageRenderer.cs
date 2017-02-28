using CoreGraphics;
using System;
using System.ComponentModel;
using Tekcent.Forms.Images;
using Tekcent.Forms.Images.iOS.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SquareImage), typeof(SquareImageRenderer))]
namespace Tekcent.Forms.Images.iOS.Controls
{
    public class SquareImageRenderer : ImageRenderer
    {
        private SquareImage SquareImage => Element as SquareImage;

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (Control == null || e.OldElement != null || Element == null || Element.Aspect != Aspect.Fill)
                return;
            
            Control.Layer.CornerRadius = (float)(SquareImage.BorderRadius);
            Control.Layer.MasksToBounds = false;
            Control.ClipsToBounds = true;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control == null) return;

            if (Element.Aspect == Aspect.Fill)
            {
                if (e.PropertyName == VisualElement.HeightProperty.PropertyName ||
                    e.PropertyName == VisualElement.WidthProperty.PropertyName)
                {
                    DrawFill();
                }
            }
            else
            {
                if (e.PropertyName == Image.IsLoadingProperty.PropertyName
                    && !Element.IsLoading && Control.Image != null)
                {
                    DrawOther();
                }
            }
        }

        private void DrawOther()
        {
            int height = (int)Control.Image.Size.Height;
            int width = (int)Control.Image.Size.Width;
            
            UIImage image = Control.Image;
            var clipRect = new CGRect(0, 0, width, height);
            var scaled = image.Scale(new CGSize(width, height));
            UIGraphics.BeginImageContextWithOptions(new CGSize(width, height), false, 0f);
            UIBezierPath.FromRoundedRect(clipRect, SquareImage.BorderRadius).AddClip();

            scaled.Draw(new CGRect(0, 0, scaled.Size.Width, scaled.Size.Height));
            UIImage final = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            Control.Image = final;
        }

        private void DrawFill()
        {
            Control.Layer.CornerRadius = SquareImage.BorderRadius;
            Control.Layer.MasksToBounds = false;
            Control.ClipsToBounds = true;
        }
    }
}
