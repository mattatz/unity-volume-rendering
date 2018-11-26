using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

namespace VolumeRendering.Utils
{

    public class TransformController : MonoBehaviour {

        static readonly string kMouseX = "Mouse X";
        static readonly string kMouseY = "Mouse Y";
        static readonly string kMouseScroll = "Mouse ScrollWheel";

        [SerializeField, Range(1f, 10f)] protected float zoomSpeed = 7.5f, zoomDelta = 5f;
        [SerializeField, Range(1f, 15f)] protected float zoomMin = 5f, zoomMax = 15f;

        [SerializeField, Range(1f, 10f)] protected float rotateSpeed = 7.5f, rotateDelta = 5f;

        protected Camera cam;
        protected Vector3 targetCamPosition;
        protected Quaternion targetRotation;

        protected void Start () {
            cam = Camera.main;
            targetCamPosition = cam.transform.position;
            targetRotation = transform.rotation;
        }
        
        protected void Update () {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            var dt = Time.deltaTime;
            Zoom(dt);
            Rotate(dt);
        }

        protected void Zoom(float dt)
        {
            var amount = Input.GetAxis("Mouse ScrollWheel");
            if(Mathf.Abs(amount) > 0f)
            {
                targetCamPosition += cam.transform.forward * zoomSpeed * amount;
                targetCamPosition = targetCamPosition.normalized * Mathf.Clamp(targetCamPosition.magnitude, zoomMin, zoomMax);
            }
            cam.transform.position = Vector3.Lerp(cam.transform.position, targetCamPosition, dt * zoomDelta);
        }

        protected void Rotate(float dt)
        {
            if (Input.GetMouseButton(0))
            {
                var mouseX = Input.GetAxis(kMouseX) * rotateSpeed;
                var mouseY = Input.GetAxis(kMouseY) * rotateSpeed;

                var up = transform.InverseTransformDirection(cam.transform.up);
                targetRotation *= Quaternion.AngleAxis(-mouseX, up);

                var right = transform.InverseTransformDirection(cam.transform.right);
                targetRotation *= Quaternion.AngleAxis(mouseY, right);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, dt * rotateDelta);
        }

    }

}


