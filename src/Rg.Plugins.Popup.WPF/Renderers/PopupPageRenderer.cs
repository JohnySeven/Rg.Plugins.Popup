using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.WPF.Renderers;
using System;
using System.Threading.Tasks;
using System.Windows;
using XF = Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.WPF;
using WinPopup = System.Windows.Controls.Primitives.Popup;
using Application = System.Windows.Application;
using WinPrimitives = System.Windows.Controls.Primitives;
using System.Windows.Controls.Primitives;

[assembly: ExportRenderer(typeof(PopupPage), typeof(PopupPageRenderer))]
namespace Rg.Plugins.Popup.WPF.Renderers
{
    [Preserve(AllMembers = true)]
    public class PopupPageRenderer : PageRenderer
    {
        internal Window Container { get; private set; }

        private PopupPage CurrentElement => (PopupPage)Element;

        [Preserve]
        public PopupPageRenderer()
        {

        }
        
        internal void Prepare(Window container)
        {
            Container = container;
            CurrentElement.CloseWhenBackgroundIsClicked = true;

            var window = Application.Current.MainWindow ?? throw new InvalidOperationException("Wpf MainWindow must be initialized!");

            window.SizeChanged += OnSizeChanged;
            window.LocationChanged += OnLocationChanged;
            window.StateChanged += OnMainWindowStateChanged;

            container.Owner = window;

            UpdatePopupLocation(container, window);
            UpdateElementSize(container);
        }

        private void OnMainWindowStateChanged(object sender, EventArgs e)
        {
            //if(sender is Window window)
            //{
            //    Container.IsOpen = window.WindowState != WindowState.Minimized;
            //}
        }

        private static void UpdatePopupLocation(Window container, Window window)
        {
            container.Width = window.Width;
            container.Height = window.Height;
            container.Left = window.Left;
            container.Top = window.Top;
        }

        private void OnLocationChanged(object sender, EventArgs e)
        {
            UpdatePopupLocation(Container, Application.Current.MainWindow);
            UpdateElementSize(Container);
        }

        internal void Destroy()
        {
            Container = null;

            var window = Application.Current.MainWindow;
            if (window != null)
            {
                window.SizeChanged -= OnSizeChanged;
                window.LocationChanged -= OnLocationChanged;
                window.StateChanged -= OnMainWindowStateChanged;
            }
        }
        
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdatePopupLocation(Container, Application.Current.MainWindow);
            UpdateElementSize(Container);
        }

        private async void UpdateElementSize(Window popup)
        {
            await Task.Delay(50);

            var windowBound = Application.Current.MainWindow.RestoreBounds;
            var visibleBounds = Application.Current.MainWindow.RestoreBounds;

            var top = visibleBounds.Top - windowBound.Top;
            var bottom = windowBound.Bottom - visibleBounds.Bottom;
            var left = visibleBounds.Left - windowBound.Left;
            var right = windowBound.Right - visibleBounds.Right;

            top = Math.Max(0, top);
            bottom = Math.Max(0, bottom);
            left = Math.Max(0, left);
            right = Math.Max(0, right);

            var systemPadding = new Xamarin.Forms.Thickness(left, top, right, bottom);

            CurrentElement.BatchBegin();

            CurrentElement.Padding = systemPadding;
            CurrentElement.Layout(new XF.Rectangle(0.0, 0.0, popup.Width, popup.Height));

            CurrentElement.BatchCommit();
        }
    }
}
