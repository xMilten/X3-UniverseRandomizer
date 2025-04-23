using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using X3UR.ViewModels;
using X3UR.UserControls;

namespace X3UR.UserControls;
/// <summary>
/// Interaktionslogik für BaseUserControl.xaml
/// </summary>
public partial class BaseUserControl : UserControl {
    public BaseUserControl() {
        InitializeComponent();
        scrollViewUniverse.Content = new UniverseSettingsUserControl();
        scrollViewDebug.Content = new DebugModeUserControl();
    }

    public void RadomizeSeed_Click(object sender, RoutedEventArgs e) {
        BaseViewModel.RadomizeSeed();
    }

    public void ChangeVisibility_Click(object sender, RoutedEventArgs e) {
        BaseViewModel.ChangeVisibility();
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Enter) {
            var simulateInput = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab) { RoutedEvent = Keyboard.KeyDownEvent };
            InputManager.Current.ProcessInput(simulateInput);
        }
    }
}