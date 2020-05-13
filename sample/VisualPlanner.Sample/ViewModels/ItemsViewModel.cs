using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Control.VisualPlanner.Platforms.Common;
using Control.VisualPlanner.Platforms.Common.Abstractions;
using VisualPlanner.Sample.Util;
using Xamarin.Forms;

namespace VisualPlanner.Sample.ViewModels
{
    public class ItemsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<PlannerItem> PlannerElements { get; set; }
        public PlannerItem SelectedElement { get; set; }
        public ICommand RightCommand { get; }
        public ICommand LeftCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ScaleUpCommand { get; }
        public ICommand ScaleDownCommand { get; }
        public string BackgroundSvg { get; set; }

        public ItemsViewModel()
        {

            RightCommand = new Command(Right);
            LeftCommand = new Command(Left);
            ScaleUpCommand = new Command(ScaleUp);
            ScaleDownCommand = new Command(ScaleDown);
            AddCommand = new Command(Add);
            PlannerElements = new ObservableCollection<PlannerItem>();
            BackgroundSvg = SvgHelper.GetImageString("Background.svg");
        }

        private void ScaleDown()
        {
            if (SelectedElement == null) return;
            if (SelectedElement.Scale < 0.6) return;
            SelectedElement.Scale -= 0.5f;
        }

        private void ScaleUp()
        {
            if (SelectedElement == null) return;
            if (SelectedElement.Scale > 4) return;
            SelectedElement.Scale += 0.5f;
        }

        private void Add(object obj)
        {

            PlannerElements.Add(new SvgPlannerItem(SvgHelper.GetImageString("Star.svg"))
            {
                X = 100f,
                Y = 300f, 
                Width = 100f,
                Height = 100f
            });
        }

        private void Right()
        {
            if (SelectedElement == null) return;
            SelectedElement.Rotation += 30f;
        }

        private void Left()
        {
            if (SelectedElement == null) return;
            SelectedElement.Rotation -= 30f;
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}