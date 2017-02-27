namespace Tekcent.Forms.Images
{
    using Xamarin.Forms;

    public class SquareImage: Image
    {
        public static readonly BindableProperty BorderThicknessProperty =
         BindableProperty.Create(propertyName: nameof(BorderThickness),
             returnType: typeof(int),
             declaringType: typeof(SquareImage),
             defaultValue: 0);

        public int BorderThickness
        {
            get { return (int)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(propertyName: nameof(BorderColor),
              returnType: typeof(Color),
              declaringType: typeof(SquareImage),
              defaultValue: Color.White);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly BindableProperty FillColorProperty =
            BindableProperty.Create(propertyName: nameof(FillColor),
              returnType: typeof(Color),
              declaringType: typeof(SquareImage),
              defaultValue: Color.Transparent);

        public Color FillColor
        {
            get { return (Color)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }

        public static readonly BindableProperty BorderRadiusProperty =
            BindableProperty.Create(propertyName: nameof(BorderRadius),
              returnType: typeof(int),
              declaringType: typeof(SquareImage),
              defaultValue: 50);

        public float BorderRadius
        {
            get { return (int)GetValue(BorderRadiusProperty); }
            set { SetValue(BorderRadiusProperty, value); }
        }
    }
}
