using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using X3UR.Objectives;
using X3UR.ViewModels.DebugModeModels;

namespace X3UR.ViewModels.DebugModeViewModels {
    public static class DebugModeRaceInfosViewModel {
        private static DebugModeRaceInfos _debugModeRaceInfos;

        public static DebugModeRaceInfos DebugModeRaceInfos {
            get => _debugModeRaceInfos;
            set => SetProperty(ref _debugModeRaceInfos, value);
        }

        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static void NotifyStaticPropertyChanged([CallerMemberName] string propertyName = "") {
            StaticPropertyChanged.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        private static void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "") {
            if (!EqualityComparer<T>.Default.Equals(field, value)) {
                field = value;
                NotifyStaticPropertyChanged(propertyName);
            }
        }

        public static void SetRaceInfos(Sector sector) {
            DebugModeRaceInfos = new DebugModeRaceInfos(sector);
        }

        public static void SetRaceInfos(SectorBase sectorBase) {
            DebugModeRaceInfos = new DebugModeRaceInfos(sectorBase);
        }
    }
}