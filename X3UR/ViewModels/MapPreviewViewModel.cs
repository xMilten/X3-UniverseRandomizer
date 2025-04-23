using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using X3UR.Models;
using X3UR.Objectives;
using X3UR.UserControls;

namespace X3UR.ViewModels;

public static class MapPreviewViewModel {
    private static byte _imageSize = 35;
    private static byte _margin = 50;

    private static ObservableCollection<ObservableCollection<SectorBase>> _map = new();

    public static MapPreviewUserControl MapPreviewControl { get; set; }
    public static byte ImageSize { 
        get { return _imageSize; }
        set {
            if (_imageSize != value) {
                _imageSize = value;
                NotifyStaticPropertyChanged();
            }
        }
    }
    public static byte Margin {
        get { return _margin; }
        set {
            if (_margin != value) {
                _margin = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static ObservableCollection<ObservableCollection<SectorBase>> Map {
        get { return _map; }
        set {
            if (_map != value) {
                _map = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    private static void NotifyStaticPropertyChanged([CallerMemberName] string propertyName = "") {
        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }

    public static void ClearMap() {
        Application.Current.Dispatcher.Invoke(() => {
            Map.Clear();
        });
    }

    public static void AddMapList(ObservableCollection<SectorBase> sectors) {
        Application.Current.Dispatcher.Invoke(() => {
            Map.Add(sectors);
        });
    }

    public static void AddSector(Sector sector) {
        Application.Current.Dispatcher.Invoke(() => {
            Map[sector.PosY][sector.PosX] = sector;
        });
    }

    public static void AddSectorBase(SectorBase sectorBase) {
        Application.Current.Dispatcher.Invoke(() => {
            Map[sectorBase.PosY][sectorBase.PosX] = new SectorBase(sectorBase.PosX, sectorBase.PosY) {
                Coords = string.Empty,
                Visibility = Visibility.Visible,
                Color = sectorBase.Color,
                SectorsCanClaimMe = sectorBase.SectorsCanClaimMe
            };
        });
    }

    public static void AddRemovedSectorBase(SectorBase sectorBase, Color color) {
        Application.Current.Dispatcher.Invoke(() => {
            Map[sectorBase.PosY][sectorBase.PosX] = new SectorBase(sectorBase.PosX, sectorBase.PosY) {
                Coords = string.Empty,
                Visibility = Visibility.Visible,
                Color = color
            };
        });
    }
}

public class RaceColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is Sector sector) {
            return sector.Race.Color;
        } else if (value is SectorBase sectorBase) {
            return sectorBase.Color;
        }
            return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

public class RaceNameConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is Sector sector) {
            return sector.Race.Name;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}