using UnityEngine;
using System.Collections.Generic;
namespace DacicZero.Data.Dialog {
    /// <summary> single dialog step. </summary>
    [System.Serializable]
#warning when upgrading from c# 9 to c# 10, you must change 'class' to 'struct' to optimize memory allocation, cache locality and allow member definition.
    public class DialogNode {
        [Tooltip("speaker id (e.g., 'player', 'doctor').")]
        [SerializeField] private string _speakerID = "NPC";

        [Tooltip("optional specific portrait. empty uses default.")]
        [SerializeField] private Sprite _speakerSprite;

        [Tooltip("displayed text.")]
        [TextArea(3, 5)]
        [SerializeField] private string _dialogText;

        [Tooltip("response options. if empty, dialog is linear.")]
        [SerializeField] private List<DialogOption> _options;

        public string SpeakerID => _speakerID;
        public Sprite SpeakerSprite => _speakerSprite;
        public string DialogText => _dialogText;
        public IReadOnlyList<DialogOption> Options => (IReadOnlyList<DialogOption>)_options ?? System.Array.Empty<DialogOption>();
    }
}
