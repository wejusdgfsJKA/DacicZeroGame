using System;
using UnityEngine;

namespace DacicZero.Data.Dialog {
    /// <summary> selectable dialog response </summary>
    [Serializable]
    public class DialogOption {
        [Tooltip("text displayed on the option button")]
        [SerializeField] private string _optionText;

        [Tooltip("event id triggered on selection ('OpenMap', 'OpenUpgradeMenu')")]
        [SerializeField] private string _actionEventID;

        [Tooltip("optional parameters passed with the event")]
        [SerializeField] private string _actionParams;

        [Tooltip("target node index. -1 continues linearly or ends")]
        [SerializeField] private int _jumpToNodeIndex = -1;

        public string OptionText => _optionText;
        public string ActionEventID => _actionEventID;
        public string ActionParams => _actionParams;
        public int JumpToNodeIndex => _jumpToNodeIndex;
    }
}