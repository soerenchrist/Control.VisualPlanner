using System;
using Control.VisualPlanner.Platforms.Common.Models;
using Xamarin.Forms;

namespace VisualPlanner.Sample.Views
{
    public partial class ItemsPage : ContentPage
    {
        public ItemsPage()
        {
            InitializeComponent();
        }
        private void MoveOnTapped(object sender, EventArgs e)
        {
            Panel.PlannerMode = PlannerMode.Move;
        }

        private void DrawOnTapped(object sender, EventArgs e)
        {
            Panel.PlannerMode = PlannerMode.Draw;
        }

        private void ArrowOnTapped(object sender, EventArgs e)
        {
            Panel.PlannerMode = PlannerMode.Arrows;
        }
    }
}