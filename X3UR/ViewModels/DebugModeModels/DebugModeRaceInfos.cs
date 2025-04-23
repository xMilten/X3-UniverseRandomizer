using System.Runtime.Serialization.Formatters;
using System.Windows.Media;
using X3UR.Generator;
using X3UR.Models;
using X3UR.Objectives;

namespace X3UR.ViewModels.DebugModeModels {
    public class DebugModeRaceInfos {
        public string RaceName {get; set;}
        public string RaceSize { get; set; }
        public string RaceClusters { get; set; }
        public string UniverseSize { get; set; }
        public short SectorId { get; set; }
        public string SectorCoords { get; set; }
        public short SectorsClaimable { get; set; }
        public short SectorsCanClaimMe { get; set; }
        public short ClusterId { get; set; }
        public string ClusterCoords { get; set; }
        public string ClusterSize { get; set; }
        public string ClusterNeighbor { get; set; }
        public short ClusterGrowableSectors { get; set; }
        public string NeighborNorth { get; set; }
        public string NeighborEast { get; set; }
        public string NeighborSouth { get; set; }
        public string NeighborWest { get; set; }
        public Brush NeighborNorthBackground { get; set; }
        public Brush NeighborEastBackground { get; set; }
        public Brush NeighborSouthBackground { get; set; }
        public Brush NeighborWestBackground { get; set; }

        public DebugModeRaceInfos(Sector sector) {
            Race race = sector.Race;

            RaceName = race.Name;
            RaceSize = $"{race.CurrentSize} / {race.MaxSize}";
            RaceClusters = $"{race.CurrentClusterCount} / {race.MaxClusterCount}";
            UniverseSize = $"{CalculateUniverseSize(GrowingUniverseGenerator.Universe.Map)} / {UniverseSettingsViewModel.MapSize}";
            SectorId = sector.Id;
            SectorCoords = $"{sector.PosX} : {sector.PosY}";
            SectorsClaimable = (short)sector.SectorBases.Count;
            SectorsCanClaimMe = (short)sector.SectorsCanClaimMe.Count;
            ClusterId = sector.Cluster.Id;
            ClusterCoords = $"{sector.Cluster.PosX} : {sector.Cluster.PosY}";
            ClusterSize = $"{(short)sector.Cluster.Sectors.Count} / {race.MaxClusterSize}";
            if (sector.Cluster.NeighborsTemp != null && sector.Cluster.NeighborsTemp.Count > 0)
                ClusterNeighbor = $"{sector.Cluster.NeighborsTemp[0].Neighbor.PosX} : {sector.Cluster.NeighborsTemp[0].Neighbor.PosY}";
            else if (sector.Cluster.Neighbors.Count > 0 && sector.Cluster.IsNeighborGrowable(out Cluster growableNeighbor))
                ClusterNeighbor = $"{growableNeighbor.PosX} : {growableNeighbor.PosY}";
            ClusterGrowableSectors = (short)sector.Cluster.GrowableSectors.Count;

            SectorBase[,] sectorBases = GrowingUniverseGenerator.Universe.Map;

            if (sector.PosY != 0) {
                SetRaceInfosHelper(sectorBases[sector.PosY - 1, sector.PosX], 'N');
            } else {
                NeighborNorthBackground = Brushes.Black;
            }
            if (sector.PosX != sectorBases.GetLength(1) - 1) {
                SetRaceInfosHelper(sectorBases[sector.PosY, sector.PosX + 1], 'E');
            } else {
                NeighborEastBackground = Brushes.Black;
            }
            if (sector.PosY != sectorBases.GetLength(0) - 1) {
                SetRaceInfosHelper(sectorBases[sector.PosY + 1, sector.PosX], 'S');
            } else {
                NeighborSouthBackground = Brushes.Black;
            }
            if (sector.PosX != 0) {
                SetRaceInfosHelper(sectorBases[sector.PosY, sector.PosX - 1], 'W');
            } else {
                NeighborWestBackground = Brushes.Black;
            }
        }

        public DebugModeRaceInfos(SectorBase sectorBase) {
            SectorCoords = $"{sectorBase.PosX} : {sectorBase.PosY}";
            SectorsCanClaimMe = (short)sectorBase.SectorsCanClaimMe.Count;

            SectorBase[,] sectorBases = GrowingUniverseGenerator.Universe.Map;

            if (sectorBase.PosY != 0) {
                SetRaceInfosHelper(sectorBases[sectorBase.PosY - 1, sectorBase.PosX], 'N');
            } else {
                NeighborNorthBackground = Brushes.Black;
            }
            if (sectorBase.PosX != sectorBases.GetLength(1) - 1) {
                SetRaceInfosHelper(sectorBases[sectorBase.PosY, sectorBase.PosX + 1], 'E');
            } else {
                NeighborEastBackground = Brushes.Black;
            }
            if (sectorBase.PosY != sectorBases.GetLength(0) - 1) {
                SetRaceInfosHelper(sectorBases[sectorBase.PosY + 1, sectorBase.PosX], 'S');
            } else {
                NeighborSouthBackground = Brushes.Black;
            }
            if (sectorBase.PosX != 0) {
                SetRaceInfosHelper(sectorBases[sectorBase.PosY, sectorBase.PosX - 1], 'W');
            } else {
                NeighborWestBackground = Brushes.Black;
            }
        }

        private void SetRaceInfosHelper(SectorBase neighborSectorBase, char direction) {
            if (neighborSectorBase is Sector neighborSector) {
                if (direction == 'N')
                    NeighborNorth = neighborSector.Race.Name;
                if (direction == 'E')
                    NeighborEast = neighborSector.Race.Name;
                if (direction == 'S')
                    NeighborSouth = neighborSector.Race.Name;
                if (direction == 'W')
                    NeighborWest = neighborSector.Race.Name;
            }
        }

        private short CalculateUniverseSize(SectorBase[,] sectorBases) {
            short value = 0;

            for (byte y = 0; y < sectorBases.GetLength(0); y++) {
                for (byte x = 0; x < sectorBases.GetLength(1); x++) {
                    if (sectorBases[y, x] is Sector) {
                        value++;
                    }
                }
            }

            return value;
        }
    }
}