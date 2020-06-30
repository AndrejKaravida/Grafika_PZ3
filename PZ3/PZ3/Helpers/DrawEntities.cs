using PZ3.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PZ3.Helpers
{
    public static class DrawEntities
    {
        public static void DrawPoints (double boundaryMaxX, double boundaryMinX, double boundaryMaxY, double boundaryMinY,
            List<Models.Point> points, Dictionary<Tuple<double, double>, double> occupied, List<SubstationEntity> substationEntities,
            List<SwitchEntity> switchEntities, List<NodeEntity> nodeEntities, Dictionary<Model3D, Tuple<string, string>> Models)
        {
            var xInterval = boundaryMaxX - boundaryMinX;
            var yInterval = boundaryMaxY - boundaryMinY;
            var stepX = xInterval / 100;
            var stepY = yInterval / 100;
            double x, z;

            foreach (Models.Point point in points)
            {
                double y = 0;
                x = (point.X - boundaryMinX) / stepX;
                z = (point.Y - boundaryMinY) / stepY;
                x = x / 10;
                z = z / 10;
                var divisionX = x / 0.05;
                var castX = (int)divisionX;
                var multiplicationX = castX * 0.05;

                var divisionZ = z / 0.05;
                var castZ = (int)divisionZ;
                var multiplicationZ = 10 - (castZ * 0.05);
                if (occupied.ContainsKey(new Tuple<double, double>(multiplicationX, multiplicationZ)))
                {
                    y = occupied[new Tuple<double, double>(multiplicationX, multiplicationZ)] + 0.05;
                    occupied[new Tuple<double, double>(multiplicationX, multiplicationZ)] = y;
                }
                else
                {
                    occupied.Add(new Tuple<double, double>(multiplicationX, multiplicationZ), y);
                }
                string tooltip = "";
                if (substationEntities.Exists(sub => sub.Id == point.Id))
                {
                    int index = substationEntities.FindIndex(sub => sub.Id == point.Id);
                    tooltip = substationEntities[index].Name + " - " + " SUBSTATION " + substationEntities[index].Id;
                }
                else if (switchEntities.Exists(s => s.Id == point.Id))
                {
                    int index = switchEntities.FindIndex(sub => sub.Id == point.Id);
                    tooltip = switchEntities[index].Name + " - " + " SWITCH " + switchEntities[index].Id;
                }
                else if (nodeEntities.Exists(n => n.Id == point.Id))
                {
                    int index = nodeEntities.FindIndex(sub => sub.Id == point.Id);
                    tooltip = nodeEntities[index].Name + " - " + " NODE " + nodeEntities[index].Id;
                }
                MainWindow main = ((MainWindow)Application.Current.MainWindow);
                DefineModel(main.modelGroup, multiplicationX, y, multiplicationZ, tooltip, point.Id.ToString(), Models);
            }
        }

        private static void DefineModel(Model3DGroup model_group, double x, double y, double z, string tooltipContent, string id,
            Dictionary<Model3D, Tuple<string, string>> Models)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            if (z == 10)
            {
                z = 9.9;
            }
            if (x == 10)
            {
                x = 9.9;
            }
            AddPoint(mesh, x, y, z);

            DiffuseMaterial surface_material = new DiffuseMaterial(Brushes.CadetBlue);

            GeometryModel3D surface_model = new GeometryModel3D(mesh, surface_material);

            Models.Add(surface_model, new Tuple<string, string>(tooltipContent, id));

            surface_model.BackMaterial = surface_material;

            model_group.Children.Add(surface_model);
        }

        private static void AddPoint (MeshGeometry3D mesh, double x1, double y1, double z1)
        {
            double step = 0.05;

            Point3D p1 = new Point3D(x1, y1, z1);
            Point3D p2 = new Point3D(x1 + step, y1, z1);
            Point3D p3 = new Point3D(x1, y1, z1 + step);
            Point3D p4 = new Point3D(x1 + step, y1, z1 + step);

            Point3D p5 = new Point3D(x1, y1 + step, z1);
            Point3D p6 = new Point3D(x1 + step, y1 + step, z1);
            Point3D p7 = new Point3D(x1, y1 + step, z1 + step);
            Point3D p8 = new Point3D(x1 + step, y1 + step, z1 + step);

            AddTriangle(mesh, p3, p4, p1);
            AddTriangle(mesh, p4, p2, p1);
            AddTriangle(mesh, p3, p4, p7);
            AddTriangle(mesh, p4, p8, p7);
            AddTriangle(mesh, p4, p2, p8);
            AddTriangle(mesh, p2, p6, p8);
            AddTriangle(mesh, p1, p7, p5);
            AddTriangle(mesh, p1, p3, p7);
            AddTriangle(mesh, p1, p5, p6);
            AddTriangle(mesh, p1, p6, p2);
            AddTriangle(mesh, p7, p8, p5);
            AddTriangle(mesh, p8, p6, p5);
        }

        private static void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            int index1 = AddPoint(mesh.Positions, point1);
            int index2 = AddPoint(mesh.Positions, point2);
            int index3 = AddPoint(mesh.Positions, point3);

            mesh.TriangleIndices.Add(index1);
            mesh.TriangleIndices.Add(index2);
            mesh.TriangleIndices.Add(index3);
        }

        private static int AddPoint(Point3DCollection points, Point3D point)
        {
            for (int i = points.Count - 1; i >= 0; i--)
            {
                if ((point.X == points[i].X) &&
                    (point.Y == points[i].Y) &&
                    (point.Z == points[i].Z))
                    return i;
            }

            points.Add(point);
            return points.Count - 1;
        }

        public static void DrawLines(double boundaryMaxX, double boundaryMinX, double boundaryMaxY, 
            double boundaryMinY, Dictionary<Tuple<long, string>, List<System.Windows.Point>> lines, List<LineEntity> lineEntities, Dictionary<Model3D, Tuple<long, long>> LineModels)
        {
           var xInterval = boundaryMaxX - boundaryMinX;
           var yInterval = boundaryMaxY - boundaryMinY;
           var stepX = xInterval / 100;
           var stepY = yInterval / 100;

            double x1, z1, x2, z2;
            foreach (var item in lines.Keys)
            {
                for (int i = 0; i < lines[item].Count - 1; i++)
                {
                    System.Windows.Point current = lines[item][i];
                    System.Windows.Point next = lines[item][i + 1];
                    x1 = (current.X - boundaryMinX) / stepX;
                    z1 = (current.Y - boundaryMinY) / stepY;
                    x1 = x1 / 10;
                    z1 = z1 / 10;
                    x2 = (next.X - boundaryMinX) / stepX;
                    z2 = (next.Y - boundaryMinY) / stepY;
                    x2 = x2 / 10;
                    z2 = z2 / 10;
                    MainWindow main = ((MainWindow)Application.Current.MainWindow);
                    DefineModelLines(main.modelGroup, x1, 10 - z1, x2, 10 - z2, item.Item2, item.Item1, lineEntities, LineModels);
                }
            }
        }

        private static void DefineModelLines(Model3DGroup model_group, double x1, double z1, double x2, double z2, string conductorMaterial, long id
            , List<LineEntity> lineEntities, Dictionary<Model3D, Tuple<long, long>> LineModels)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            AddLine(mesh, x1, z1, x2, z2);
            DiffuseMaterial surface_material;
            if (conductorMaterial == "Steel")
                surface_material = new DiffuseMaterial(Brushes.DeepPink);
            else if (conductorMaterial == "Copper")
                surface_material = new DiffuseMaterial(Brushes.DarkRed);
            else if (conductorMaterial == "Acsr")
                surface_material = new DiffuseMaterial(Brushes.MediumAquamarine);
            else
                surface_material = new DiffuseMaterial(Brushes.Orange);

            GeometryModel3D surface_model =
                new GeometryModel3D(mesh, surface_material);


            surface_model.BackMaterial = surface_material;
            var index = lineEntities.FindIndex(sub => sub.Id == id);
            LineModels.Add(surface_model, new Tuple<long, long>(lineEntities[index].FirstEnd, lineEntities[index].SecondEnd));

            model_group.Children.Add(surface_model);
        }

        private static void AddLine(MeshGeometry3D mesh, double x1, double z1, double x2, double z2)
        {
            var step = 0.02;
            Point3D p1 = new Point3D(x1, 0, z1);
            Point3D p2 = new Point3D(x2, 0, z2);
            Point3D p3 = new Point3D(x2, 0, z2 + step);
            Point3D p4 = new Point3D(x1, 0, z1 + step);

            Point3D p5 = new Point3D(x1, step, z1);
            Point3D p6 = new Point3D(x2, step, z2);
            Point3D p7 = new Point3D(x2, step, z2 + step);
            Point3D p8 = new Point3D(x1, step, z1 + step);

            AddTriangle(mesh, p3, p4, p1);
            AddTriangle(mesh, p4, p2, p1);
            AddTriangle(mesh, p3, p4, p7);
            AddTriangle(mesh, p4, p8, p7);
            AddTriangle(mesh, p4, p2, p8);
            AddTriangle(mesh, p2, p6, p8);
            AddTriangle(mesh, p1, p7, p5);
            AddTriangle(mesh, p1, p3, p7);
            AddTriangle(mesh, p1, p5, p6);
            AddTriangle(mesh, p1, p6, p2);
            AddTriangle(mesh, p7, p8, p5);
            AddTriangle(mesh, p8, p6, p5);
        }

    }
}
