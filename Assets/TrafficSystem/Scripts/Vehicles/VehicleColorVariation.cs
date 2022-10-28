using UnityEngine;
namespace TrafficSystem
{
    /// <summary>
    /// Sets the colour of a vehicle every time it initializes.
    /// </summary>
    public class VehicleColorVariation : MonoBehaviour
    {
        private MeshRenderer _renderer = null;

        [SerializeField]
        private Color[] _colorVariations = null;

        private void Awake()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        public void Generate()
        {
            if (_colorVariations == null || _colorVariations.Length == 0 || _renderer == null)
            {
                return;
            }

            _renderer.material.color = _colorVariations[Random.Range(0, _colorVariations.Length)];
        }
    }
}