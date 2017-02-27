using Android.Graphics;
using Android.Runtime;
using System;
using System.ComponentModel;
using Tekcent.Forms.Images.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Tekcent.Forms.Images;

[assembly: ExportRenderer(typeof(SquareImage), typeof(SquareImageRenderer))]
namespace Tekcent.Forms.Images.Droid.Controls
{
    [Preserve(AllMembers = true)]
    public class SquareImageRenderer: ImageRenderer
    {
        private SquareImage SquareImage
        {
            get
            {
                return (SquareImage)Element;
            }
        }

        public static void Init()
        {
            var temp = DateTime.Now;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                //Only enable hardware accelleration on lollipop
                if ((int)Android.OS.Build.VERSION.SdkInt < 21)
                {
                    SetLayerType(LayerType.Software, null);
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == SquareImage.BorderColorProperty.PropertyName ||
              e.PropertyName == SquareImage.BorderThicknessProperty.PropertyName ||
              e.PropertyName == SquareImage.FillColorProperty.PropertyName)
            {
                this.Invalidate();
            }
        }

        protected override bool DrawChild(Canvas canvas, Android.Views.View child, long drawingTime)
        {
            try
            {
                var path = new Path();

                RectF rectF = new RectF(20, 700, ( Width - 20 ), Height);
                //path.AddRect(rectF, Path.Direction.Ccw);
                path.AddRoundRect(rectF, 100, 100, Path.Direction.Ccw);

                canvas.Save();
                canvas.ClipPath(path);
                
                var paint = new Paint();
                paint.AntiAlias = true;
                paint.SetStyle(Paint.Style.Fill);
                paint.Color = ((SquareImage)Element).FillColor.ToAndroid();
                canvas.DrawPath(path, paint);
                paint.Dispose();

                var result = base.DrawChild(canvas, child, drawingTime);

                canvas.Restore();

                path = new Path();

                rectF = new RectF(20, 700, (Width - 20), Height);
           
                path.AddRoundRect(rectF, 100, 100, Path.Direction.Ccw);

                paint = new Paint();
                paint.AntiAlias = true;
                paint.SetStyle(Paint.Style.Fill);
                paint.Color = ((SquareImage)Element).FillColor.ToAndroid();
                canvas.DrawPath(path, paint);
                paint.Dispose();

                path.Dispose();

                return result;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create square image: " + ex);
            }

            return base.DrawChild(canvas, child, drawingTime);
        }

    }
}