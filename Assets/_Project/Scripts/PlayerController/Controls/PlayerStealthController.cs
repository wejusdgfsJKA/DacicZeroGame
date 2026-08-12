using EventBus;
using HP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
namespace PlayerController
{
    public class PlayerStealthController : MonoBehaviour
    {
        [SerializeField] public bool IsEnabled;
        [SerializeField] PlayerMovementController MovementController;
        [SerializeField] CameraController CameraController;
        [SerializeField] Collider PlayerCollider;



        public void Awake()
        {
            if (IsEnabled) MovementController.EnableFancyMovement = false;
        }
        public IEnumerator GetSpottedBy(Transform spotter)
        {
            var PlayerBody = transform.root.GetComponent<Rigidbody>();
            CameraController.cameraSpeed = 0f;
            var dir = -spotter.forward;
            CameraController.SetCameraRotation(Quaternion.LookRotation(dir));
            MovementController.enabled = false;
            PlayerBody.linearVelocity = Vector3.zero;
            yield return new WaitForSeconds(1);
            EventBus<TakeDamage>.Raise(transform.root.GetInstanceID(), new TakeDamage(77777, spotter.root, PlayerCollider));
        }
    }
}