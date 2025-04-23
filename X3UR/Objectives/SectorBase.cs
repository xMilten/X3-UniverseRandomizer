using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using X3UR.Generator;

namespace X3UR.Objectives {
    /// <summary>
    /// Ein leere Platz im Universum, der von einem Sector besetzten kann.
    /// </summary>
    public class SectorBase {
        public byte PosX { get; protected set; }
        public byte PosY { get; protected set; }
        public string Coords { get; set; }
        public Visibility Visibility { get; set; }
        public List<Sector> SectorsCanClaimMe { get; set; }
        public Color Color { get; set; }

        public SectorBase(byte posX, byte posY) {
            PosX = posX;
            PosY = posY;
            Coords = $"{posX} : {posY}";
            Visibility = Visibility.Hidden;

            SectorsCanClaimMe = new List<Sector>();
        }

        public Sector ConvertToSector() {
            RemoveSectorBases();
            GrowingUniverseGenerator.Universe.Map[PosY, PosX] = new Sector(PosX, PosY);
            return (Sector)GrowingUniverseGenerator.Universe.Map[PosY, PosX];
        }

        /// <summary>
        /// Entfernt diesen SectorBase aus den Listen "SectorBases" aller Sectoren, die diesen hätten übernehmen können.
        /// Anschließend wird überprüft, ob die Listen der Sectors noch Einträge haben und wenn nicht,
        /// wird der entsprechende Sector aus der Liste "GrowableSectors" seines Clusters entfernt.
        /// </summary>
        public void RemoveSectorBases() {
            if (SectorsCanClaimMe.Count > 0) {
                foreach (Sector sector in SectorsCanClaimMe) {
                    sector.SectorBases.Remove(this);

                    if (sector.SectorBases.Count == 0) {
                        sector.Cluster.GrowableSectors.Remove(sector);
                    }
                }
            }
        }
    }
}