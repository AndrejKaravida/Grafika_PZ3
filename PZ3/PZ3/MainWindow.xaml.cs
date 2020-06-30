using PZ3.Helpers;
using PZ3.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml;
using Point = System.Windows.Point;

namespace PZ3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point mapPosition = new Point();
        private Point start = new Point();
        private Point diffOffset = new Point();
        private int zoomMax = 20;
        private int zoomCurent = 1;
        public double boundaryMinX = 19.793909;
        public double boundaryMaxX = 19.894459;
        public double boundaryMinY = 45.2325;
        public double boundaryMaxY = 45.277031;
        public double xInterval, yInterval;
        public double stepX, stepY;
        private Model3D selectedCube1 = null;
        private Model3D selectedCube2 = null;
        List<Models.Point> points = new List<Models.Point>();
        Dictionary<Tuple<double, double>, double> occupied = new Dictionary<Tuple<double, double>, double>();
        Dictionary<Tuple<long, string>, List<Point>> lines = new Dictionary<Tuple<long, string>, List<Point>>();
        List<LineEntity> lineEntities = new List<LineEntity>();
        public List<SubstationEntity> substationEntities = new List<SubstationEntity>();
        public List<NodeEntity> nodeEntities = new List<NodeEntity>();
        public List<SwitchEntity> switchEntities = new List<SwitchEntity>();
        private Dictionary<Model3D, Tuple<string, string>> Models = new Dictionary<Model3D, Tuple<string, string>>();
        private Dictionary<Model3D, Tuple<long, long>> LineModels = new Dictionary<Model3D, Tuple<long, long>>();
        System.Windows.Controls.ToolTip _toolTip;
        public object ToolTipContent { get { return _toolTip.Content; } set { _toolTip.Content = value; } }

        public MainWindow()
        {
            InitializeComponent();
            _toolTip = new System.Windows.Controls.ToolTip();

            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.Load("Geographic.xml");

            LoadEntities.GetSubstations(xmlDocument, boundaryMaxX, boundaryMinX, boundaryMaxY, boundaryMinY, points, substationEntities);
            LoadEntities.GetNodes(xmlDocument, boundaryMaxX, boundaryMinX, boundaryMaxY, boundaryMinY, points, nodeEntities);
            LoadEntities.GetSwitches(xmlDocument, boundaryMaxX, boundaryMinX, boundaryMaxY, boundaryMinY, points, switchEntities);

            DrawEntities.DrawPoints(boundaryMaxX, boundaryMinX, boundaryMaxY, boundaryMinY, points, occupied, substationEntities, switchEntities, nodeEntities, Models);

            LoadEntities.GetLines(xmlDocument, boundaryMaxX, boundaryMinX, boundaryMaxY, boundaryMinY, points, lineEntities, lines);

            DrawEntities.DrawLines(boundaryMaxX, boundaryMinX, boundaryMaxY, boundaryMinY, lines, lineEntities, LineModels);
        }

        private void viewport1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (_toolTip != null)
                _toolTip.IsOpen = false;
            viewport1.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = translacija.OffsetX;
            diffOffset.Y = translacija.OffsetZ;

            // Perform the hit test.
            HitTestResult result =
                VisualTreeHelper.HitTest(viewport1, start);

            // Display information about the hit.
            RayMeshGeometry3DHitTestResult mesh_result = result as RayMeshGeometry3DHitTestResult;
            if (mesh_result != null && result != null)
            {
                try
                {
                    // Perform action on hit visual object.
                    //this.Title = Models[mesh_result.ModelHit];
                    _toolTip.Content = Models[mesh_result.ModelHit];
                    if (_toolTip != null)
                        _toolTip.Dispatcher.Invoke(new Action(() => { _toolTip.IsOpen = true; }));
                }
                catch (Exception)
                {
                }
                try
                {
                    if (selectedCube1 != null && selectedCube2 != null)
                    {
                        GeometryModel3D cube1OldColor = selectedCube1 as GeometryModel3D;
                        var diffuseMaterialBlue = new DiffuseMaterial(new SolidColorBrush(Colors.IndianRed));
                        var surface_material5s = new DiffuseMaterial(Brushes.IndianRed);
                        cube1OldColor.Material = diffuseMaterialBlue;
                        cube1OldColor.BackMaterial = surface_material5s;

                        GeometryModel3D cube2OldColor = selectedCube2 as GeometryModel3D;
                        cube2OldColor.Material = diffuseMaterialBlue;
                        cube2OldColor.BackMaterial = surface_material5s;
                    }
                    selectedCube1 = null;
                    selectedCube2 = null;

                    var ids = LineModels[mesh_result.ModelHit];
                    foreach (var cube in Models)
                    {
                        if (cube.Value.Item2 == ids.Item1.ToString() || cube.Value.Item2 == ids.Item2.ToString())
                        {
                            if (selectedCube1 == null)
                            {
                                selectedCube1 = cube.Key;
                            }
                            else
                            {
                                selectedCube2 = cube.Key;
                                break;
                            }
                        }
                    }

                    GeometryModel3D casted3DModelCube1 = selectedCube1 as GeometryModel3D;
                    var diffuseMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Yellow));
                    var surface_material5 = new DiffuseMaterial(Brushes.Yellow);
                    casted3DModelCube1.Material = diffuseMaterial;
                    casted3DModelCube1.BackMaterial = surface_material5;

                    GeometryModel3D casted3DModelCube2 = selectedCube2 as GeometryModel3D;
                    casted3DModelCube2.Material = diffuseMaterial;
                    casted3DModelCube2.BackMaterial = surface_material5;
                }
                catch (Exception)
                {
                }
            }
        }

        private void viewport1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewport1.ReleaseMouseCapture();
        }

        private void viewport1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (viewport1.IsMouseCaptured)
            {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    start = e.GetPosition(this);
                    if (start.X < mapPosition.X)
                    {
                        rotate.Angle -= 1;
                    }
                    else if (start.X > mapPosition.X)
                    {
                        rotate.Angle += 1;
                    }
                    if (start.Y > mapPosition.Y && rotateX.Angle < 45)
                    {
                        rotateX.Angle += 1;
                    }
                    else if (rotateX.Angle > -10)
                    {
                        rotateX.Angle -= 1;
                    }
                    mapPosition = start;
                    rotateTransform.CenterX = 5;
                    rotateTransform.CenterZ = 5;
                    rotateTransformX.CenterX = 5;
                    rotateTransformX.CenterZ = 5;
                }
                else
                {
                    Point end = e.GetPosition(this);
                    double offsetX = end.X - start.X;
                    double offsetY = end.Y - start.Y;
                    double w = this.Width;
                    double h = this.Height;
                    double translateX = (offsetX * 1000) / w;
                    double translateY = (offsetY * 1000) / h;
                    translacija.OffsetX = diffOffset.X + (translateX / (100 * skaliranje.ScaleX));
                    translacija.OffsetZ = diffOffset.Y + (translateY / (100 * skaliranje.ScaleZ));
                }

            }
        }

        private void viewport1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p = e.MouseDevice.GetPosition(this);
            double scaleX = 1;
            double scaleY = 1;
            double scaleZ = 1;
            if (e.Delta > 0 && zoomCurent < zoomMax)
            {
                scaleX = skaliranje.ScaleX + 0.1;
                scaleY = skaliranje.ScaleY + 0.1;
                scaleZ = skaliranje.ScaleZ + 0.1;
                zoomCurent++;
                skaliranje.ScaleX = scaleX;
                skaliranje.ScaleY = scaleY;
                skaliranje.ScaleZ = scaleZ;
            }
            else if (e.Delta <= 0 && zoomCurent > -zoomMax)
            {
                scaleX = skaliranje.ScaleX - 0.1;
                scaleY = skaliranje.ScaleY - 0.1;
                scaleZ = skaliranje.ScaleZ - 0.1;
                zoomCurent--;
                skaliranje.ScaleX = scaleX;
                skaliranje.ScaleY = scaleY;
                skaliranje.ScaleZ = scaleZ;
            }
        }

        private void Viewport1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                viewport1.CaptureMouse();
            }
        }

        private void Viewport1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            viewport1.ReleaseMouseCapture();
        }

      
    }
}
