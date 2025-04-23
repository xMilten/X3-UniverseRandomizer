using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using X3UR.Enums;
using X3UR.ViewModels;

namespace X3UR.Models;

public class RaceSettingsModel : INotifyPropertyChanged {
    private RaceNames _raceId;
    private bool _isChecked;
    private Color _color;
    private string _name;
    private short _raceSize;
    private short _cluster;
    private short _clusterSize;
    // Only UI relevant
    private short _raceSizeMin = 0;
    private short _raceSizeMax = 50;
    private short _clusterMin;
    private short _clusterMax;
    private short _clusterSizeMin;
    private short _clusterSizeMax;
    private string _raceSizePercentage;
    private short _raceSizeDefault;
    private short _clusterDefault;
    private short _clusterSizeDefault;

    private short _mapSize;

    public RaceNames RaceId {
        get => _raceId;
        set {
            if (_raceId != value) {
                _raceId = value;
                NotifyPropertyChanged();
            }
        }
    }

    public bool IsChecked {
        get => _isChecked;
        set {
            if (_isChecked != value) {
                _isChecked = value;
                NotifyPropertyChanged();

                OnIsCheckedChanged();
            }
        }
    }

    public Color Color {
        get => _color;
        set {
            if (_color != value) {
                _color = value;
                NotifyPropertyChanged();
            }
        }
    }

    public string Name {
        get => _name;
        set {
            if (_name != value) {
                _name = value;
                NotifyPropertyChanged();
            }
        }
    }

    public short RaceSize {
        get => _raceSize;
        set {
            if (_raceSize != value) {
                if (value > _raceSizeMax) {
                    _raceSize = _raceSizeMax;
                } else if (value < _raceSizeMin) {
                    _raceSize = _raceSizeMin;
                } else {
                    _raceSize = value;
                }
                NotifyPropertyChanged();

                OnRaceSizeChanged();
            }
        }
    }

    public short ClusterCount {
        get => _cluster;
        set {
            if (_cluster != value) {
                if (value > _clusterMax) {
                    _cluster = _clusterMax;
                } else if (value < _clusterMin) {
                    _cluster = _clusterMin;
                } else {
                    _cluster = value;
                }
                NotifyPropertyChanged();

                OnClusterChanged();
            }
        }
    }

    public short ClusterSize {
        get => _clusterSize;
        set {
            if (_clusterSize != value) {
                if (value > _clusterSizeMax) {
                    _clusterSize = _clusterSizeMax;
                } else if (value < _clusterSizeMin) {
                    _clusterSize = _clusterSizeMin;
                } else {
                    _clusterSize = value;
                }
                NotifyPropertyChanged();
            }
        }
    }

    // Only UI relevant
    public short RaceSizeMin {
        get => _raceSizeMin;
        set {
            if (_raceSizeMin != value) {
                _raceSizeMin = value;
                NotifyPropertyChanged();
            }
        }
    }
    public short RaceSizeMax {
        get => _raceSizeMax;
        set {
            if (_raceSizeMax != value) {
                _raceSizeMax = value;
                NotifyPropertyChanged();
            }
        }
    }

    public short ClusterMin {
        get => _clusterMin;
        set {
            if (_clusterMin != value) {
                _clusterMin = value;
                NotifyPropertyChanged();

                OnClusterMinChanged();
            }
        }
    }

    public short ClusterMax {
        get => _clusterMax;
        set {
            if (_clusterMax != value) {
                _clusterMax = value;
                NotifyPropertyChanged();

                OnClusterMaxChanged();
            }
        }
    }

    public short ClusterSizeMin {
        get => _clusterSizeMin;
        set {
            if (_clusterSizeMin != value) {
                _clusterSizeMin = value;
                NotifyPropertyChanged();

                OnClusterSizeMinChanged();
            }
        }
    }

    public short ClusterSizeMax {
        get => _clusterSizeMax;
        set {
            if (_clusterSizeMax != value) {
                _clusterSizeMax = value;
                NotifyPropertyChanged();

                OnClusterSizeMaxChanged();
            }
        }
    }

    public string RaceSizePercentage {
        get => _raceSizePercentage;
        set {
            if (_raceSizePercentage != value) {
                _raceSizePercentage = value;
                NotifyPropertyChanged();
            }
        }
    }

    public short MapSize {
        get => _mapSize;
        set {
            if (_mapSize != value) {
                _mapSize = value;

                OnMapSizeChanged();
            }
        }
    }

    public RaceSettingsModel(RaceNames raceId, Color color, short raceSize, short cluster, short clusterSize) {
        _raceId = raceId;
        _isChecked = true;
        _color = color;
        _name = Enum.GetName(typeof(RaceNames), _raceId);
        _raceSize = raceSize;
        _cluster = cluster;
        _clusterSize = clusterSize;
        _mapSize = UniverseSettingsViewModel.MapSize;

        _raceSizeMin = 0;
        _raceSizeMax = _mapSize;
        _clusterMin = 1;
        _clusterMax = raceSize;
        _clusterSizeMin = CalculateClusterSizeMin();
        _clusterSizeMax = CalculateClusterSizeMax();
        _raceSizePercentage = CalculatePercentageRaceSize();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private short CalculateClusterSizeMin() {
        short value = (short)Math.Ceiling((float)_raceSize / _cluster);
        return (short)(value < 1 ? 0 : value);
    }

    private short CalculateClusterSizeMax() {
        if (_raceSize == 0) {
            return 0;
        }

        short value = (short)(_raceSize - (_cluster - 1));
        return (short)(value < 1 ? 0 : value);
    }

    private string CalculatePercentageRaceSize() {
        double value = (float)_raceSize / _mapSize;
        return value.ToString("0.00%");
    }

    private void OnIsCheckedChanged() {
        if (!_isChecked) {
            _raceSizeDefault = RaceSize;
            _clusterDefault = ClusterCount;
            _clusterSizeDefault = ClusterSize;
            RaceSize = 0;
            ClusterCount = 0;
            ClusterSize = 0;
        } else {
            ClusterSize = _clusterSizeDefault;
            ClusterCount = _clusterDefault;
            RaceSize = _raceSizeDefault;
        }
    }

    private void OnRaceSizeChanged() {
        if (_cluster > 0 && _raceSize == 0) {
            ClusterMin = 0;
        } else if (_cluster == 0 && _raceSize > 0) {
            ClusterMin = 1;
        }

        ClusterMax = _raceSize;

        // Only UI relevant
        ClusterSizeMin = CalculateClusterSizeMin();
        ClusterSizeMax = CalculateClusterSizeMax();

        RaceSizePercentage = CalculatePercentageRaceSize();
    }

    private void OnClusterChanged() {
        // Only UI relevant
        ClusterSizeMin = CalculateClusterSizeMin();
        ClusterSizeMax = CalculateClusterSizeMax();
    }

    private void OnClusterMinChanged() {
        if (_cluster < _clusterMin) {
            _cluster = _clusterMin;
            NotifyPropertyChanged(nameof(ClusterCount));
        }
    }

    private void OnClusterMaxChanged() {
        if (_cluster > _clusterMax) {
            _cluster = _clusterMax;
            NotifyPropertyChanged(nameof(ClusterCount));
        }
    }

    private void OnClusterSizeMinChanged() {
        if (_clusterSize < _clusterSizeMin) {
            _clusterSize = _clusterSizeMin;
            NotifyPropertyChanged(nameof(ClusterSize));
        }
    }

    private void OnClusterSizeMaxChanged() {
        if (_clusterSize > _clusterSizeMax) {
            _clusterSize = _clusterSizeMax;
            NotifyPropertyChanged(nameof(ClusterSize));
        }
    }

    private void OnMapSizeChanged() {
        RaceSizeMax = _mapSize;
        /*
        double temp = Convert.ToDouble(_raceSizePercentage.Remove(_raceSizePercentage.Length - 1));
        temp = Math.Ceiling(temp / 100 * _mapSize);
        RaceSize = (int)temp;
        */
        RaceSizePercentage = CalculatePercentageRaceSize();
    }
}