using Esri.ArcGISRuntime.Controls;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
#endif

namespace BCFAR.Controls
{
    [TemplatePart(Name = "CompassRotateTransform", Type = typeof(RotateTransform))]
    [TemplatePart(Name = "RootGrid", Type = typeof(Grid))]
    public class Compass : Control
    {
        private RotateTransform _rotateTransform;
        private Grid _rootGrid;

        public Compass()
        {
// For Store and Phone, map the default style
#if NETFX_CORE
            DefaultStyleKey = typeof(Compass);
#endif
        }

// For desktop, map the default style
#if !NETFX_CORE
        static Compass()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Compass), new FrameworkPropertyMetadata(typeof(Compass)));
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

            _rotateTransform = GetTemplateChild("CompassRotateTransform") as RotateTransform;
            _rootGrid = GetTemplateChild("RootGrid") as Grid;

#if !NETFX_CORE
            _rootGrid.MouseDown += _rootGrid_MouseRightButtonDown;
            _rootGrid.TouchDown += _rootGrid_TouchDown;
#else
            _rootGrid.Tapped += _rootGrid_OnTapped;
            _rootGrid.PointerPressed += _rootGrid_OnPointerPressed;
#endif
        }

#if !NETFX_CORE
        private async void _rootGrid_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            await ResetRotationAsync();
        }

        private async void _rootGrid_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            await ResetRotationAsync();
        }
#else
        private async void _rootGrid_OnTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await ResetRotationAsync();
        }

        private async void _rootGrid_OnPointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            await ResetRotationAsync();
        }
#endif

        #region MapView
        public static readonly DependencyProperty MapViewProperty =
            DependencyProperty.Register("MapView", typeof(MapView), typeof(Compass), new PropertyMetadata(null, OnMapViewPropertyChanged));

        private static void OnMapViewPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var compass = obj as Compass;
            var oldMapView = args.OldValue as MapView;
            var newMapView = args.NewValue as MapView;

            if (oldMapView != null)
            {
                oldMapView.PropertyChanged -= compass.OnMapViewPropertyChanged;
            }

            if (newMapView != null)
            {
                newMapView.PropertyChanged += compass.OnMapViewPropertyChanged;
            }
        }

        private void OnMapViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Rotation")
            {
                var rotation = MapView.Rotation;
                Debug.WriteLine($"Map rotation after navigation is [{rotation}]");
                _rotateTransform.Angle = 360 - rotation;
            }
        }

        public MapView MapView
        {
            get { return GetValue(MapViewProperty) as MapView; }
            set { SetValue(MapViewProperty, value); }
        }

#endregion // MapView

#region Private methods

        private async Task ResetRotationAsync()
        {
            Debug.WriteLine($"Rotation reset invoked.");
            await MapView.SetRotationAsync(0);
        }

#endregion
    }
}
