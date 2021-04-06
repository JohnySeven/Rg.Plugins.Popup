using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.WPF.Renderers;
using System;
using System.Threading.Tasks;
using System.Windows;
using XF = Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.WPF;
using Application = System.Windows.Application;
using System.Windows.Controls;
using System.Windows.Media;

[assembly: ExportRenderer(typeof(PopupPage), typeof(PopupPageRenderer))]
namespace Rg.Plugins.Popup.WPF.Renderers
{
    [Preserve(AllMembers = true)]
    public class PopupPageRenderer : PageRenderer
    {
        internal Grid Container { get; private set; }

        private PopupPage CurrentElement => (PopupPage)Element;

        [Preserve]
        public PopupPageRenderer()
        {

        }

        private Grid InjectGrid()
        {
            var grid = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            var mainWindow = Application.Current.MainWindow;
            var border = VisualTreeHelper.GetChild(mainWindow, 0) as Border;
            var mainGrid = border.Child as Grid;
            Grid.SetRow(grid, 1);
            Grid.SetRowSpan(grid, 2);
            mainGrid.Children.Add(grid);

            return grid;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XF.Page> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Destroy();
            }

            Container = InjectGrid();
            Container.Children.Add(Control);
            CurrentElement.CloseWhenBackgroundIsClicked = true;
            Container.SizeChanged += Container_SizeChanged;
            UpdateElementSize();
        }

        private void Container_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateElementSize();
        }

        private void UpdateElementSize()
        {
            if (Container.ActualWidth > 0 && Container.ActualHeight > 0)
            {
                CurrentElement.BatchBegin();
                CurrentElement.Layout(new XF.Rectangle(0.0, 0.0, Container.ActualWidth, Container.ActualHeight));
                CurrentElement.BatchCommit();
            }
        }

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    if(disposing)
        //    {
        //        Destroy();
        //    }
        //}

        internal void Destroy()
        {
            if (Container != null)
            {
                var parentGrid = Container.Parent as Grid;
                parentGrid.Children.Remove(Container);
                Container.SizeChanged -= Container_SizeChanged;
                Container = null;
                Element.BindingContext = null;

            }
        }
    }
}
