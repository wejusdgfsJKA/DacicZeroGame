using KBCore.Refs;
using Pooling;
using UnityEngine;
namespace Entity
{
    public class EntityBase : ValidatedMonoBehaviour, IPoolable<EntityID>
    {
        #region Fields
        [SerializeField, Self] protected HPComponent hpComponent;
        protected EntityID type;
        /// <summary>
        /// Set the entity's parameters.
        /// </summary>
        public EntityData Data
        {
            set
            {
                type = value.Type;
                hpComponent.MaxHealth = value.MaxHealth;
            }
        }

        public EntityID ID
        {
            get
            {
                return type;
            }
        }
        #endregion
        protected void OnEnable()
        {
            //register entity
            if (EntityManager.Instance != null)
            {
                if (!EntityManager.Instance.Register(this))
                {
                    Debug.LogError($"Transform {transform} unable to register.");
                }
            }
            else
            {
                Debug.LogError("No EntityManager instance found!");
            }
        }
        protected void OnDisable()
        {
            if (EntityManager.Instance != null)
            {
                EntityManager.Instance.DeRegister(this);
            }
            else
            {
                Debug.LogError("No EntityManager instance found!");
            }
        }
    }
}