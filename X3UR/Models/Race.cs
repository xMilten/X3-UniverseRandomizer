using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using X3UR.Enums;

namespace X3UR.Models {
    public class Race {
        /// <summary>
        /// Die ID der Rasse
        /// </summary>
        public RaceNames Id { get; private set; }
        /// <summary>
        /// Der Name der Rasse
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Die aktuelle Größe der Rasse (aktuelle anzahl der Sektoren)
        /// </summary>
        public short CurrentSize { get; set; }
        /// <summary>
        /// Die maximale Größe der Rasse (maximale anzahl der Sektoren)
        /// </summary>
        public short MaxSize { get; private set; }
        /// <summary>
        /// Die aktuelle anzahl an Clustern die die Rasse hat
        /// </summary>
        public short CurrentClusterCount { get; set; }
        /// <summary>
        /// Die maximale anzahl an Clustern, die die Rasse haben kann
        /// </summary>
        public short MaxClusterCount { get; private set; }
        /// <summary>
        /// Die maximale Größe, die ein Cluster haben kann (maximale anzahl der Sektoren)
        /// </summary>
        public short MaxClusterSize { get; private set; }
        /// <summary>
        /// Die Farbe der Rasse (Nur für das UI wichtig)
        /// </summary>
        public Color Color { get; set; } = Color.FromRgb(255, 255, 255);

        public Race(RaceNames raceId, string name, short maxSize, short maxClustersCount, short maxClusterSize, Color color) {
            Id = raceId;
            Name = name;
            CurrentSize = 0;
            MaxSize = maxSize;
            CurrentClusterCount = 0;
            MaxClusterCount = maxClustersCount;
            MaxClusterSize = maxClusterSize;
            Color = color;
        }
    }
}