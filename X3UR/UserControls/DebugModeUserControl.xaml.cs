using System.Windows.Controls;
using X3UR.ViewModels.DebugModeViewModels;

namespace X3UR.UserControls;
/// <summary>
/// Interaktionslogik für BebugModeUserControl.xaml
/// </summary>
public partial class DebugModeUserControl : UserControl {
    public DebugModeUserControl() {
        InitializeComponent();
        expanderRaceInfos.DataContext = typeof(DebugModeRaceInfosViewModel);
    }
}