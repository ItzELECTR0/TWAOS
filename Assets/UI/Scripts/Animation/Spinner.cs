using UnityEngine;

namespace Michsky.UI.Heat
{
    [AddComponentMenu("Heat UI/Animation/Spinner")]
    public class Spinner : MonoBehaviour
    {
        [Header("Resources")]
        [SerializeField] private RectTransform objectToSpin;

        [Header("Settings")]
        [SerializeField] [Range(0.1f, 50)] private float speed = 10;
        [SerializeField] private Direction direction;
        float currentPos;

        public enum Direction { Clockwise, CounterClockwise }

        void Awake()
        {
            if (objectToSpin == null)
                this.enabled = false;
        }

        void Update()
        {
            if (direction == Direction.Clockwise)
            {
                currentPos -= speed * 100 * Time.unscaledDeltaTime;
                if (currentPos >= 360) { currentPos = 0; }
            }

            else if (direction == Direction.CounterClockwise)
            {
                currentPos += speed * 100 * Time.unscaledDeltaTime;
                if (currentPos <= -360) { currentPos = 0; }
            }

            objectToSpin.localRotation = Quaternion.Euler(0, 0, currentPos);
        }
    }
}