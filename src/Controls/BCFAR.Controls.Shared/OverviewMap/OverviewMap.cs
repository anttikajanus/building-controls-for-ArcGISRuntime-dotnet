using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Layers;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
    [TemplatePart(Name = "OverviewMapView", Type = typeof(MapView))]
    public class OverviewMap : Control
    {
        private MapView _overviewMapView;

        public OverviewMap()
        {
// For Store and Phone, map the default style
#if NETFX_CORE
            DefaultStyleKey = typeof(OverviewMap);
#endif
        }

// For desktop, map the default style
#if !NETFX_CORE
        static OverviewMap()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OverviewMap), new FrameworkPropertyMetadata(typeof(OverviewMap)));
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

            _overviewMapView = GetTemplateChild("OverviewMapView") as MapView;
            _overviewMapView.Map = new Map();
            _overviewMapView.Map.Layers.Add(new ArcGISTiledMapServiceLayer()
            {
                ServiceUri = "http://services.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer"
            });
            _overviewMapView.GraphicsOverlays.First().Graphics.Add(new Graphic());
            _overviewMapView.InteractionOptions.IsEnabled = false;

        }

        #region MapView
        public static readonly DependencyProperty MapViewProperty =
            DependencyProperty.Register("MapView", typeof(MapView), typeof(OverviewMap), new PropertyMetadata(null, OnMapViewPropertyChanged));

        private static void OnMapViewPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var OverviewMap = obj as OverviewMap;
            var oldMapView = args.OldValue as MapView;
            var newMapView = args.NewValue as MapView;

            if (oldMapView != null)
            {
                oldMapView.PropertyChanged -= OverviewMap.OnMapViewPropertyChanged;
                oldMapView.ExtentChanged -= OverviewMap.OnExtentChanged;
            }

            if (newMapView != null)
            {
                newMapView.PropertyChanged += OverviewMap.OnMapViewPropertyChanged;
                newMapView.ExtentChanged += OverviewMap.OnExtentChanged; 
            }
        }

        private void OnExtentChanged(object sender, System.EventArgs e)
        {
            var controllingMapView = sender as MapView;
            _overviewMapView.SetViewAsync(controllingMapView.Extent.Expand(1.5));
            _overviewMapView.GraphicsOverlays.First().Graphics.First().Geometry = controllingMapView.Extent;

        }

        private void OnMapViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Rotation")
            {
                var rotation = MapView.Rotation;
                _overviewMapView.SetRotationAsync(rotation);
            }
        }

        public MapView MapView
        {
            get { return GetValue(MapViewProperty) as MapView; }
            set { SetValue(MapViewProperty, value); }
        }

        #endregion // MapView

        #region Private methods

        #endregion  
    }
}
