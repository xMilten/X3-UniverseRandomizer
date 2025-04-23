using System.Collections;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using X3UR.Generator;
using X3UR.UserControls;

namespace X3UR;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    private readonly GrowingUniverseGenerator _universeGenerator;

    public MainWindow() {
        InitializeComponent();
        contentBaseSettings.Content = new BaseUserControl();
        contentMapPreview.Content = new MapPreviewUserControl();
        _universeGenerator = new GrowingUniverseGenerator();
    }

    private void OnKeyDown(object sender, KeyEventArgs e) {
        switch (e.Key) {
            case Key.Space:
                //_universeGenerator.Stepwise();
                //_universeGenerator.Grow();
                break;
        }
    }
}