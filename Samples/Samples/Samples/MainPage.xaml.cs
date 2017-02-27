using System;
using Xamarin.Forms;

namespace Samples
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnBtnClicked(object sender, EventArgs e)
        {
            DisplayAlert("Image Button", "You clicked the ImageButton", "OK");
        }
    }
}
