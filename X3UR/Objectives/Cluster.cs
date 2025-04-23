using System;
using System.Collections.Generic;
using System.Linq;
using X3UR.Helpers;
using X3UR.Models;

namespace X3UR.Objectives;

/// <summary>
/// Der Cluster einer Race. Jede Race kann ein bis mehrere Cluster haben. Ein Cluster kann ein bis mehrere Sectors haben.
/// </summary>
public class Cluster {
    public byte PosX { get; private set; }
    public byte PosY { get; private set; }
    /// <summary>
    /// Die ID des Clusters
    /// </summary>
    public byte Id { get; set; }
    /// <summary>
    /// Die Rasse, die der Cluster angehört
    /// </summary>
    public Race Race { get; private set; }
    /// <summary>
    /// Die Sektoren, die dem Cluster angehören
    /// </summary>
    public List<Sector> Sectors { get; private set; }
    /// <summary>
    /// Eine vorrübergehende Tulp-List zum sortieren der Nachbarn
    /// </summary>
    public List<(Cluster Neighbor, float Distance)> NeighborsTemp { get; private set; }
    /// <summary>
    /// Die benachbarten Cluster, zu den der Cluster wachsen könnte
    /// </summary>
    public List<Cluster> Neighbors { get; private set; }
    /// <summary>
    /// Die Sektoren, die der Cluster beim wachsen übernehmen könnte
    /// </summary>
    public List<Sector> GrowableSectors { get; private set; }

    public Cluster(byte posX, byte posY, Race race) {
        PosX = posX;
        PosY = posY;
        Race = race;
        Race.CurrentClusterCount++;
        Sectors = new List<Sector>();
        NeighborsTemp = new List<(Cluster Neighbor, float Distance)>();
        Neighbors = new List<Cluster>();
        GrowableSectors = new List<Sector>();
    }

    /// <summary>
    /// Setzt die NeighborsTemp-Liste auf null
    /// </summary>
    public void ClearNeighborsTemp() {
        NeighborsTemp = null;
    }

    /// <summary>
    /// Sucht einen growableSector aus seiner Liste, der am Nächsten zum benachbarten Cluster ist
    /// oder wählt einen zufälligen Sektor aus seiner Liste, falls keine benachbarten Cluster mehr vorhanden sind,
    /// fügt diesen dann zu sich hinzu, entfernt ihn aus den Listen "GrowableSectors" von sich und der anderen Cluster.
    /// Anschließend wird die Liste "SectorFromClusterCanClaimMe" vom growableSector gelöscht.
    /// </summary>
    /// <param name="random"></param>
    public Sector ClaimSector(Random random) {
        Sector pickedGrowableSector = null;
        SectorBase sectorBaseToClaim;

        // TODO: Andere vorgehensweise für das Wachsen zu einem Nachbarn finden
        // ChatGPT nach einer Möglichkeit fragen, zum nächsten Sektor eines Nachbarn zu wachsen.
        // Cluster die sich zum ersten mal berührt haben, sollen nie wieder versuchen zueinander zu wachsen
        // und stattdessen zum nächsten Cluster wachsen, dessen Sektor am Nächsten ist.
        // Die Cluster der selben Rasse sollen als Anpeilung zum hinnachsen niedrig prorisiert werden.

        if (IsNeighborGrowable(out Cluster growableNeighbor)) {
            double currentDistance = 10;
            double distance;

            // Suche den Sektor zum nächsten Nachbarn aus der GrowableSectors-Liste
            foreach (Sector growableSector in GrowableSectors) {
                distance = MathHelpers.DistanceOfTwoPoints2D(growableNeighbor.PosX, growableNeighbor.PosY, growableSector.PosX, growableSector.PosY);

                if (distance < currentDistance) {
                    currentDistance = distance;
                    pickedGrowableSector = growableSector;
                }
            }

            // Suche den Sektor zum nächsten Nachbarn aus der ClaimableSectors-Liste
            sectorBaseToClaim = pickedGrowableSector.GetSectorBase(growableNeighbor.PosX, growableNeighbor.PosY);
        } else {
            // Wähle einen zufälligen Sektor aus der GrowableSectors-Liste
            pickedGrowableSector = GrowableSectors[random.Next(GrowableSectors.Count)];
            // Wähle einen zufälligen Sektor aus der ClaimableSectors-Liste
            sectorBaseToClaim = pickedGrowableSector.SectorBases[random.Next(pickedGrowableSector.SectorBases.Count)];
        }

        if (sectorBaseToClaim.SectorsCanClaimMe.Count > 0) {
            sectorBaseToClaim.RemoveSectorBases();
        }

        Sector claimedSector = sectorBaseToClaim.ConvertToSector();
        claimedSector.InitSector(this);
        return claimedSector;
    }

    /// <summary>
    /// Überprüft, ob der nächste Neighbor mindestens einen Sector hat, der mehr als 2 Plätze (SectoBases) zum wachsen hat.
    /// </summary>
    /// <returns></returns>
    public bool IsNeighborGrowable(out Cluster growableNeighbor) {
        foreach (Cluster neighbor in Neighbors) {
            if (neighbor.GrowableSectors.Any(sector => sector.SectorBases.Count > 2)) {
                growableNeighbor = neighbor;
                return true;
            }
        }

        growableNeighbor = null;
        return false;
    }
}