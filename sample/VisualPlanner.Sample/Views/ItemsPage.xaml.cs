using System;
using Xamarin.Forms;

namespace VisualPlanner.Sample.Views
{
    public partial class ItemsPage : ContentPage
    {
        public ItemsPage()
        {
            InitializeComponent();
        }

        private void DrawOnTapped(object sender, EventArgs e)
        {
            Panel.DrawMode = !Panel.DrawMode;
        }
    }
}