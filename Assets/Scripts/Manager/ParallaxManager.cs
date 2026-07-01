using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private Transform[] _backGroundImages;
    [SerializeField] private float[] _parallaxScale;

    private Transform _camera;
    private Vector3 _previousCameraPosition;

    private void Awake()
    {
        _camera = Camera.main.transform;
    }

    private void OnEnable()
    {
        _previousCameraPosition = _camera.position;

        _parallaxScale = new float[_backGroundImages.Length];

        for (int i = 0; i < _backGroundImages.Length; i++)
        {
            _parallaxScale[i] = 1f / (i + 2f);
        }
    }

    private void LateUpdate()
    {
        if(_camera == null) return;

        float camDeltaX = _camera.position.x - _previousCameraPosition.x; 

        for (int i = 0; i < _backGroundImages.Length; i++)
        {
            //position based parallax
            float posParallax = camDeltaX * _parallaxScale[i]; 

            float targetX = _backGroundImages[i].position.x + posParallax; 

            Vector3 targetPos = new Vector3(targetX, _backGroundImages[i].position.y, _backGroundImages[i].position.z);

            _backGroundImages[i].position = targetPos;
        }

        _previousCameraPosition = _camera.position;
    }
}
