using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X3UR.Objectives;

public enum ObjectTypes {
    UNKNOWN00,
    Sector,
    Background,
    Sun,
    Planet,
    tradingDock,
    Factory,
    Ship,
    Laser,
    Shield,
    Missle,
    Energy,
    Novelty,
    Basic,
    Consumable,
    Mineral,
    Tech,
    Asteroid,
    Gate,
    UNKNOWN19,
    Special,
    Nebulae,
    UNKNOWN22,
    Container,
    ScriptC,
    UNKNOWN25,
    ScriptV,
    UNKNOWN27,
    Debris,
    WreckDock,
    WreckFactory,
    WreckShip
}

public class SectorObject {
    internal int t = (int)ObjectTypes.UNKNOWN00;
}