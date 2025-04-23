using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using System.Windows.Controls;
using X3UR.Objectives;
using System.Runtime.CompilerServices;

namespace X3UR.ViewModels;
public static class DebugModeViewModel {
    private static Cluster _pickedCluster;
    private static Sector _claimingSector;
    private static TreeViewItem _tviCurrentRaceName;
    private static ObservableCollection<TreeViewItem> _items = new();

    public static ObservableCollection<TreeViewItem> Items {
        get {
            return _items;
        }
        set {
            if (_items != value) {
                _items = value;
                NotifyStaticPropertyChanged(nameof(Items));
            }
        }
    }

    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    private static void NotifyStaticPropertyChanged([CallerMemberName] string propertyName = "") {
        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }

    public static void AddTVIToDebugModeTab(Cluster pickedCluster, Sector claimingSector) {
        _pickedCluster = pickedCluster;
        _claimingSector = claimingSector;

        if (_tviCurrentRaceName != null) {
            if (_tviCurrentRaceName.Name == _claimingSector.Race.Name) {
                FindTVICluster();
                return;
            }
            foreach (TreeViewItem twiRaceName in Items) {
                if (twiRaceName.Name == _claimingSector.Race.Name) {
                    //_tviCurrentRaceName.IsExpanded = false;
                    _tviCurrentRaceName = twiRaceName;
                    _tviCurrentRaceName.IsExpanded = true;
                    FindTVICluster();
                    return;
                }
            }
        }
        CreateTVIRaceName();
    }

    private static void FindTVICluster() {
        TreeViewItem tempTviCluster = null;

        foreach (TreeViewItem tviCluster in _tviCurrentRaceName.Items) {
            if (tviCluster.Name == $"{_claimingSector.Race.Name}_{_pickedCluster.Id}") {
                tempTviCluster = tviCluster;
            }
        }
        tempTviCluster ??= GetTVICluster();
        CreateTVISector(tempTviCluster);
    }

    private static void CreateTVISector(TreeViewItem tviCluster) {
        tviCluster.Items.Add(new TreeViewItem() {
            Name = _claimingSector.Race.Name + _claimingSector.PosX.ToString("00") + _claimingSector.PosY.ToString("00"),
            Header = $"{_claimingSector.PosX}  {_claimingSector.PosY}"
        });
        tviCluster.IsExpanded = true;
    }

    private static void CreateTVIRaceName() {
        string raceName = _claimingSector.Race.Name;

        TreeViewItem tviRaceName = new() {
            Name = raceName,
            Header = raceName
        };

        //if (_tviCurrentRaceName != null)
        //    _tviCurrentRaceName.IsExpanded = false;

        _tviCurrentRaceName = tviRaceName;
        _tviCurrentRaceName.IsExpanded = true;
        Items.Add(_tviCurrentRaceName);

        TreeViewItem twiCluster = GetTVICluster();

        CreateTVISector(twiCluster);

    }

    private static TreeViewItem GetTVICluster() {
        TreeViewItem tviCluster = new() {
            Name = $"{_claimingSector.Race.Name}_{_pickedCluster.Id}",
            Header = $"{_pickedCluster.Id} - {_pickedCluster.PosX}  {_pickedCluster.PosY}"
        };
        _tviCurrentRaceName.Items.Add(tviCluster);
        return tviCluster;
    }
}