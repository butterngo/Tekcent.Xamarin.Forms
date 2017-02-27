using Android.Graphics;
using Android.Runtime;
using System;
using System.ComponentModel;
using Tekcent.Forms.Images.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Tekcent.Forms.Images;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using Tekcent.Forms.Images.Droid.Helpers;

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

        double _imageWidth;
        double _imageHeight;

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var image = sender as Image;
            if (image != null)
            {
                _imageHeight = image.Height;
                _imageWidth = image.Width;
            }

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
                int imageWidth;
                int imageHeight;
                GetWidthAndHeight(out imageWidth, out imageHeight);

                float xSpace = (Width - imageWidth) / 2;
                
                float ySpace = (Height - imageHeight) / 2;

                float left = xSpace;

                float right = Width - xSpace;

                float top = ySpace;

                float bot = Height - ySpace;

                RectF rectF = new RectF(left, top, right, bot);

                var path = new Path();

                path.AddRoundRect(rectF, SquareImage.BorderRadius, SquareImage.BorderRadius, Path.Direction.Ccw);

                var density = Context.Resources.DisplayMetrics.Density;

                canvas.Save();

                canvas.ClipPath(path);
                
                var paint = new Paint();

                paint.AntiAlias = true;

                paint.SetStyle(Paint.Style.Fill);

                paint.Color = ((SquareImage)Element).FillColor.ToAndroid();

                canvas.DrawPath(path, paint);

                paint.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Unable to create square image: " + ex);
            }

            return base.DrawChild(canvas, child, drawingTime);
        }

        private void GetWidthAndHeight(out int width, out int height)
        {
            var bitmap = AsyncHelpers.RunSync(() => GetBitmapAsync(Element.Source));
            var drawable = new BitmapDrawable(bitmap);
            width = drawable.IntrinsicWidth;
            height = drawable.IntrinsicHeight;
        }

        private async Task<Bitmap> GetBitmapAsync(ImageSource source)
        {
            var handler = GetHandler(source);
            var returnValue = (Bitmap)null;

            if (handler != null)
                returnValue = await handler.LoadImageAsync(source, Context).ConfigureAwait(false);

            return returnValue;
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
    }
}