using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Scripts.Player
{
    public class Scanner : MonoBehaviour
    {
        [Header("Scan Parameters")]
        [SerializeField] private Transform _scanPoint;
        [SerializeField] private int _raycastsPerFixedUpdate = 10;
        [SerializeField] private LayerMask _scanLayerMask = new LayerMask();
        [SerializeField] private int _scanDispersion = 150;
        [SerializeField] private int _maxDispersion = 300;
        [SerializeField] private int _minDispersion = 50;
        [SerializeField] private int _dispersionChangeStep = 10;

        [SerializeField] private float _mins = 0.75f;

        private Transform _cameraTransform;

        private readonly List<PaintableSurface> _contactedSurfacesPerFrame = new List<PaintableSurface>();

        private void Start() 
        {
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) == false)  // DEBUG ONLY, TODO: SWAP TO NEW INPUT SYSTEM
                return;
            
            PaintSpray();
        }

        private void PaintSpray() 
        {
            for (var i = 0; i < _raycastsPerFixedUpdate; i++)
                PaintOnePoint();

            ApplyChangesOnSurfaces();
        }

        private void PaintOnePoint()
        {
            if (Physics.Raycast(_cameraTransform.position, GetDispersedVector(), out var hit, maxDistance: Mathf.Infinity, layerMask: _scanLayerMask) == false) 
                return;
            
            if (!hit.collider.TryGetComponent(out PaintableSurface surface)) 
                return;
                
            surface.DrawPixelOnRaycastHit(hit);

            if (_contactedSurfacesPerFrame.Contains(surface) == false)
                _contactedSurfacesPerFrame.Add(surface);
        }

        private void ApplyChangesOnSurfaces() 
        {
            foreach (var surface in _contactedSurfacesPerFrame)
                surface.ApplyTextureChanges();

            _contactedSurfacesPerFrame.Clear();
        }

        private Vector3 GetDispersedVector() 
        {
            var direction = _cameraTransform.forward;

            direction += Quaternion.AngleAxis(Random.Range(0, 360), _cameraTransform.forward) * _cameraTransform.up * Random.Range(0, _scanDispersion / 360f);

            return direction;
        }

        private void DecreaseScanRadius() 
        {
            ChangeScanRadius(-_dispersionChangeStep);
        }

        private void IncreaseScanRadius() 
        {
            ChangeScanRadius(_dispersionChangeStep);
        }

        private void ChangeScanRadius(int amount)
        {
            _scanDispersion += amount;
            _scanDispersion = Mathf.Clamp(_scanDispersion, _minDispersion, _maxDispersion);
        }
    }
}