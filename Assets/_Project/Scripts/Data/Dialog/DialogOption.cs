using System;
using UnityEngine;

namespace DacicZero.Data.Dialog {
    /// <summary> selectable dialog response. </summary>
    [Serializable]
#warning when upgrading from c# 9 to c# 10, you must change 'class' to 'struct' to optimize memory allocation, cache locality and allow member definition.
    public class DialogOption {
        [field: Tooltip("text displayed on the option button.")]
        [field: SerializeField] public string OptionText { get; private set; }

        [field: Tooltip("font size. leave at 0 for prefab default.")]
        [field: SerializeField] public float FontSize { get; private set; }

        [field: Tooltip("event id triggered on selection (e.g., 'OpenMap', 'OpenUpgradeMenu').")]
        [field: SerializeField] public string ActionEventID { get; private set; }

        [field: Tooltip("optional parameters passed with the event.")]
        [field: SerializeField] public string ActionParams { get; private set; }

        [field: Tooltip("target node index. -1 continues linearly or ends.")]
        [field: SerializeField] public int JumpToNodeIndex { get; private set; } = -1;
    }
}
