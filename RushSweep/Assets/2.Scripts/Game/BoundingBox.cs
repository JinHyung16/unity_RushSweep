using UnityEngine;

namespace Hugh.Game
{
    public class BoundingBox
    {
        private float _hWidth, _hDepth;
        public Vector3 Position { get; private set; }

        public float Width { get; private set; }
        public float Depth { get; private set; }
        public float Left => Position.x - _hWidth;
        public float Right => Position.x + _hWidth;
        public float Near => Position.z - _hDepth;
        public float Far => Position.z + _hDepth;

        public void Set(float width, float depth, Vector3 position)
        {
            Position = position;
            Width = width;
            Depth = depth;

            _hWidth = width * 0.5f;
            _hDepth = depth * 0.5f;
        }

        public void UpdatePosition(Vector3 position)
        {
            Position = position;
        }

        public void OnDrawGizmos(Color color)
        {
            Gizmos.color = color;
            Vector3 boxCenter = new Vector3((Left + Right) * 0.5f, 0.5f, (Near + Far) * 0.5f);
            Vector3 boxSize = new Vector3(Right - Left, 1.0f, Far - Near);
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }
}