using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.WPF.Impl;
using Rg.Plugins.Popup.WPF.Renderers;
using Rg.Plugins.Popup.WPF.Extensions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using XPlatform = Xamarin.Forms.Platform.WPF.Platform;
using System.Windows.Media;

[assembly: Dependency(typeof(PopupPlatformWPF))]
namespace Rg.Plugins.Popup.WPF.Impl
{    
    [Preserve(AllMembers = true)]
    internal class PopupPlatformWPF : IPopupPlatform
    {
        private IPopupNavigation PopupNavigationInstance => PopupNavigation.Instance;

        public event EventHandler OnInitialized
        {
            add => Popup.OnInitialized += value;
            remove => Popup.OnInitialized -= value;
        }

        public bool IsInitialized => Popup.IsInitialized;

        public bool IsSystemAnimationEnabled => true;

        [Preserve]
        public PopupPlatformWPF()
        {
            //TODO onBackPressed subscribe
        }

        public async Task AddAsync(PopupPage page)
        {
            page.Parent = Application.Current.MainPage;

            var popup = new System.Windows.Window()
            {
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                BorderThickness = new System.Windows.Thickness(),
                WindowStyle = System.Windows.WindowStyle.None,
                ShowInTaskbar = false,
                Owner = System.Windows.Application.Current.MainWindow
            };

            var renderer = (PopupPageRenderer)page.GetOrCreateRenderer();

            renderer.Prepare(popup);
            popup.Content = renderer.Control;
            page.ForceLayout();

            popup.Show();

            await Task.Delay(5);
        }

        public async Task RemoveAsync(PopupPage page)
        {
            var renderer = (PopupPageRenderer)page.GetOrCreateRenderer();
            var popup = renderer.Container;

            if (popup != null)
            {
                renderer.Dispose();

                Cleanup(page);
                page.Parent = null;
                popup.Content = null;
                popup.Close();
            }

            await Task.Delay(5);
        }

        internal static void Cleanup(VisualElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            var elementRenderer = XPlatform.GetRenderer(element);
            foreach (Element descendant in element.Descendants())
            {
                if (descendant is VisualElement child)
                {
                    var childRenderer = XPlatform.GetRenderer(child);
                    if (childRenderer != null)
                    {
                        childRenderer.Dispose();
                        XPlatform.SetRenderer(child, null);
                    }
                }
            }
            if (elementRenderer == null)
                return;

            elementRenderer.Dispose();
            XPlatform.SetRenderer(element, null);
        }
    }
}
