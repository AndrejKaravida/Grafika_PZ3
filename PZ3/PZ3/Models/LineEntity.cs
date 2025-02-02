﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZ3.Models
{
    public class LineEntity
    {
        private long id;
        private string name;
        private bool isUnderground;
        private float r;
        private string conductorMaterial;
        private string lineType;
        private long thermalConstantHeat;
        private long firstEnd;
        private long secondEnd;
        private List<System.Windows.Point> vertices;
        public LineEntity()
        {

        }

        public List<System.Windows.Point> Vertices { get => vertices; set => vertices = value; }
        public long Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public bool IsUnderground { get => isUnderground; set => isUnderground = value; }
        public float R { get => r; set => r = value; }
        public string ConductorMaterial { get => conductorMaterial; set => conductorMaterial = value; }
        public string LineType { get => lineType; set => lineType = value; }
        public long ThermalConstantHeat { get => thermalConstantHeat; set => thermalConstantHeat = value; }
        public long FirstEnd { get => firstEnd; set => firstEnd = value; }
        public long SecondEnd { get => secondEnd; set => secondEnd = value; }
    }
}
