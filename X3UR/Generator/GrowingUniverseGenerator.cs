using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using X3UR.Enums;
using X3UR.Helpers;
using X3UR.Models;
using X3UR.Objectives;
using X3UR.ViewModels;
using X3UR.ViewModels.DebugModeViewModels;

namespace X3UR.Generator;

public class GrowingUniverseGenerator {
    /// <summary>
    /// Das Universum, indem sich die Map befindet.
    /// Zur besseren Organisation hat dies auch eine Liste mit Clustern.
    /// Da das Universum einzigartig ist und der Zugriff es einfacher macht, ist es statisch.
    /// </summary>
    public static Universe Universe { get; set; }
    /// <summary>
    /// Um einige Stellen per Zufall ermitteln zu können.
    /// Da es einen Seed bekommt und alles was per Zufall ermittelt wird, mit diesem Seed passieren soll, ist es statisch.
    /// </summary>
    public static Random Random { get; set; }
    /// <summary>
    /// Die Liste aller aktiven Rassen
    /// </summary>
    private List<Race> _races;
    private CancellationTokenSource _cancellationTokenSource;

    /// <summary>
    /// Der Basiswert des Abstands der Cluster zueinander, wenn sie der selben Rasse angehören. 12.5f.
    /// </summary>
    private const float BASEDISTANCESAME = 12.5f;
    /// <summary>
    /// Der Basiswert des Abstands der Cluster zueinander, wenn sie unterschiedlichen Rassen angehören. 1.5f.
    /// </summary>
    private const float BASEDISTANCEDIFF = 1.5f;
    /// <summary>
    /// Der Standard Wert von der Anzahl der Sektoren die im Universum möglich wären, wenn die Größe x22 * y17 beträgt.
    /// </summary>
    private const short BASESECTORCOUNT = 374;
    /// <summary>
    /// Der errechnete Abstand der Cluster zueinander, wenn sie der selben Rasse angehören. 374.
    /// Wird mit der eingestellten Größe des Universums gerechnet.
    /// </summary>
    private float _distanceByMapSizeSame;
    /// <summary>
    /// Der errechnete Abstand der Cluster zueinander, wenn sie unterschiedlichen Rassen angehören.
    /// Wird mit der eingestellten Größe des Universums gerechnet.
    /// </summary>
    private float _distanceByMapSizeDiff;

    public GrowingUniverseGenerator() {
        _cancellationTokenSource = new CancellationTokenSource();
        BaseViewModel.StaticPropertyChanged += OnRefresh;

        StartUniverseGenerationAsync();
    }

    /// <summary>
    /// Generiert das Universum erneut.
    /// </summary>
    private void OnRefresh(object sender, PropertyChangedEventArgs e) {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        StartUniverseGenerationAsync();
    }

    /// <summary>
    /// Durchläuft mehrere Phasen, um das Universum zu generieren.
    /// Initiallisierung, Das füllen des Univerums mit leeren Sektoren,
    /// das Setzen der Cluster jeder Rasse, das Sortieren der Nachbarn eines jeden Clusters
    /// und das wachsen eines jeden Clusters.
    /// </summary>
    private async void StartUniverseGenerationAsync() {
        await Task.Run(Initialize, _cancellationTokenSource.Token);
        await Task.Run(FillUniverseGridWithSpace, _cancellationTokenSource.Token);

        await Task.Run(SetClusterPositions, _cancellationTokenSource.Token);
        await Task.Run(SortClusterNeighbors, _cancellationTokenSource.Token);
        await Task.Run(Grow, _cancellationTokenSource.Token);
    }

    /// <summary>
    /// Initiallisierung. Setzt die Abstände der Cluster zueinander,
    /// übernimmt den eingestellten Seed für Random,
    /// erstellt die Karte des Universums anhand der eingestellten Größe des Universums,
    /// übernimmt die Einstellungen der Rassen und
    /// setzt im UI die Map und den DebugMode zurück.
    /// </summary>
    private void Initialize() {
        _distanceByMapSizeSame = UniverseSettingsViewModel.MapSize * (BASEDISTANCESAME / BASESECTORCOUNT);
        _distanceByMapSizeDiff = UniverseSettingsViewModel.MapSize * (BASEDISTANCEDIFF / BASESECTORCOUNT);
        Random = new Random(BaseViewModel.Seed.GetHashCode());
        //UniverseSettingsViewModel.RaceSettingsModels[8].IsChecked = false;
        Universe = new Universe {
            Map = new SectorBase[UniverseSettingsViewModel.MapHeight, UniverseSettingsViewModel.MapWidht]
        };
        InitRaces();
        MapPreviewViewModel.ClearMap();
        DebugModeRaceListsViewModel.ClearAllLists();
    }

    private void InitRaces() {
        _races = new List<Race>();

        foreach (var raceSetting in UniverseSettingsViewModel.RaceSettingsModels) {
            _races.Add(new Race(
                raceSetting.RaceId,
                raceSetting.Name,
                raceSetting.RaceSize,
                raceSetting.ClusterCount,
                raceSetting.ClusterSize,
                raceSetting.Color
            ));
        }
    }

    /// <summary>
    /// Füllt das Universum mit SectorBases, die dann später zu Sectors werden können.
    /// </summary>
    private void FillUniverseGridWithSpace() {
        for (byte y = 0; y < UniverseSettingsViewModel.MapHeight; y++) {
            ObservableCollection<SectorBase> SectorBases = new();

            for (byte x = 0; x < UniverseSettingsViewModel.MapWidht; x++) {
                SectorBase sectorBase = new(x, y);
                Universe.Map[y ,x] = sectorBase;
                SectorBases.Add(sectorBase);
            }

            MapPreviewViewModel.AddMapList(SectorBases);
        }
    }

    /// <summary>
    /// Erstellt eine 2D-Liste. Die erste Liste stellt die Rasse dar.
    /// Jede Rasse kann mehrere Cluster haben, was die zweite Liste darstellt.
    /// Der Inhalt ist dann der Index zum RaceSettingsModels.
    /// </summary>
    /// <returns></returns>
    private List<List<byte>> Get_race_cluster_raceIndices() {
        List<List<byte>> races = new();

        for (byte i = 0; i < UniverseSettingsViewModel.RaceSettingsModels.Count; i++) {
            if (UniverseSettingsViewModel.RaceSettingsModels[i].ClusterCount > 0) {
                List<byte> clusters = new();

                for (byte j = 0; j < UniverseSettingsViewModel.RaceSettingsModels[i].ClusterCount; j++) {
                    clusters.Add(i);
                }

                races.Add(clusters);
            }
        }

        return races;
    }

    /// <summary>
    /// Kopiert die Reihenfolge der übergebenen 2D-Liste, die die Rassen darstellt.
    /// Der Inhalt ist der erste Index der 2D-Liste.
    /// </summary>
    /// <param name="race_cluster_raceIndices"></param>
    /// <returns></returns>
    private List<byte> Get_race_raceIndices(List<List<byte>> race_cluster_raceIndices) {
        List<byte> races = new();

        for (byte i = 0; i < race_cluster_raceIndices.Count; i++) {
            if (race_cluster_raceIndices[i].Count > 0)
                races.Add(i);
            else {
                race_cluster_raceIndices.RemoveAt(i);
                // DebugMode
                DebugModeRaceListsViewModel.ClearFirstList();
                DebugModeRaceListsViewModel.AddToFirstList(race_cluster_raceIndices);
                i--;
            }
        }

        return races;
    }

    /// <summary>
    /// Hier werden die Cluster jeder Rasse gesetzt.
    /// Dazu wird die 2D-Liste verwendet:
    /// Es gibt mehrere Rassen, jede Rasse kann mehrere Cluster haben, dessen Inhalt die der Index von RaceSettingsModels.
    /// Eine 1D-Liste übernimmt die Rassen von der 2D-Liste, dessen Inhalt der erste Index der 2D-Liste ist.
    /// Eine Schleife wird solange durchlaufen, bis die 2D-Liste leer ist.
    /// Eine weitere Schleife, innerhalb der Ersten, wird solange durchlaufen, bis die 1D-Liste leer ist.
    /// In der zweiten Schleife wird ein Eintrag (Rasse) aus der 1D-Liste per Zufall gewählt und der Eintrag dann aus der Liste entfernt.
    /// Mit dem Eintrag aus der ersten Liste erhalten wir den ersten Index der 2D-Liste.
    /// Nun wird am ersten Index der 2D-Liste, der zweite Index per Zufall gewählt und die Liste (Cluster) aus der 2D-Liste entfernt.
    /// Mit dem ersten (Rasse) und zweiten (Cluster) Index wird aus der 2D-Liste dann der Eintrag (Index von RaceSettingsModels) genommen.
    /// Mit dem Index von RaceSettingsModels können wir nun die Rasse ermitteln und den ersten Cluster mit der ermittelten Rasse setzen.
    /// Wenn die 1D-Liste keine Einträge mehr hat und damit die zweite Schleife endet, wird überprüft, ob die 2D-Liste noch Einträge hat.
    /// Wenn ja, bekommt die 1D-Liste erneut eine Kopie von der Reihenfolge der 2D-Liste.
    /// Wenn eine Liste (Rasse) in der 2D-Liste keine Listen (Cluster) mehr hat, wird diese aus der 2D-Liste entfernt.
    /// Hat die 2D-Liste keine Listen mehr, endet die erste Schleife und alle Cluster wurden gesetzt.
    /// </summary>
    private void SetClusterPositions() {
        List<List<byte>> race_cluster_raceIndices = Get_race_cluster_raceIndices();
        List<byte> race_raceIndices = Get_race_raceIndices(race_cluster_raceIndices);
        List<SectorBase> avaliableSectors_DifferentRace = GetAllSectorBases();
        List<List<SectorBase>> clusterPositions_PerSameRace = new();
        Dictionary<string, List<Enum>> clusterIdsByRace = new();
        byte randomRaceListIndex;
        byte race_cluster_ListIndex;
        byte randomClusterListIndex;
        byte raceIndex;
        List<SectorBase> clusterPositionsSameRace;
        List<SectorBase> avaliableSectors;
        SectorBase randomSectorBase;
        Sector pickedSector;
        Cluster cluster;

        // DebugMode
        DebugModeRaceListsViewModel.AddToFirstList(race_cluster_raceIndices);
        DebugModeRaceListsViewModel.AddToSecondList(race_raceIndices, race_cluster_raceIndices);

        for (byte i = 0; i < UniverseSettingsViewModel.RaceSettingsModels.Count; i++) {
            clusterPositionsSameRace = new();
            clusterPositions_PerSameRace.Add(clusterPositionsSameRace);
        }

        while (race_cluster_raceIndices.Count > 0) {
            while (race_raceIndices.Count > 0) {
                randomRaceListIndex = (byte)Random.Next(race_raceIndices.Count);
                race_cluster_ListIndex = race_raceIndices[randomRaceListIndex];
                race_raceIndices.RemoveAt(randomRaceListIndex);

                randomClusterListIndex = (byte)Random.Next(race_cluster_raceIndices[race_cluster_ListIndex].Count);
                raceIndex = race_cluster_raceIndices[race_cluster_ListIndex][randomClusterListIndex];
                race_cluster_raceIndices[race_cluster_ListIndex].RemoveAt(randomClusterListIndex);

                clusterPositionsSameRace = clusterPositions_PerSameRace[raceIndex];
                avaliableSectors = new(avaliableSectors_DifferentRace);

                for (byte i = 0; i < clusterPositionsSameRace.Count; i++) {
                    RemoveSectorBasesOfSameRaceInRadius(avaliableSectors, raceIndex, clusterPositionsSameRace[i].PosX, clusterPositionsSameRace[i].PosY);
                }

                randomSectorBase = GetRandomSectorBase(avaliableSectors);
                cluster = new Cluster(randomSectorBase.PosX, randomSectorBase.PosY, _races[raceIndex]);
                AddClusterNeighbors(cluster);
                //MessageBox.Show($"{Universe.Clusters.Count} - {cluster.NeighborsTemp.Count}");
                Universe.Clusters.Add(cluster);

                pickedSector = Universe.Map[randomSectorBase.PosY, randomSectorBase.PosX].ConvertToSector();
                pickedSector.InitSector(cluster);

                // Für DebugMode
                if (clusterIdsByRace.TryGetValue(cluster.Race.Name, out List<Enum> value)) {
                    value.Add(cluster.Race.Id);
                } else {
                    clusterIdsByRace.Add(cluster.Race.Name, new List<Enum> { cluster.Race.Id });
                }

                cluster.Id = (byte)(clusterIdsByRace[cluster.Race.Name].Count + 1);

                // MapPreview
                MapPreviewViewModel.AddSector(pickedSector);

                // DebugMode
                DebugModeRaceInfosViewModel.SetRaceInfos(pickedSector);
                DebugModeRaceListsViewModel.ReduceFromFirstList(race_cluster_ListIndex);
                DebugModeRaceListsViewModel.MoveFromSecondToThirdList(randomRaceListIndex);

                clusterPositionsSameRace.Add(randomSectorBase);

                RemoveSectorDummiesOfDifferentRaceInRadius(avaliableSectors_DifferentRace, randomSectorBase.PosX, randomSectorBase.PosY);
            }

            if (race_raceIndices.Count == 0 && race_cluster_raceIndices.Count > 0) {
                race_raceIndices = Get_race_raceIndices(race_cluster_raceIndices);

                // DebugMode
                DebugModeRaceListsViewModel.ClearSecondList();
                DebugModeRaceListsViewModel.AddToSecondList(race_raceIndices, race_cluster_raceIndices);
            }
        }
        MessageBox.Show("Ende");
    }

    /// <summary>
    /// Erstellt eine Liste mit allen SectorBases aus Universe.Map und gibt diese zurück.
    /// </summary>
    /// <returns></returns>
    private List<SectorBase> GetAllSectorBases() {
        List<SectorBase> sectorBases = new();
        for (byte y = 0; y < Universe.Map.GetLength(0); y++) {
            for (byte x = 0; x < Universe.Map.GetLength(1); x++) {
                sectorBases.Add(new SectorBase(x, y));
            }
        }

        return sectorBases;
    }

    /// <summary>
    /// Nimmt einen zufälligen SectorBase aus der übergebenen Liste,
    /// entfernt diesen daraus und gibt ihn anschließend zurück.
    /// </summary>
    /// <param name="availableSectors"></param>
    /// <returns></returns>
    private SectorBase GetRandomSectorBase(List<SectorBase> availableSectors) {
        short randomIndex = (short)Random.Next(availableSectors.Count);
        SectorBase randomSectorBase = availableSectors[randomIndex];
        availableSectors.RemoveAt(randomIndex);
        return randomSectorBase;
    }

    /// <summary>
    /// Überprüft die Abstände jedes SectorBases aus der übergebenen Liste mit den übergebenen Koordinaten.
    /// Mit dem übergebenen raceIndex wird ein Mindestabstand anhand der maximalen Anzahl an Clustern dieser Rasse ermittelt.
    /// Jeder SectorBase der unterhalb des Mindestabstandes ist, wird aus der übergebenen Liste entfernt.
    /// </summary>
    /// <param name="avalibaleSectors_SameRace"></param>
    /// <param name="raceIndex"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void RemoveSectorBasesOfSameRaceInRadius(List<SectorBase> avalibaleSectors_SameRace, byte raceIndex, byte posX, byte posY) {
        for (short i = 0; i < avalibaleSectors_SameRace.Count; i++) {
            float distance = MathHelpers.DistanceOfTwoPoints2D(posX, posY, avalibaleSectors_SameRace[i].PosX, avalibaleSectors_SameRace[i].PosY);
            float minDistance = (float)Math.Round((float)((float)2 / UniverseSettingsViewModel.RaceSettingsModels[raceIndex].ClusterCount * _distanceByMapSizeSame), 1);

            if (distance < minDistance) {
                avalibaleSectors_SameRace.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Überprüft die Abstände jedes SectorBases aus der übergebenen Liste mit den übergebenen Koordinaten.
    /// Jeder SectorBase der unterhalb des Mindestabstandes ist, wird aus der übergebenen Liste entfernt.
    /// </summary>
    /// <param name="avalibaleSectors_DifferentRace"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    private void RemoveSectorDummiesOfDifferentRaceInRadius(List<SectorBase> avalibaleSectors_DifferentRace, byte posX, byte posY) {
        float minDistance = _distanceByMapSizeDiff;

        for (short i = 0; i < avalibaleSectors_DifferentRace.Count; i++) {
            float distance = MathHelpers.DistanceOfTwoPoints2D(posX, posY, avalibaleSectors_DifferentRace[i].PosX, avalibaleSectors_DifferentRace[i].PosY);

            if (distance < minDistance) {
                avalibaleSectors_DifferentRace.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// Errechnet einen Mindestabstand und überprüft,
    /// ob der Abstand des übergebenen Clusters mit einen aus den Universe.Clusters unter dem mindestabstand liegt.
    /// Wenn ja, fügen sich beide Cluster gegenseitig in ihre Neighbors-Liste.
    /// </summary>
    /// <param name="pickedCluster"></param>
    private void AddClusterNeighbors(Cluster pickedCluster) {
        if (Universe.Clusters.Count > 0) {
            float maxDistance = 8f / BASESECTORCOUNT * UniverseSettingsViewModel.MapSize;

            foreach (Cluster cluster in Universe.Clusters) {
                if (pickedCluster.Race.Id == RaceNames.Khaak || cluster.Race.Id == RaceNames.Khaak)
                    continue;

                float distance = MathHelpers.DistanceOfTwoPoints2D(pickedCluster.PosX, pickedCluster.PosY, cluster.PosX, cluster.PosY);

                if (distance < maxDistance) {
                    pickedCluster.NeighborsTemp.Add((cluster, distance));
                    cluster.NeighborsTemp.Add((pickedCluster, distance));
                }
            }
        }
    }

    /// <summary>
    /// Sortiert die Neighbors alle Cluster im Universum
    /// </summary>
    private void SortClusterNeighbors() {
        foreach (Cluster cluster in Universe.Clusters) {
            cluster.NeighborsTemp.Sort((a, b) => a.Distance.CompareTo(b.Distance));

            foreach ((Cluster neighbor, float distance) in cluster.NeighborsTemp) {
                cluster.Neighbors.Add(neighbor);
            }

            cluster.ClearNeighborsTemp();
        }
    }

    /// <summary>
    /// Erstellt eine 2D-Liste dessen erster Index die Rasse darstellt und der zweite Index dessen Cluster.
    /// Der Inhalt ist der Index von Universe.Clusters, der zu den entsprechenden Cluster verweist.
    /// Anschließend wird die 2D-Liste zurückgegeben.
    /// </summary>
    /// <returns></returns>
    private List<List<byte>> Get_race_cluster_universeClusterIndices() {
        List<List<byte>> races = new();

        // Die ersten Indexe der Liste werden nach den Rassen sortiert werden.
        foreach (RaceSettingsModel raceSettingsModel in UniverseSettingsViewModel.RaceSettingsModels) {
            // Wenn die Größe eines Clusters der Rasse kleiner als 2 ist, soll diese nicht hinzugefügt werden.
            if (raceSettingsModel.ClusterSizeMax < 2) continue;

            List<byte> sector = new();

            for (byte i = 0; i < Universe.Clusters.Count; i++) {
                if (Universe.Clusters[i].Race.Id == raceSettingsModel.RaceId) {
                    sector.Add(i);
                }
            }

            races.Add(sector);
        }

        return races;
    }

    private List<byte> Get_race_universeClusterIndices(List<List<byte>> universeClusterIndices) {
        List<byte> raceIndices = new();

        for (byte i = 0; i < universeClusterIndices.Count; i++) {
            if (universeClusterIndices[i].Any(clusterIndex => IsGrowConditions(Universe.Clusters[clusterIndex]))) {
                raceIndices.Add(i);
            } else {
                universeClusterIndices.RemoveAt(i);
                // DebugMode
                DebugModeRaceListsViewModel.ClearFirstList();
                DebugModeRaceListsViewModel.AddToFirstList2(universeClusterIndices);
                i--;
            }
        }

        return raceIndices;
    }

    /// <summary>
    /// Jede Rasse lässt einen zufälligen seiner gesetzten Cluster im Universum wachsen.
    /// Welche Rasse an der Reihe ist, wird per Zufall ermittelt.
    /// Hat jede Rasse einen seiner Cluster wachsen lassen, geht es wierder von vorne los,
    /// bis keine Rasse oder eines seiner Cluster mehr wachsen kann.
    /// </summary>
    public void Grow() {
        // [race][cluster]<Universe.Cluster Index>
        List<List<byte>> race_cluster_universeClusterIndices = Get_race_cluster_universeClusterIndices();
        // [race]<Universe.Cluster Index>
        List<byte> race_universeClusterIndices = Get_race_universeClusterIndices(race_cluster_universeClusterIndices);
        byte randomRaceListIndex;
        byte universeClusterIndices_randomRaceIndex;
        byte randomClusterListIndex;
        byte universeClusterIndex;
        Cluster pickedCluster;
        Sector claimingSector;

        // DebugMode
        DebugModeRaceListsViewModel.AddToFirstList2(race_cluster_universeClusterIndices);
        DebugModeRaceListsViewModel.AddToSecondList2(race_universeClusterIndices, race_cluster_universeClusterIndices);

        // solange [race] mit [cluster] noch vorhanden sind
        while (race_cluster_universeClusterIndices.Count > 0) {
            // solange [race] vorhanden sind
            while (race_universeClusterIndices.Count > 0) {
                randomRaceListIndex = (byte)Random.Next(race_universeClusterIndices.Count);
                universeClusterIndices_randomRaceIndex = race_universeClusterIndices[randomRaceListIndex];
                race_universeClusterIndices.RemoveAt(randomRaceListIndex);

                randomClusterListIndex = (byte)Random.Next(race_cluster_universeClusterIndices[universeClusterIndices_randomRaceIndex].Count);
                universeClusterIndex = race_cluster_universeClusterIndices[universeClusterIndices_randomRaceIndex][randomClusterListIndex];
                pickedCluster = Universe.Clusters[universeClusterIndex];

                if (!IsGrowConditions(pickedCluster)) {
                    DebugModeRaceListsViewModel.RemoveFromSecondList(randomRaceListIndex);
                    continue;
                }

                claimingSector = pickedCluster.ClaimSector(Random);
                Universe.Map[claimingSector.PosY, claimingSector.PosX] = claimingSector;
                MapPreviewViewModel.AddSector(claimingSector);

                // DebugMode
                DebugModeRaceInfosViewModel.SetRaceInfos(claimingSector);
                DebugModeRaceListsViewModel.MoveFromSecondToThirdList(randomRaceListIndex);
            }

            if (race_universeClusterIndices.Count == 0 && race_cluster_universeClusterIndices.Count > 0) {
                race_universeClusterIndices = Get_race_universeClusterIndices(race_cluster_universeClusterIndices);

                // DebugMode
                DebugModeRaceListsViewModel.ClearSecondList();
                DebugModeRaceListsViewModel.AddToSecondList2(race_universeClusterIndices, race_cluster_universeClusterIndices);
            }
        }
        MessageBox.Show("Ich habe Fertig");
    }

    /// <summary>
    /// Prüft ob der übergebene Cluster die Bedingungen erfüllt.
    /// Der Cluster muss mindestens 1 GrowableSector haben.
    /// Die Größe eines Clusters (Anzahl seiner Sectors) darf nicht über seine eingestellte maximale Größe sein.
    /// Die Größe der Rasse des Clusters darf nicht über seine eingestellte maximale Größe sein.
    /// </summary>
    /// <param name="cluster"></param>
    /// <returns></returns>
    private bool IsGrowConditions(Cluster cluster) {
        if (cluster.GrowableSectors.Count < 1
            || cluster.Sectors.Count >= cluster.Race.MaxClusterSize
            || cluster.Race.CurrentSize >= cluster.Race.MaxSize)
            return false;
        return true;
    }
}