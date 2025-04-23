using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using X3UR.Helpers;

namespace X3UR.ViewModels;
public static class BaseViewModel {
    private static long _seed = MathHelpers.RandomLong(1000000000, 10000000000);
    private static Visibility _visibility;

    public static long Seed {
        get => _seed;
        set {
            if (_seed != value) {
                _seed = value;
                NotifyStaticPropertyChanged();

            }
        }
    }

    public static Visibility Visibility {
        get => _visibility;
        set {
            if (_visibility != value) {
                _visibility = value;
                NotifyStaticPropertyChanged();
            }
        }
    }

    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    private static void NotifyStaticPropertyChanged([CallerMemberName] string propertyName = "") {
        StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(propertyName));
    }

    public static void RadomizeSeed() {
        Seed = MathHelpers.RandomLong(1000000000, 10000000000);
    }

    public static void ChangeVisibility() {
        if (Visibility == Visibility.Visible) {
            Visibility = Visibility.Hidden;
        } else if (Visibility == Visibility.Hidden) {
            Visibility = Visibility.Visible;
        }
    }
}