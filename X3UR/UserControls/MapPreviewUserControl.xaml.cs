using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using X3UR.Objectives;
using X3UR.ViewModels;
using X3UR.ViewModels.DebugModeViewModels;

namespace X3UR.UserControls;
/// <summary>
/// Interaction logic for MapPreviewUserControl.xaml
/// </summary>
public partial class MapPreviewUserControl : UserControl {

    public MapPreviewUserControl() {
        InitializeComponent();
        MapPreviewViewModel.MapPreviewControl = this;
        Loaded += MeLoaded;
    }

    private void MeLoaded(object sender, RoutedEventArgs e) {
        double actualHeight = ActualHeight;
        MapPreviewViewModel.Margin = (byte)(actualHeight / UniverseSettingsViewModel.MapHeight);
        MapPreviewViewModel.ImageSize = (byte)(MapPreviewViewModel.Margin - 15);
    }

    private void Sector_MouseDown(object sender, MouseButtonEventArgs e) {
        if (sender is FrameworkElement element && element.DataContext is Sector sector) {
            DebugModeRaceInfosViewModel.SetRaceInfos(sector);
        } else if (sender is FrameworkElement frelement && frelement.DataContext is SectorBase sectorBase) {
            DebugModeRaceInfosViewModel.SetRaceInfos(sectorBase);
        }
    }
}
