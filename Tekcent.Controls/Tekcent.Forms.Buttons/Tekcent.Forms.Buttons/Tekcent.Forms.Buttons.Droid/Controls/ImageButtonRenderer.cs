using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Views;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.ComponentModel;
using Tekcent.Forms.Buttons;
using Tekcent.Forms.Buttons.Droid.Controls;
using Tekcent.Forms.Buttons.Enums;

[assembly: ExportRenderer(typeof(ImageButton), typeof(ImageButtonRenderer))]
namespace Tekcent.Forms.Buttons.Droid.Controls
{
    public class ImageButtonRenderer : ButtonRenderer
    {
        private static float _density = float.MinValue;

        private int _screenWidth = 0;

        private int _screenHeight = 0;

        public ImageButtonRenderer()
        {
            var widthPixels = Context.Resources.DisplayMetrics.WidthPixels;

            var heightPixels = Context.Resources.DisplayMetrics.HeightPixels;

            var density = Context.Resources.DisplayMetrics.Density;

            _screenWidth = (int)(widthPixels / density);

            _screenHeight = (int)(heightPixels / density);
        }

        private ImageButton ImageButton
        {
            get { return (ImageButton)Element; }
        }

        protected async override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            _density = Resources.DisplayMetrics.Density;

            var targetButton = Control;

            if (targetButton != null) targetButton.SetOnTouchListener(TouchListener.Instance.Value);

            if (Element != null && ImageButton.Source != null) await SetImageSourceAsync(targetButton, ImageButton).ConfigureAwait(false);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && Control != null)
            {
                Control.Dispose();
            }
        }

        private async Task SetImageSourceAsync(Android.Widget.Button targetButton, ImageButton model)
        {
            if (targetButton == null || targetButton.Handle == IntPtr.Zero || model == null) return;

            var source = model.IsEnabled ? model.Source : model.DisabledSource ?? model.Source;

            using (var bitmap = await GetBitmapAsync(source).ConfigureAwait(false))
            {
                if (bitmap == null)
                    targetButton.SetCompoundDrawables(null, null, null, null);
                else
                {
                    var drawable = new BitmapDrawable(bitmap);
                    var tintColor = model.IsEnabled ? model.ImageTintColor : model.DisabledImageTintColor;
                    if (tintColor != Xamarin.Forms.Color.Transparent)
                    {
                        drawable.SetTint(tintColor.ToAndroid());
                        drawable.SetTintMode(PorterDuff.Mode.SrcIn);
                    }

                    if (model.IsFullImage)
                    {
                        model.Text = "";
                    }

                    using (var scaledDrawable = GetScaleDrawable(drawable, GetWidth(
                        model.IsFullImage, drawable, model.ImageWidthRequest), GetHeight(model.IsFullImage, drawable, model.ImageHeightRequest)))
                    {
                        Drawable left = null;
                        Drawable right = null;
                        Drawable top = null;
                        Drawable bottom = null;
                        int padding = 10;
                        targetButton.CompoundDrawablePadding = RequestToPixels(padding);
                        switch (model.Orientation)
                        {
                            case ImageOrientation.ImageToLeft:
                                targetButton.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
                                left = scaledDrawable;
                                break;
                            case ImageOrientation.ImageToRight:
                                targetButton.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
                                right = scaledDrawable;
                                break;
                            case ImageOrientation.ImageOnTop:
                                targetButton.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
                                top = scaledDrawable;
                                break;
                            case ImageOrientation.ImageOnBottom:
                                targetButton.Gravity = GravityFlags.Bottom | GravityFlags.CenterHorizontal;
                                bottom = scaledDrawable;
                                break;
                            case ImageOrientation.ImageCentered:
                                targetButton.Gravity = GravityFlags.Center;
                                top = scaledDrawable;
                                break;
                        }

                        targetButton.SetCompoundDrawables(left, top, right, bottom);
                    }
                }
            }
        }

        private async Task<Bitmap> GetBitmapAsync(ImageSource source)
        {
            var handler = GetHandler(source);
            var returnValue = (Bitmap)null;

            if (handler != null)
                returnValue = await handler.LoadImageAsync(source, Context).ConfigureAwait(false);

            return returnValue;
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ImageButton.SourceProperty.PropertyName ||
                e.PropertyName == ImageButton.DisabledSourceProperty.PropertyName ||
                e.PropertyName == VisualElement.IsEnabledProperty.PropertyName ||
                e.PropertyName == ImageButton.ImageTintColorProperty.PropertyName ||
                e.PropertyName == ImageButton.DisabledImageTintColorProperty.PropertyName)
            {
                await SetImageSourceAsync(Control, ImageButton).ConfigureAwait(false);
            }
        }

        private Drawable GetScaleDrawable(Drawable drawable, int width, int height)
        {
            var returnValue = new ScaleDrawable(drawable, 0, 100, 100).Drawable;

            returnValue.SetBounds(0, 0, RequestToPixels(width), RequestToPixels(height));

            return returnValue;
        }

        public int RequestToPixels(int sizeRequest)
        {
            if (_density == float.MinValue)
            {
                if (Resources.Handle == IntPtr.Zero || Resources.DisplayMetrics.Handle == IntPtr.Zero)
                    _density = 1.0f;
                else
                    _density = Resources.DisplayMetrics.Density;
            }

            return (int)(sizeRequest * _density);
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

        private int GetWidth(bool isFullImage, BitmapDrawable bitmap, int requestedWidth)
        {
            if (isFullImage)
            {
                return _screenWidth - Control.PaddingLeft;
            }
            
            return requestedWidth > 0 ? requestedWidth : bitmap.IntrinsicWidth;
        }

        private int GetHeight(bool isFullImage, BitmapDrawable bitmap, int requestedHeight)
        {
            if (isFullImage)
            {
                var xScale = _screenWidth / bitmap.IntrinsicWidth;

                var absoluteHeight = xScale * bitmap.IntrinsicHeight;

                return absoluteHeight - Control.PaddingTop;
            }

            return requestedHeight > 0 ? requestedHeight : bitmap.IntrinsicHeight;
        }
    }

    //Hot fix for the layout positioning issue on Android as described in http://forums.xamarin.com/discussion/20608/fix-for-button-layout-bug-on-android
    class TouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
    {
        public static readonly Lazy<TouchListener> Instance = new Lazy<TouchListener>(() => new TouchListener());

        /// <summary>
        /// Make TouchListener a singleton.
        /// </summary>
        private TouchListener()
        { }

        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            var buttonRenderer = v.Tag as ButtonRenderer;
            if (buttonRenderer != null && e.Action == MotionEventActions.Down)
            {
                buttonRenderer.Control.Text = buttonRenderer.Element.Text;
            }

            return false;
        }
    }
}