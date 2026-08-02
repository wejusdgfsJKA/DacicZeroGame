using UnityEngine;
using System.Collections.Generic;

namespace DacicZero.Data.Dialog 
{
    /// <summary> defines which side is currently speaking </summary>
    public enum SpeakerSide { Left, Right, Both, None }

    /// <summary> single dialog step </summary>
    [System.Serializable]
    public class DialogNode 
    {
        [Tooltip("speaking side used to dim the non-speaking portrait")]
        [SerializeField] private SpeakerSide _activeSpeaker = SpeakerSide.Left;

        [Tooltip("ID for the left character ('NPC_TechGuy')")]
        [SerializeField] private string _leftID = "NPC_TechGuy";

        [Tooltip("ID for the right character ('Player')")]
        [SerializeField] private string _rightID = "Player";

        [Tooltip("changes the left side portrait for a dialog line")]
        [SerializeField] private Sprite _leftSprite;

        [Tooltip("changes the right side portrait for a dialog line")]
        [SerializeField] private Sprite _rightSprite;
        
        [Tooltip("displayed text")]
        [TextArea(3, 5)]
        [SerializeField] private string _dialogText;

        [Tooltip("response options")]
        [SerializeField] private List<DialogOption> _options = new List<DialogOption>();

        public SpeakerSide ActiveSpeaker => _activeSpeaker;
        public string LeftID => _leftID;
        public string RightID => _rightID;
        public Sprite LeftSprite => _leftSprite;
        public Sprite RightSprite => _rightSprite;
        public string DialogText => _dialogText;
        public IReadOnlyList<DialogOption> Options => _options;
    }
}