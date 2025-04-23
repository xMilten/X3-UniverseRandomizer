using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using X3UR.Enums;
using X3UR.Models;

namespace X3UR.ViewModels;

public static class UniverseSettingsViewModel {
    private static byte _mapWidht = 22;
    private static byte _mapWidhtMin = 5;
    private static byte _mapWidhtMax = 24;
    private static byte _mapHeight = 17;
    private static byte _mapHeightMin = 5;
    private static byte _mapHeightMax = 20;
    private static short _mapSize = (short)(_mapWidht * _mapHeight);
    private static short _totalSectors = CalculateTotalSectors();
    private static string _totalSectorsPercentage = CalculateTotalSectorsPercentage();
    private static byte raceIndex;

    public static byte MapWidht {
        get => _mapWidht;
        set {
            if (_mapWidht != value) {
                if (value < _mapWidhtMin) {
                    _mapWidht = _mapWidhtMin;
                }
                else if (value > _mapWidhtMax) {
                    _mapWidht = _mapWidhtMax;
                } else {
                    _mapWidht = value;
                }
                NotifyStaticPropertyChanged();

                OnMapWidthChanged();
            }
        }
    }

    public static byte MapWidhtMin {
        get => _mapWidhtMin;
        set {
            if (_mapWidhtMin != value) {
                _mapWidhtMin = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static byte MapWidhtMax {
        get => _mapWidhtMax;
        set {
            if (_mapWidhtMax != value) {
                _mapWidhtMax = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static byte MapHeight {
        get => _mapHeight;
        set {
            if (_mapHeight != value) {
                if (value < _mapHeightMin) {
                    _mapHeight = _mapHeightMin;
                }
                else if (value > _mapHeightMax) {
                    _mapHeight = _mapHeightMax;
                } else {
                    _mapHeight = value;
                }
                NotifyStaticPropertyChanged();

                OnMapHeightChanged();
            }
        }
    }

    public static byte MapHeightMin {
        get => _mapHeightMin;
        set {
            if (_mapHeightMin != value) {
                _mapHeightMin = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static byte MapHeightMax {
        get => _mapHeightMax;
        set {
            if (_mapHeightMax != value) {
                _mapHeightMax = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gibt die MapWidth * MapHeigt
    /// </summary>
    public static short MapSize {
        get => _mapSize;
        set {
            if (_mapSize != value) {
                _mapSize = value;
                NotifyStaticPropertyChanged();

                OnMapSizeChanged();
            }
        }
    }

    public static short TotalSectors {
        get => _totalSectors;
        set {
            if (_totalSectors != value) {
                _totalSectors = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static string TotalSectorsPercentage {
        get => _totalSectorsPercentage;
        set {
            if (_totalSectorsPercentage != value) {
                _totalSectorsPercentage = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static List<RaceSettingsModel> RaceSettingsModels { get; private set; }

    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    private static void NotifyStaticPropertyChanged([CallerMemberName] string propertyName = "") {
        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }

    private static void InitRaces() {
        RaceSettingsModels = new List<RaceSettingsModel> {
            new(RaceNames.Argon, HexToColor("#1e1ee1"), 32, 3, 16),
            new(RaceNames.Boron, HexToColor("#1ee11e"), 27, 5, 7),
            new(RaceNames.Split, HexToColor("#e11ee1"), 31, 4, 13),
            new(RaceNames.Paranid, HexToColor("#1ee1e1"), 27, 2, 14),
            new(RaceNames.Teladi, HexToColor("#e1e11e"), 30, 4, 9),
            new(RaceNames.Xenon, HexToColor("#e11e1e"), 10, 6, 4),
            new(RaceNames.Khaak, HexToColor("#701ee1"), 3, 3, 1),
            new(RaceNames.Piraten, HexToColor("#e1701e"), 20, 8, 8),
            new(RaceNames.Unbekannt, HexToColor("#a0a0a0"), 13, 11, 2),
            new(RaceNames.Terraner, HexToColor("#dfffbf"), 21, 1, 21),
            new(RaceNames.Yaki, HexToColor("#ffbfdf"), 3, 1, 3)
        };

        foreach (var racesettingsModel in RaceSettingsModels) {
            racesettingsModel.PropertyChanged += RaceSettingsModel_PropertyChanged;
        }
    }

    private static void RaceSettingsModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == "RaceSize") {
            TotalSectors = CalculateTotalSectors();
            TotalSectorsPercentage = CalculateTotalSectorsPercentage();

            CalculateRaceSizes(sender);
        }
    }

    public static void OnKeyEnter() {
        var simulateInput = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab) { RoutedEvent = Keyboard.KeyDownEvent };
        InputManager.Current.ProcessInput(simulateInput);
    }

    private static void CalculateRaceSizes(object sender) {
        double temp = Convert.ToDouble(_totalSectorsPercentage.Remove(_totalSectorsPercentage.Length - 1));
        if (temp > 100) {
            while (temp > 100) {
                if (RaceSettingsModels[raceIndex] != sender && RaceSettingsModels[raceIndex].RaceSize != 0) {
                    RaceSettingsModels[raceIndex].RaceSize--;
                    temp = Convert.ToDouble(_totalSectorsPercentage.Remove(_totalSectorsPercentage.Length - 1));
                }

                raceIndex++;
                if (raceIndex == RaceSettingsModels.Count) raceIndex = 0;
            }
        }
    }

    private static void CalculateRaceSizes() {
        double temp = Convert.ToDouble(_totalSectorsPercentage.Remove(_totalSectorsPercentage.Length - 1));
        if (temp > 100) {
            while (temp > 100) {
                if (RaceSettingsModels[raceIndex].RaceSize != 0) {
                    RaceSettingsModels[raceIndex].RaceSize--;
                    temp = Convert.ToDouble(_totalSectorsPercentage.Remove(_totalSectorsPercentage.Length - 1));
                }

                raceIndex++;
                if (raceIndex == RaceSettingsModels.Count) raceIndex = 0;
            }
        }
    }

    private static void CalculateMapSize() {
        MapSize = (short)(_mapWidht * _mapHeight);
    }

    private static short CalculateTotalSectors() {
        if (RaceSettingsModels == null) {
            InitRaces();
        }

        short value = 0;
        foreach (RaceSettingsModel raceSettingsModel in RaceSettingsModels) {
            value += raceSettingsModel.RaceSize;
        }

        return value;
    }

    private static string CalculateTotalSectorsPercentage() {
        return ((float)TotalSectors / _mapSize).ToString("0.00%");
    }

    private static void OnMapWidthChanged() {
        CalculateMapSize();
    }

    private static void OnMapHeightChanged() {
        CalculateMapSize();
    }

    private static void OnMapSizeChanged() {
        TotalSectors = CalculateTotalSectors();
        TotalSectorsPercentage = CalculateTotalSectorsPercentage();

        foreach (RaceSettingsModel raceSettingsModel in RaceSettingsModels) {
            raceSettingsModel.MapSize = MapSize;
        }

        CalculateRaceSizes();
    }

    private static Color HexToColor(string hex) {
        return (Color)ColorConverter.ConvertFromString(hex);
    }
}