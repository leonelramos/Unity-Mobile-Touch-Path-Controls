using UnityEngine;

namespace TouchControls
{
    public class MobileTouch
    {
        private static readonly MobileTouch inactiveTouch = new MobileTouch();
        private Touch Finger { get; set; }
        public bool IsActive { get; set; }
        public Vector2 Position { get; }
        public Vector3 TargetPosition { get; set; }
        public bool HasTargetPositionSet { get; set; }
        public TouchPhase Phase { get; }

        public MobileTouch()
        {
            IsActive = HasTargetPositionSet = false;
        }

        public MobileTouch(Touch touch, bool isActive)
        {
            Finger = touch;
            IsActive = isActive;
            Position = touch.position;
            Phase = touch.phase;
            HasTargetPositionSet = false;
        }

        public static MobileTouch InactiveTouch()
        {
            return inactiveTouch;
        }
    }
}

