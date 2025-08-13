using UnityEngine;

namespace Entity
{
    /// <summary>
    /// All relevant data regarding an entity.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/EntityData")]
    [System.Serializable]
    public class EntityData : ObjectData<EntityBase>
    {
        /// <summary>
        /// Internal ID.
        /// </summary>
        [field: SerializeField]
        public EntityID Type { get; protected set; }
        /// <summary>
        /// Maximum health.
        /// </summary>
        [field: SerializeField]
        public int MaxHealth { get; protected set; }
    }
}