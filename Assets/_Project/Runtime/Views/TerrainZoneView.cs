using UnityEngine;

namespace Warzone.Runtime.Views
{
    public sealed class TerrainZoneView : MonoBehaviour
    {
        public void Initialize(Color color, Vector3 scale)
        {
            transform.localScale = scale;
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                Color tint = color;
                tint.a = 0.7f;
                renderer.material.color = tint;
            }
        }
    }
}



