using System.Collections.Generic;
using UnityEngine;

namespace DacicZero.NPC {
    /// <summary> scene manager for global npc state and visibility. </summary>
    public class NPCDirector : MonoBehaviour {
        [Header("Managed NPCs")]
        [Tooltip("Drag all NPC GameObjects here to manage their visibility globally.")]
        [SerializeField] private List<NPCController> _npcsInScene;

        public IReadOnlyList<NPCController> NpcsInScene => (IReadOnlyList<NPCController>)_npcsInScene ?? System.Array.Empty<NPCController>();

        private void Start() => RefreshNPCVisibility();

        public void RefreshNPCVisibility() {
            if (_npcsInScene == null) return;

            foreach (var npc in _npcsInScene)
                if (npc != null) npc.gameObject.SetActive(true);
        }
    }
}
