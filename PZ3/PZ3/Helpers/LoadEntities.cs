using PZ3.Models;
using System;
using System.Collections.Generic;
using System.Xml;

namespace PZ3.Helpers
{
    public static class LoadEntities
    {
        public static void GetSubstations(XmlDocument xmlDocument,double boundaryMaxX, double boundaryMinX, double boundaryMaxY,
            double boundaryMinY, List<Point> points, List<SubstationEntity> substationEntities)
        {
           var nodeList = xmlDocument.DocumentElement.SelectNodes("/NetworkModel/Substations/SubstationEntity");
            foreach (XmlNode node in nodeList)
            {
                SubstationEntity sub = new SubstationEntity();

                sub.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                sub.Name = node.SelectSingleNode("Name").InnerText;
                sub.X = double.Parse(node.SelectSingleNode("X").InnerText);
                sub.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                Converter.ToLatLon(sub.X, sub.Y, 34, out double newX, out double newY);
                if (newX < boundaryMaxX && newX > boundaryMinX && newY < boundaryMaxY && newY > boundaryMinY)
                {
                    points.Add(new Models.Point(newX, newY, sub.Id, sub.Name, ""));
                    substationEntities.Add(sub);
                }

            }
        }

        public static void GetNodes (XmlDocument xmlDocument, double boundaryMaxX, double boundaryMinX, double boundaryMaxY, double boundaryMinY, 
            List<Point> points, List<NodeEntity> nodeEntities)
        {
            var nodeList = xmlDocument.DocumentElement.SelectNodes("/NetworkModel/Nodes/NodeEntity");

            foreach (XmlNode node in nodeList)
            {
                NodeEntity nodeEntity = new NodeEntity();

                nodeEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                nodeEntity.Name = node.SelectSingleNode("Name").InnerText;
                nodeEntity.X = double.Parse(node.SelectSingleNode("X").InnerText);
                nodeEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);

                Converter.ToLatLon(nodeEntity.X, nodeEntity.Y, 34, out double newX, out double newY);
                if (newX < boundaryMaxX && newX > boundaryMinX && newY < boundaryMaxY && newY > boundaryMinY)
                {
                    points.Add(new Models.Point(newX, newY, nodeEntity.Id, nodeEntity.Name, ""));
                    nodeEntities.Add(nodeEntity);
                }
            }
        }

        public static void GetSwitches(XmlDocument xmlDocument, double boundaryMaxX, double boundaryMinX, double boundaryMaxY, double boundaryMinY,
            List<Point> points, List<SwitchEntity> switchEntities)
        {
            var nodeList = xmlDocument.DocumentElement.SelectNodes("/NetworkModel/Switches/SwitchEntity");

            foreach (XmlNode node in nodeList)
            {
                SwitchEntity switchEntity = new SwitchEntity();

                switchEntity.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                switchEntity.Name = node.SelectSingleNode("Name").InnerText;
                switchEntity.X = double.Parse(node.SelectSingleNode("X").InnerText);
                switchEntity.Y = double.Parse(node.SelectSingleNode("Y").InnerText);
                switchEntity.State = node.SelectSingleNode("Status").InnerText;

                Converter.ToLatLon(switchEntity.X, switchEntity.Y, 34, out double newX, out double newY);
                if (newX < boundaryMaxX && newX > boundaryMinX && newY < boundaryMaxY && newY > boundaryMinY)
                {
                    points.Add(new Models.Point(newX, newY, switchEntity.Id, switchEntity.Name, ""));
                    switchEntities.Add(switchEntity);
                }
            }
        }

        public static void GetLines(XmlDocument xmlDocument, double boundaryMaxX, double boundaryMinX, double boundaryMaxY, double boundaryMinY,
            List<Point> points, List<LineEntity> lineEntities, Dictionary<Tuple<long, string>, List<System.Windows.Point>> lines)
        {
            var nodeList = xmlDocument.DocumentElement.SelectNodes("/NetworkModel/Lines/LineEntity");

            foreach (XmlNode node in nodeList)
            {
                LineEntity l = new LineEntity();

                List<System.Windows.Point> linePoints = new List<System.Windows.Point>();
                l.Id = long.Parse(node.SelectSingleNode("Id").InnerText);
                l.Name = node.SelectSingleNode("Name").InnerText;
                l.FirstEnd = long.Parse(node.SelectSingleNode("FirstEnd").InnerText);
                l.SecondEnd = long.Parse(node.SelectSingleNode("SecondEnd").InnerText);
                l.ConductorMaterial = node.SelectSingleNode("ConductorMaterial").InnerText;
                foreach (XmlNode item in node.ChildNodes[9].ChildNodes)
                {
                    System.Windows.Point point = new System.Windows.Point();
                    point.X = double.Parse(item.SelectSingleNode("X").InnerText);
                    point.Y = double.Parse(item.SelectSingleNode("Y").InnerText);
                    Converter.ToLatLon(point.X, point.Y, 34, out double x, out double y);
                    if (x < boundaryMaxX && x > boundaryMinX && y < boundaryMaxY && y > boundaryMinY)
                    {
                        linePoints.Add(new System.Windows.Point(x, y));
                    }
                }
                l.Vertices = linePoints;
                if (points.Exists(p => p.Id == l.FirstEnd) && points.Exists(p => p.Id == l.SecondEnd))
                {
                    if (!lineEntities.Exists(line => line.FirstEnd == l.FirstEnd && line.SecondEnd == l.SecondEnd))
                    {
                        lineEntities.Add(l);
                        lines.Add(new Tuple<long, string>(l.Id, l.ConductorMaterial), linePoints);
                    }
                }

            }
        }
    }
}
