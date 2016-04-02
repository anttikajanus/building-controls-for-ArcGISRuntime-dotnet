using Esri.ArcGISRuntime.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows.Controls;
using System.Windows.Media;
#endif

namespace BCFAR.Controls
{
    [TemplatePart(Name = "ZoomOutButton", Type = typeof(Button))]
    [TemplatePart(Name = "ZoomInButton", Type = typeof(Button))]
    public class Zoom : Control
    {      
        private Button _zoomInButton;
        private Button _zoomOutButton;

        public Zoom()
        {
            // For Store and Phone, map the default style
#if NETFX_CORE
            DefaultStyleKey = typeof(Zoom);
#endif
        }

        // For desktop, map the default style
#if !NETFX_CORE
        static Zoom()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Zoom), new FrameworkPropertyMetadata(typeof(Zoom)));
        }
#endif


#if NETFX_CORE
        protected 
#else
        public
#endif
        override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _zoomInButton = GetTemplateChild("ZoomInButton") as Button;
            _zoomInButton.Click += _zoomIButton_Click;

            _zoomOutButton = GetTemplateChild("ZoomOutButton") as Button;
            _zoomOutButton.Click += _zoomOutButton_Click;

#if !NETFX_CORE
            _zoomInButton.TouchDown += _zoomIButton_TouchDown;
            _zoomOutButton.TouchDown += _zoomOutButton_TouchDown;
#else
            _zoomInButton.Tapped += _zoomInButton_Tapped;
            _zoomOutButton.Tapped += _zoomOutButton_Tapped;
#endif

            CheckEnabledState();
        }

        #region MapView
        public static readonly DependencyProperty MapViewProperty =
            DependencyProperty.Register("MapView", typeof(MapView), typeof(Zoom), new PropertyMetadata(null, OnMapViewPropertyChanged));

        private static void OnMapViewPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var Zoom = obj as Zoom;
            var newMapView = args.NewValue as MapView;

            if (newMapView != null)
                Zoom.EnableZoom();
            else
                Zoom.DisableZoom();
        }

        public MapView MapView
        {
            get { return GetValue(MapViewProperty) as MapView; }
            set { SetValue(MapViewProperty, value); }
        }

        #endregion // MapView

        #region ZoomInFactor
        public static readonly DependencyProperty ZoomInFactorProperty =
            DependencyProperty.Register("ZoomInFactor", typeof(double), typeof(Zoom), new PropertyMetadata(1.5));

        public double ZoomInFactor
        {
            get { return (double)GetValue(ZoomInFactorProperty); }
            set { SetValue(ZoomInFactorProperty, value); }
        }

        #endregion // ZoomInFactor

        #region ZoomOutFactor
        public static readonly DependencyProperty ZoomOutFactorProperty =
            DependencyProperty.Register("ZoomOutFactor", typeof(double), typeof(Zoom), new PropertyMetadata(0.5));

        public double ZoomOutFactor
        {
            get { return (double)GetValue(ZoomOutFactorProperty); }
            set { SetValue(ZoomOutFactorProperty, value); }
        }

        #endregion // ZoomOutFactor

        #region Private methods

        private async Task ZoomInAsync()
        {
            Debug.WriteLine($"Zoom in invoked.");
            await MapView.ZoomAsync(ZoomInFactor);
        }

        private async Task ZoomOutAsync()
        {
            Debug.WriteLine($"Zoom in invoked.");
            await MapView.ZoomAsync(ZoomOutFactor);
        }

        private void CheckEnabledState()
        {
            if (MapView != null)
            {
                EnableZoom();
            }
            else
            {
                DisableZoom();
            }
        }

        private void EnableZoom()
        {
            if (_zoomOutButton != null)
                _zoomOutButton.IsEnabled = true;
            if (_zoomInButton != null)
                _zoomInButton.IsEnabled = true;
        }

        private void DisableZoom()
        {
            if (_zoomOutButton != null)
                _zoomOutButton.IsEnabled = false;
            if (_zoomInButton != null)
                _zoomInButton.IsEnabled = false;
        }

        private async void _zoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            await ZoomOutAsync();
        }

        private async void _zoomIButton_Click(object sender, RoutedEventArgs e)
        {
            await ZoomInAsync();
        }

#if !NETFX_CORE
        private async void _zoomOutButton_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            await ZoomOutAsync();
        }

        private async void _zoomIButton_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            await ZoomInAsync();
        }
#else
        private async void _zoomOutButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await ZoomOutAsync();
        }

        private async void _zoomInButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await ZoomInAsync();
        }
#endif

        #endregion
    }
}
