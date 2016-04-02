using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using System;
using System.Threading.Tasks;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using System.Windows.Input;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endif

namespace BCFAR.Controls
{
    [TemplatePart(Name = "RootGrid", Type = typeof(Grid))]
    public class HighlightOverlay : Control
    {
        private Grid _rootGrid;

        public HighlightOverlay()
        {
// For Store and Phone, map the default style
#if NETFX_CORE
            DefaultStyleKey = typeof(HighlightOverlay);
#endif
        }

// For desktop, map the default style
#if !NETFX_CORE
        static HighlightOverlay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HighlightOverlay), new FrameworkPropertyMetadata(typeof(HighlightOverlay)));
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

            _rootGrid = GetTemplateChild("RootGrid") as Grid;
        }

        #region MapView
        public static readonly DependencyProperty MapViewProperty =
            DependencyProperty.Register("MapView", typeof(MapView), typeof(HighlightOverlay), new PropertyMetadata(null, OnMapViewPropertyChanged));

        private static void OnMapViewPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var highlightOverlay = obj as HighlightOverlay;
            var oldMapView = args.OldValue as MapView;
            var newMapView = args.NewValue as MapView;

            if (oldMapView != null)
            {
                oldMapView.ExtentChanged -= highlightOverlay.OnExtentChanged;
            }

            if (newMapView != null)
            {
                newMapView.ExtentChanged += highlightOverlay.OnExtentChanged;
            }
        }

        private void OnExtentChanged(object sender, EventArgs e)
        {
            _rootGrid.Children.Clear();
        }

        public MapView MapView
        {
            get { return GetValue(MapViewProperty) as MapView; }
            set { SetValue(MapViewProperty, value); }
        }

        #endregion // MapView

        #region LineLength
        public static readonly DependencyProperty LineLengthProperty =
            DependencyProperty.Register("LineLength", typeof(double), typeof(HighlightOverlay), new PropertyMetadata(100d));

        public double LineLength
        {
            get { return (double)GetValue(LineLengthProperty); }
            set { SetValue(LineLengthProperty, value); }
        }
        #endregion // LineLength

        #region LineWidth
        public static readonly DependencyProperty LineWidthProperty =
            DependencyProperty.Register("LineWidth", typeof(double), typeof(HighlightOverlay), new PropertyMetadata(3d));

        public double LineWidth
        {
            get { return (double)GetValue(LineWidthProperty); }
            set { SetValue(LineWidthProperty, value); }
        }
        #endregion // LineWidth

        #region TopToCenterLineFill
        public static readonly DependencyProperty TopToCenterLineFillProperty =
            DependencyProperty.Register("TopToCenterLineFill", typeof(Brush), typeof(HighlightOverlay), 
                new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush TopToCenterLineFill
        {
            get { return (Brush)GetValue(TopToCenterLineFillProperty); }
            set { SetValue(TopToCenterLineFillProperty, value); }
        }
        #endregion // TopToCenterLineFill

        #region BottomToCenterLineFill
        public static readonly DependencyProperty BottomToCenterLineFillProperty =
            DependencyProperty.Register("BottomToCenterLineFill", typeof(Brush), typeof(HighlightOverlay),
                new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush BottomToCenterLineFill
        {
            get { return (Brush)GetValue(BottomToCenterLineFillProperty); }
            set { SetValue(BottomToCenterLineFillProperty, value); }
        }
        #endregion // BottomToCenterLineFill

        #region LeftToCenterLineFill
        public static readonly DependencyProperty LeftToCenterLineFillProperty =
            DependencyProperty.Register("LeftToCenterLineFill", typeof(Brush), typeof(HighlightOverlay),
                new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush LeftToCenterLineFill
        {
            get { return (Brush)GetValue(LeftToCenterLineFillProperty); }
            set { SetValue(LeftToCenterLineFillProperty, value); }
        }
        #endregion // LeftToCenterLineFill

        #region RightToCenterLineFill
        public static readonly DependencyProperty RightToCenterLineFillProperty =
            DependencyProperty.Register("RightToCenterLineFill", typeof(Brush), typeof(HighlightOverlay),
                new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        public Brush RightToCenterLineFill
        {
            get { return (Brush)GetValue(RightToCenterLineFillProperty); }
            set { SetValue(RightToCenterLineFillProperty, value); }
        }
        #endregion //RightToCenterLineFill


        private DelegateCommand<MapPoint> _highlightCommand;
        public ICommand HighlightCommand
        {
            get
            {
                if (_highlightCommand == null)
                {
                    _highlightCommand = new DelegateCommand<MapPoint>(
                        async (mapPoint) => {
                            //if (mapPoint == null)
                            //    throw new ArgumentNullException("mapPoint");

                            // Check if the mapPoint is in the view and don't do anything if not
                            if (!GeometryEngine.Contains(GeometryEngine.NormalizeCentralMeridian(MapView.Extent) ,mapPoint))
                                return;

                            var point = MapView.LocationToScreen(mapPoint);

                            if (MapView.WrapAround)
                            {
                                // Handle multiple pans to the east
                                if (point.X < 0)
                                {
                                    while (point.X < 0)
                                    {
                                        point.X = MapView.ActualWidth - (-point.X);
                                    }
                                }
                                // Handle multiple pans to the west
                                if (MapView.ActualWidth < point.X)
                                {
                                    while (MapView.ActualWidth < point.X)
                                    {
                                        point.X = point.X - MapView.ActualWidth;
                                    }
                                }
                                if (point.Y < 0)
                                {
                                    while (point.Y < 0)
                                    {
                                        point.Y = MapView.ActualHeight - (-point.Y);
                                    }
                                }
                                if (MapView.ActualHeight < point.Y)
                                {
                                    while (MapView.ActualHeight < point.Y)
                                    {
                                        point.Y =  point.Y - MapView.ActualHeight;
                                    }
                                }
                            }
                            // TODO rotation support

                            var topToCenterRectangle = CreateTopToCenterRectangle(point, MapView);
                            var bottomToCenterRectangle = CreateBottomToCenterRectangle(point, MapView);
                            var leftToCenterRectangle = CreateLeftToCenterRectangle(point, MapView);
                            var rightToCenterRectangle = CreateRightToCenterRectangle(point, MapView);

                            _rootGrid.Children.Add(topToCenterRectangle);
                            _rootGrid.Children.Add(bottomToCenterRectangle);
                            _rootGrid.Children.Add(leftToCenterRectangle);
                            _rootGrid.Children.Add(rightToCenterRectangle);
                            
                            // Top to bottom
                            ThicknessAnimation TopToCenterAnimation = new ThicknessAnimation(
                                new Thickness(point.X, point.Y - topToCenterRectangle.Height - 12 , 0, 0), 
                                TimeSpan.FromSeconds(0.2)
                                );
                            topToCenterRectangle.BeginAnimation(Rectangle.MarginProperty, TopToCenterAnimation);

                            // Bottom to top
                            ThicknessAnimation bottomToCenterAnimation = new ThicknessAnimation(
                                new Thickness(point.X, point.Y + 12, 0,0),
                                TimeSpan.FromSeconds(0.2)
                                );
                            bottomToCenterRectangle.BeginAnimation(Rectangle.MarginProperty, bottomToCenterAnimation);

                            // Right to center
                            ThicknessAnimation rightToCenterAnimation = new ThicknessAnimation(
                                new Thickness(point.X + 12, point.Y, 0, 0),
                                TimeSpan.FromSeconds(0.2)
                                );
                            rightToCenterRectangle.BeginAnimation(Rectangle.MarginProperty, rightToCenterAnimation);

                            // Left to center
                            ThicknessAnimation leftToCenterAnimation = new ThicknessAnimation(
                                new Thickness(point.X - leftToCenterRectangle.Width - 12, point.Y, 0, 0),
                                TimeSpan.FromSeconds(0.2)
                                );
                            leftToCenterRectangle.BeginAnimation(Rectangle.MarginProperty, leftToCenterAnimation);

                            await Task.Delay(TimeSpan.FromSeconds(0.25));

                            _rootGrid.Children.Remove(topToCenterRectangle);
                            _rootGrid.Children.Remove(bottomToCenterRectangle);
                            _rootGrid.Children.Remove(leftToCenterRectangle);
                            _rootGrid.Children.Remove(rightToCenterRectangle);
                        },
                        (mapPoint) => {
                            return true;
                        });
                }
                return _highlightCommand;
            }
        }

        #region Private methods

        private Rectangle CreateTopToCenterRectangle(Point targetPoint, MapView mapView)
        {
            var rectangle = new Rectangle();
            rectangle.Width = LineWidth; // Vertical so with is width
            rectangle.Height = LineLength; // Vertical so length is height
            rectangle.Margin = new Thickness(targetPoint.X, 0, 0, mapView.ActualHeight);
            rectangle.Fill = TopToCenterLineFill;
            rectangle.HorizontalAlignment = HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;

            return rectangle;
        }

        private Rectangle CreateBottomToCenterRectangle(Point targetPoint, MapView mapView)
        {
            var rectangle = new Rectangle();
            rectangle.Width = LineWidth; // Vertical so with is width
            rectangle.Height = LineLength; // Vertical so length is height
            rectangle.Margin = new Thickness(targetPoint.X, mapView.ActualHeight, 0,0);
            rectangle.Fill = BottomToCenterLineFill;
            rectangle.HorizontalAlignment = HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;

            return rectangle;
        }

        private Rectangle CreateLeftToCenterRectangle(Point targetPoint, MapView mapView)
        {
            var rectangle = new Rectangle();
            rectangle.Width = LineLength; 
            rectangle.Height = LineWidth; 
            rectangle.Margin = new Thickness(-LineLength, targetPoint.Y, 0, 0);
            rectangle.Fill = LeftToCenterLineFill;
            rectangle.HorizontalAlignment = HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;

            return rectangle;
        }

        private Rectangle CreateRightToCenterRectangle(Point targetPoint, MapView mapView)
        {
            var rectangle = new Rectangle();
            rectangle.Width = LineLength;
            rectangle.Height = LineWidth; rectangle.Margin = new Thickness(mapView.ActualWidth, targetPoint.Y, 0, 0);
            rectangle.Fill = RightToCenterLineFill;
            rectangle.HorizontalAlignment = HorizontalAlignment.Left;
            rectangle.VerticalAlignment = VerticalAlignment.Top;

            return rectangle;
        }
        #endregion  
    }
}
