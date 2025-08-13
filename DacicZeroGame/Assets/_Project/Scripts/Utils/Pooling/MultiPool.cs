using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public class MultiPool<Id, T> where T : IPoolable<Id>
    {
        protected Dictionary<Id, Stack<T>> pools = new();
        /// <summary>
        /// Get a new object from the pool identified by the id.
        /// </summary>
        /// <param name="id">The id of the object.</param>
        /// <returns>The object if found, or null if the pool was empty/not found.</returns>
        public T Get(Id id)
        {
            Stack<T> pool;
            if (pools.TryGetValue(id, out pool))
            {
                if (pool.Count > 0)
                {
                    return pool.Pop();
                }
            }
            return default;
        }
        /// <summary>
        /// Release an object into its coresponding pool. Will create a new pool if none is found.
        /// </summary>
        /// <param name="entity">The object to be released.</param>
        public void Release(T entity)
        {
            Stack<T> pool;
            if (!pools.TryGetValue(entity.ID, out pool))
            {
                pool = new();
                pools.Add(entity.ID, pool);
            }
            pool.Push(entity);
        }
        /// <summary>
        /// Clears and deletes underlying pools.
        /// </summary>
        public void Clear()
        {
            Debug.Log($"Clearing multipool of {typeof(Id)}.");
            foreach (var pool in pools.Values)
            {
                pool.Clear();
            }
            pools.Clear();
        }
    }
}