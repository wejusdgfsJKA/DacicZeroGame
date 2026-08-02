using UnityEngine;
using System.Collections.Generic;

namespace DacicZero.Data.Dialog {
    [CreateAssetMenu(fileName = "New Dialog Sequence", menuName = "DacicZero/Dialog Sequence")]
    public class DialogSequenceSO : ScriptableObject {
        
        [Tooltip("sequence of dialog nodes")]
        [SerializeField] private List<DialogNode> _nodes = new List<DialogNode>(); 
        
        public IReadOnlyList<DialogNode> Nodes => _nodes; 
    }
}