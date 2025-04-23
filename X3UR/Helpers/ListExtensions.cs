using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X3UR.Helpers;
public static class ListExtensions {
    public static void SwapAndRemoveAt<T>(this List<T> list, int index) {
        if (index < 0 || index >= list.Count) {
            throw new ArgumentOutOfRangeException(nameof(index), "Index liegt außerhalb des gültigen Bereichs.");
        }

        int lastIndex = list.Count - 1;
        if (index != lastIndex) {
            list[index] = list[lastIndex]; // Tausche mit letztem Element
        }
        list.RemoveAt(lastIndex); // Entferne letztes Element
    }
}
