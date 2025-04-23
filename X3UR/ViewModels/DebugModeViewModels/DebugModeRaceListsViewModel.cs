using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using X3UR.Generator;

namespace X3UR.ViewModels.DebugModeViewModels {
    public static class DebugModeRaceListsViewModel {
        public static ObservableCollection<UiRaceListItem> FirstList { get; set; } = new();
        public static ObservableCollection<UiRaceListItem> SecondList { get; set; } = new();
        public static ObservableCollection<UiRaceListItem> ThirdList { get; set; } = new();

        public static void AddToFirstList(List<List<byte>> raceIndicesWithClusters) {
            Application.Current.Dispatcher.Invoke(() => {
                for (byte i = 0; i < raceIndicesWithClusters.Count; i++) {
                    if (raceIndicesWithClusters[i].Count == 0) continue;

                    FirstList.Add(new UiRaceListItem(
                        i,
                        UniverseSettingsViewModel.RaceSettingsModels[raceIndicesWithClusters[i][0]].Name,
                        (short)raceIndicesWithClusters[i].Count,
                        UniverseSettingsViewModel.RaceSettingsModels[raceIndicesWithClusters[i][0]].Color
                    ));
                }
            });
        }

        public static void AddToSecondList(List<byte> raceIndices, List<List<byte>> raceIndicesWithClusters) {
            Application.Current.Dispatcher.Invoke(() => {
                for (byte i = 0; i < raceIndices.Count; i++) {
                    SecondList.Add(new UiRaceListItem(
                        i,
                        UniverseSettingsViewModel.RaceSettingsModels[raceIndicesWithClusters[raceIndices[i]][0]].Name,
                        (short)raceIndices.Count,
                        UniverseSettingsViewModel.RaceSettingsModels[raceIndicesWithClusters[raceIndices[i]][0]].Color
                    ));
                }
            });
        }

        public static void AddToFirstList2(List<List<byte>> raceIndicesWithClusters) {
            Application.Current.Dispatcher.Invoke(() => {
                for (byte i = 0; i < raceIndicesWithClusters.Count; i++) {
                    if (raceIndicesWithClusters[i].Count == 0) continue;

                    FirstList.Add(new UiRaceListItem(
                        i,
                        GrowingUniverseGenerator.Universe.Clusters[raceIndicesWithClusters[i][0]].Race.Name,
                        (short)raceIndicesWithClusters[i].Count,
                        GrowingUniverseGenerator.Universe.Clusters[raceIndicesWithClusters[i][0]].Race.Color
                    ));
                }
            });
        }

        public static void AddToSecondList2(List<byte> raceIndices, List<List<byte>> raceIndicesWithClusters) {
            Application.Current.Dispatcher.Invoke(() => {
                for (byte i = 0; i < raceIndices.Count; i++) {
                    SecondList.Add(new UiRaceListItem(
                        i,
                        GrowingUniverseGenerator.Universe.Clusters[raceIndicesWithClusters[raceIndices[i]][0]].Race.Name,
                        (short)raceIndices.Count,
                        GrowingUniverseGenerator.Universe.Clusters[raceIndicesWithClusters[raceIndices[i]][0]].Race.Color
                    ));
                }
            });
        }

        public static void ClearFirstList() {
            Application.Current.Dispatcher.Invoke(() => {
                FirstList.Clear();
            });
        }

        public static void ClearSecondList() {
            Application.Current.Dispatcher.Invoke(() => {
                SecondList.Clear();
                ThirdList.Clear();
            });
        }

        public static void ClearAllLists() {
            ClearFirstList();
            ClearSecondList();
        }

        public static void RemoveFromFirstList(byte raceIndicesWithClustersIndex) {
            Application.Current.Dispatcher.Invoke(() => {
                FirstList.RemoveAt(raceIndicesWithClustersIndex);
            });
        }
        public static void RemoveFromSecondList(byte randomRaceIndex) {
            Application.Current.Dispatcher.Invoke(() => {
                SecondList.RemoveAt(randomRaceIndex);
            });
        }

        public static void MoveFromSecondToThirdList(byte randomRaceIndex) {
            Application.Current.Dispatcher.Invoke(() => {
                ThirdList.Add(SecondList[randomRaceIndex]);
                SecondList.RemoveAt(randomRaceIndex);
            });
        }

        public static void ReduceFromFirstList(byte raceIndicesWithClustersIndex) {
            UiRaceListItem uiRaceListItem = FirstList[raceIndicesWithClustersIndex];
            //MessageBox.Show($"{uiRaceListItem.Name}, {uiRaceListItem.Count}");
            Application.Current.Dispatcher.Invoke(() => {
                uiRaceListItem.Count--;
            });
        }

        public class UiRaceListItem : INotifyPropertyChanged {
            private short _count;

            public byte Index { get; set; }
            public string Name { get; set; }
            public short Count {
                get => _count;
                set {
                    if (_count != value) {
                        _count = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            public SolidColorBrush Color { get; set; }

            public UiRaceListItem(byte index, string name, short count, Color color) {
                Index = index;
                Name = name;
                Count = count;
                Color = new SolidColorBrush(color);
            }

            public UiRaceListItem(string name, Color color) {
                Name = name;
                Color = new SolidColorBrush(color);
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}