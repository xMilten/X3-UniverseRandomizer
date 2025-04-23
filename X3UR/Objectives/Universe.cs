using System.Collections.Generic;

namespace X3UR.Objectives;

public class Universe {
    public SectorBase[,] Map { get; set; }
    public List<Cluster> Clusters { get; }

    public Universe() {
        Clusters = new List<Cluster>();
    }
}