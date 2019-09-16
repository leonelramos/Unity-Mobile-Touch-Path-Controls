using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchControls
{
    public class TouchPath
    {
        private Queue<MobileTouch> Touches { get; set; }
        private MobileTouch CurrentTouch { get; set; }
        private Transform Player { get; set; }
        private Camera ReferenceCamera { get; set; }
        private Camera ReferenceCameraSnapshot { get; set; }
        private GameObject ReferenceCameraContainer { get; set; }
        private float DistanceBetweenTouches { get; set; }
        private float MaxDistanceDelta { get; set; }
        private float RotationSpeed { get; set; }

        public TouchPath(Transform player, Camera referenceCamera, float maxDistanceDelta = 1.5f, float distanceBetweenTouches = .02f, float rotationSpeed = .65f)
        {
            Touches = new Queue<MobileTouch>();
            CurrentTouch = MobileTouch.InactiveTouch();
            Player = player;
            ReferenceCamera = referenceCamera;
            ReferenceCameraContainer = new GameObject();
            ReferenceCameraContainer.name = "Reference Camera Container";
            ReferenceCameraSnapshot = ReferenceCameraContainer.AddComponent<Camera>() as Camera;
            ReferenceCameraSnapshot.tag = "Untagged";
            ReferenceCameraSnapshot.depth = referenceCamera.depth - 1;
            DistanceBetweenTouches = distanceBetweenTouches;
            MaxDistanceDelta = maxDistanceDelta;
            RotationSpeed = rotationSpeed;
        }

        public void PathPositionUpdate()
        {
            UpdateTouches();
            if (CurrentTouch.IsActive)
            {
                MoveTorwardsCurrentTouch();
                if (ArrivedAtCurrentTouch())
                {
                    CurrentTouch.IsActive = false;
                    UpdateCurrentTouch();
                }
            }
        }

        private void MoveTorwardsCurrentTouch()
        {
            Vector3 targetPosition;
            if (CurrentTouch.HasTargetPositionSet)
            {
                targetPosition = CurrentTouch.TargetPosition;
            }
            else
            {
                CurrentTouch.TargetPosition = WorldPositionFromTouchPosition(CurrentTouch.Position);
                targetPosition = CurrentTouch.TargetPosition;
                CurrentTouch.HasTargetPositionSet = true;
            }
            targetPosition.y = Player.position.y;
            LookAtTarget(targetPosition);
            Player.position = Vector3.MoveTowards(Player.position, targetPosition, MaxDistanceDelta * Time.deltaTime);
        }

        private void UpdateTouches()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        Touches = new Queue<MobileTouch>();
                        CurrentTouch = new MobileTouch(touch, true);
                        ReferenceCameraSnapshot.CopyFrom(ReferenceCamera);
                        break;
                    case TouchPhase.Moved:
                        if (Touches.Count == 0 || Vector2.Distance(touch.position, CurrentTouch.Position) > DistanceBetweenTouches)
                        {
                            Touches.Enqueue(new MobileTouch(touch, true));
                        }
                        break;
                    case TouchPhase.Ended:
                        Touches.Enqueue(new MobileTouch(touch, false));
                        break;
                }
            }
        }

        private void UpdateCurrentTouch()
        {
            if (Touches.Count > 0)
            {
                CurrentTouch = Touches.Dequeue();
            }
        }

        private bool ArrivedAtCurrentTouch()
        {
            return EqualWithinTolerance(Player.transform.position, CurrentTouch.TargetPosition);
        }

        private Vector3 WorldPositionFromTouchPosition(Vector2 touchPosition)
        {
            Vector3 worldPosition = new Vector3(0, 0, 0);
            Ray positionRay = ReferenceCameraSnapshot.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(positionRay, out hit))
            {
                worldPosition = hit.point;
            }
            return worldPosition;
        }

        private bool EqualWithinTolerance(Vector3 position, Vector3 target, float tolerance = .15f)
        {
            return Vector3.Distance(position, target) < (tolerance + position.y + target.y);
        }

        public void UpdateConfiguration(float maxDistanceDelta, float distanceBetweenTouches, float rotationSpeed)
        {
            MaxDistanceDelta = maxDistanceDelta;
            DistanceBetweenTouches = distanceBetweenTouches;
            RotationSpeed = rotationSpeed;
        }

        public void LookAtTarget(Vector3 targetPosition)
        {
            Vector3 lookDirection = (targetPosition - Player.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            Player.rotation = Quaternion.Slerp(Player.rotation, lookRotation, RotationSpeed * Time.deltaTime); 
        }
    }
}


