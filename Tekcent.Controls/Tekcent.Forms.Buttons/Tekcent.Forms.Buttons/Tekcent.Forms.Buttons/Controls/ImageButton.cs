using Tekcent.Forms.Buttons.Enums;
using Xamarin.Forms;

namespace Tekcent.Forms.Buttons
{
    public class ImageButton : Button
    {
        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
            "Source", typeof(ImageSource), typeof(ImageButton), null);

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly BindableProperty DisabledSourceProperty = BindableProperty.Create(
            "DisabledSource", typeof(ImageSource), typeof(ImageButton), null);

        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource DisabledSource
        {
            get { return (ImageSource)GetValue(DisabledSourceProperty); }
            set { SetValue(DisabledSourceProperty, value); }
        }

        public static readonly BindableProperty ImageWidthRequestProperty = BindableProperty.Create(
            "ImageWidthRequest", typeof(int), typeof(ImageButton), 0);

        public int ImageHeightRequest
        {
            get { return (int)GetValue(ImageHeightRequestProperty); }
            set { SetValue(ImageHeightRequestProperty, value); }
        }

        public static readonly BindableProperty ImageHeightRequestProperty = BindableProperty.Create(
            "ImageHeightRequest", typeof(int), typeof(ImageButton), 0);

        public int ImageWidthRequest
        {
            get { return (int)GetValue(ImageWidthRequestProperty); }
            set { SetValue(ImageWidthRequestProperty, value); }
        }

        public static readonly BindableProperty ImageOrientationProperty = BindableProperty.Create(
            "ImageOrientation", typeof(ImageOrientation), typeof(ImageButton), ImageOrientation.ImageToLeft);

        public ImageOrientation Orientation
        {
            get { return (ImageOrientation)GetValue(ImageOrientationProperty); }
            set { SetValue(ImageOrientationProperty, value); }
        }

        public static readonly BindableProperty ImageTintColorProperty = BindableProperty.Create(
            "ImageTintColor", typeof(Color), typeof(ImageButton), Color.Transparent);
 
        public Color ImageTintColor
        {
            get { return (Color)GetValue(ImageTintColorProperty); }
            set { SetValue(ImageTintColorProperty, value); }
        }

        public static readonly BindableProperty DisabledImageTintColorProperty = BindableProperty.Create(
            "DisabledImageTintColor", typeof(Color), typeof(ImageButton), Color.Transparent);

        public Color DisabledImageTintColor
        {
            get { return (Color)GetValue(DisabledImageTintColorProperty); }
            set { SetValue(DisabledImageTintColorProperty, value); }
        }

        public static readonly BindableProperty IsFullImageProperty = BindableProperty.Create(
            "IsFullImage", typeof(bool), typeof(ImageButton), false);

        public bool IsFullImage
        {
            get { return (bool)GetValue(IsFullImageProperty); }
            set { SetValue(IsFullImageProperty, value); }
        }
    }
}
