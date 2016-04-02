using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace BuildingControlsForArcGISRuntime
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MyMapView_LayerLoaded(object sender, LayerLoadedEventArgs e)
        {
            if (e.LoadError != null)
            {
                Debug.WriteLine(string.Format("Error while loading layer : {0} - {1}", e.Layer.ID, e.LoadError.Message));
                throw new Exception("Layer failed to load", e.LoadError);
            }

            if (e.Layer is FeatureLayer)
            {
                var featureLayer = e.Layer as FeatureLayer;
                var results = await featureLayer.FeatureTable.QueryAsync(new Esri.ArcGISRuntime.Data.QueryFilter()
                {
                    WhereClause = "1=1"
                });
                foreach (var result in results)
                {
                    Points.Items.Add(result);
                }
            }
        }

        private void rotationSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            MyMapView.SetRotation(e.NewValue);
        }

        private void Points_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count ==1)
            {
                var selectedFeature = Points.SelectedItem as Feature;
                var featureLayer = MyMapView.Map.Layers.OfType<FeatureLayer>().First();

                // Remove previous highlighting
                if (featureLayer.SelectedFeatureIDs.Count() > 0)
                    featureLayer.UnselectFeatures(featureLayer.SelectedFeatureIDs.ToArray());

                // Highlight the feature
                featureLayer.SelectFeatures(new[] {
                    ((long)selectedFeature.Attributes[featureLayer.FeatureTable.ObjectIDField])});

                // Enable animation highlight
                highlightOverlay.HighlightCommand.Execute(
                    selectedFeature.Geometry as MapPoint);
            }

        }
    }
}
