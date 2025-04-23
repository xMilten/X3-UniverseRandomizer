using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using X3UR.Generator;
using X3UR.Helpers;
using X3UR.Models;

namespace X3UR.Objectives;
/// <summary>
/// Der Sector der sich im Universum und in seinem entsprechenden Cluster befindet.
/// Es kennt seinen Cluster, seine Rasse und welche SectorBases an ihm angrenzen.
/// </summary>
public class Sector : SectorBase {
    /// <summary>
    /// Der Cluster zu dem der Sektor gehört
    /// </summary>
    public Cluster Cluster { get; private set; }
    /// <summary>
    /// Die Rasse, die dem Sektor angehört
    /// </summary>
    public Race Race { get; protected set; }
    /// <summary>
    /// Die ID des Sektors
    /// </summary>
    public short Id { get; private set; }

    public bool IsCore { get; set; }
    public long Size { get; set; }
    public int Music { get; set; }
    public long Population { get; set; }
    public byte QTrade { get; set; }
    public byte QFight { get; set; }
    public byte QBuild { get; set; }
    public byte QThink { get; set; }

    public List<SectorObject> SectorObjects { get; private set; }
    /// <summary>
    /// Die SectorBases die an diesem Sector angrenzen.
    /// </summary>
    public List<SectorBase> SectorBases { get; private set; }

    public Sector(byte posX, byte posY) : base(posX, posY) {
        SectorBases = new List<SectorBase>();
    }

    public void InitSector(Cluster cluster) {
        Cluster = cluster;
        Cluster.Sectors.Add(this);
        Race = Cluster.Race;
        Race.CurrentSize++;
        Id = (short)Cluster.Sectors.Count;
        Visibility = Visibility.Visible;

        AddClaimableSectors();
    }

    /// <summary>
    /// Gibt den SectorBase zurück der sich am nächsten zu den übergebenen Koordinaten befindet.
    /// </summary>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    /// <returns></returns>
    public SectorBase GetSectorBase(byte posX, byte posY) {
        SectorBase pickedClaimableSector = null;
        double currentDistance = 10;
        double distance;

        if (SectorBases.Count > 1) {
            foreach (SectorBase claimableSectorBase in SectorBases) {
                distance = MathHelpers.DistanceOfTwoPoints2D(posX, posY, claimableSectorBase.PosX, claimableSectorBase.PosY);
                Debug.WriteLine($"Nachbar: {posX} : {posY}, SectorBase: {claimableSectorBase.PosX} : {claimableSectorBase.PosY}");

                if (distance < currentDistance) {
                    currentDistance = distance;
                    pickedClaimableSector = claimableSectorBase;
                }
            }
        } else {
            pickedClaimableSector = SectorBases[0];
        }
        return pickedClaimableSector;
    }

    /// <summary>
    /// Überprüft, ob es angrenzende SectorBases oder Sectors gibt.
    /// </summary>
    private void AddClaimableSectors() {
        if (PosY > 0) {
            SectorBase top = GrowingUniverseGenerator.Universe.Map[PosY - 1, PosX];

            AddClaimableSectorsHelper(top);
        }
        if (PosY < GrowingUniverseGenerator.Universe.Map.GetLength(0) - 1) {
            SectorBase bottom = GrowingUniverseGenerator.Universe.Map[PosY + 1, PosX];

            AddClaimableSectorsHelper(bottom);
        }
        if (PosX > 0) {
            SectorBase left = GrowingUniverseGenerator.Universe.Map[PosY, PosX - 1];

            AddClaimableSectorsHelper(left);
        }
        if (PosX < GrowingUniverseGenerator.Universe.Map.GetLength(1) - 1) {
            SectorBase right = GrowingUniverseGenerator.Universe.Map[PosY, PosX + 1];

            AddClaimableSectorsHelper(right);
        }

        // Hat dieser Sector SectorBases um sich herum,
        // füge ihn in die GrowableSectors-Liste, seines Clusters.
        if (SectorBases.Count > 0) {
            Cluster.GrowableSectors.Add(this);
        }
    }

    /// <summary>
    /// Überprüft, ob der übergebene SectorBase ein Sector ist.
    /// Wenn ja, wird überprüft, ob sich dessen Cluster in der Neighbors-Liste befindet
    /// und sorgt dafür, dass sich beide Cluster gegenseitig aus ihrer Neighbors-Liste entfernen,
    /// wenn der Cluster gefunden wurde.
    /// Sollte es kein Sector sein, wird dieser in die SectorBases-Liste als angrenzender SectoBase hinzugefügt.
    /// Anschließend wird dieser Sector in die SectorsCanClaimMe-Liste des SectorBase hinzugefügt.
    /// </summary>
    /// <param name="claimableBaseSector"></param>
    private void AddClaimableSectorsHelper(SectorBase claimableBaseSector) {
        if (claimableBaseSector is Sector claimableSector) {
            // Wenn Nachbarn aufeinander treffen, entfernen sie sich gegenseitig.
            if (Cluster.Neighbors.Any(neighbor => neighbor == claimableSector.Cluster)) {
                Cluster.Neighbors.Remove(claimableSector.Cluster);
                claimableSector.Cluster.Neighbors.Remove(Cluster);
            }
        } else {
            // Nur wenn es ein SectorBase ist, soll der benachbarte SectorBase in die Liste.
            SectorBases.Add(claimableBaseSector);
            claimableBaseSector.SectorsCanClaimMe.Add(this);

            /*
            if (claimableBaseSector.Color == Color.FromArgb(0, 0, 0, 0)) {
                claimableBaseSector.Color = Color.FromArgb(64, Race.Color.R, Race.Color.G, Race.Color.B);
            } else {
                byte r = (byte)(claimableBaseSector.Color.R + (Race.Color.R - claimableBaseSector.Color.R) * 0.5);
                byte g = (byte)(claimableBaseSector.Color.G + (Race.Color.G - claimableBaseSector.Color.G) * 0.5);
                byte b = (byte)(claimableBaseSector.Color.B + (Race.Color.B - claimableBaseSector.Color.B) * 0.5);
                claimableBaseSector.Color = Color.FromArgb(128, r, g, b);
            }

            // DebugMode
            MapPreviewViewModel.AddSectorBase(claimableBaseSector);
            */
        }
    }
}