using UnityEngine;
using System.Collections.Generic;

namespace DacicZero.Data.Dialog {
    /// <summary> scriptableobject storing a complete dialog sequence. </summary>
    [CreateAssetMenu(fileName = "New Dialog Sequence", menuName = "DacicZero/Dialog Sequence")]
    public class DialogSequenceSO : ScriptableObject {
        [Tooltip("default npc sprite for this conversation. leave empty to fall back to character sprite in scene.")]
        [UnityEngine.Serialization.FormerlySerializedAs("_npcSprite")]
        [SerializeField] private Sprite _defaultSprite;

        [Tooltip("sequence of dialog nodes.")]
        [SerializeField] private List<DialogNode> _nodes;

        public Sprite DefaultSprite => _defaultSprite;
        public IReadOnlyList<DialogNode> Nodes => (IReadOnlyList<DialogNode>)_nodes ?? System.Array.Empty<DialogNode>();
    }
}
