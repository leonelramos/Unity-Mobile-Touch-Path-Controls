using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchControls
{
    public class TouchPathManager : MonoBehaviour
    {
        public TouchPath Path { get; set; }
        public Camera referenceCamera;
        public float speed;
        public float smoothness;
        public float rotationSpeed;

        void Awake()
        {
            Path = new TouchPath(transform, referenceCamera, speed, smoothness, rotationSpeed);
        }

        void Update()
        {
            Path.UpdateConfiguration(speed, smoothness, rotationSpeed);
            Path.PathPositionUpdate();
        }
    }
}

