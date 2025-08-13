using Pooling;
using System.Collections.Generic;
using UnityEngine;
namespace Entity
{
    /// <summary>
    /// Manages entity pooling, spawning, damage and death.
    /// </summary>
    public class EntityManager : MonoBehaviour
    {
        public static EntityManager Instance { get; private set; }
        /// <summary>
        /// Active entities, identified by their root InstanceID.
        /// </summary>
        public Dictionary<int, EntityBase> Entities { get; } = new();
        /// <summary>
        /// All entity pools.
        /// </summary>
        protected MultiPool<EntityID, EntityBase> multiPool = new();
        /// <summary>
        /// All entities the manager can spawn.
        /// </summary>
        protected Dictionary<EntityID, EntityData> roster { get; } = new();
        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        /// <summary>
        /// Register an existing entity. This should only be called in OnEnable in EntityBase.
        /// </summary>
        /// <param name="entity">The entity to be registered.</param>
        /// <returns>True if registration successfull.</returns>
        public bool Register(EntityBase entity)
        {
            return Entities.TryAdd(entity.transform.GetInstanceID(), entity);
        }
        /// <summary>
        /// Spawn a new enemy, or get one from the entity pool. The spawned enemy will be inactive.
        /// </summary>
        /// <param name="id">The id of the enemy (it's id, not its unique identifier).</param>
        /// <param name="position">Spawn position.</param>
        /// <returns>An instance of the enemy if existing pool/roster entry was found, null otherwise</returns>
        public EntityBase Spawn(EntityID id, Vector3 position)
        {
            return Spawn(id, position, Quaternion.identity);
        }
        /// <summary>
        /// Spawn a new enemy, or get one from the entity pool. The spawned enemy will be inactive.
        /// </summary>
        /// <param name="id">The id of the enemy (it's id, not its unique identifier).</param>
        /// <param name="position">Spawn position.</param>
        /// <param name="rotation">Spawn rotation.</param>
        /// <returns>An instance of the enemy if existing pool/roster entry was found, null otherwise</returns>
        public EntityBase Spawn(EntityID id, Vector3 position, Quaternion rotation)
        {
            EntityBase e = multiPool.Get(id);
            if (e == null)
            {
                EntityData data;
                if (roster.TryGetValue(id, out data))
                {
                    e = Instantiate(data.Prefab, position, rotation);
                    //we need to set the entity's data for the first time.
                    e.Data = data;
                }
                else
                {
                    Debug.LogError($"Id {id} not found in entity roster!");
                    return null;
                }
            }
            return e;
        }
        /// <summary>
        /// Add an entity to its coresponding pool, identified by its EntityID field.
        /// </summary>
        /// <param name="entity">The entity will be added to the pool.</param>
        protected void AddToPool(EntityBase entity)
        {
            multiPool.Release(entity);
        }
        /// <summary>
        /// Send an attack to an entity.
        /// </summary>
        /// <param name="entity">The root transform of the attacked entity.</param>
        /// <param name="dmgInfo">The damage package.</param>
        public void SendAttack(Transform entity, DmgInfo dmgInfo)
        {
            EntityBase b;
            if (Entities.TryGetValue(entity.GetInstanceID(), out b))
            {
                b.TakeDamage(dmgInfo);
            }
        }
        /// <summary>
        /// This entity just died, will be added to the pool.
        /// </summary>
        /// <param name="entity">The entity that died.</param>
        public void DeRegister(EntityBase entity)
        {
            Entities.Remove(entity.transform.GetInstanceID());
            AddToPool(entity);
        }
        private void OnDisable()
        {
            Entities.Clear();
            multiPool.Clear();
            roster.Clear();
        }
        private void OnDestroy()
        {
            OnDisable();
        }
        private void OnApplicationQuit()
        {
            OnDestroy();
        }
    }
}