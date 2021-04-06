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

            var renderer = (PopupPageRenderer)page.GetOrCreateRenderer();
            page.ForceLayout();
            await Task.Delay(5);
        }

        public Task RemoveAsync(PopupPage page)
        {
            var renderer = (PopupPageRenderer)page.GetOrCreateRenderer();
            var popupGrid = renderer.Container;

            if (popupGrid != null)
            {
                Cleanup(page);
                page.Parent = null;
                renderer.Destroy();
            }

            return Task.CompletedTask;
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
                    descendant.BindingContext = null;
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
