using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
    [Header("Camera Shake")]
    private float shakeStrength;
    [SerializeField] private float _shakeDecay = 6f;
    [SerializeField] private float _noiseSpeed = 30f;

    public Vector3 CurrentShakeOffset { get; private set; }

    [Header("Position and noise config")]
    private Vector3 _baseLocalPosition;
    private Vector2 _noiseSeed;

    public static CameraShakeController instance;

    private void Awake()
    {
        if(instance != this && instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        _baseLocalPosition = transform.localPosition;

        //Noise Seed
        _noiseSeed = Random.insideUnitCircle * 1000f;
    }

    private void LateUpdate()
    {
        CameraShakeUsingPerlinNoise();
    }

    public void Shake(float strength)
    {
        shakeStrength = strength;
    }

    private void CameraShakeUsingPerlinNoise()
    {

        Vector3 shakeOffset = Vector3.zero;

        if (shakeStrength > 0)
        {
            float x = Mathf.PerlinNoise(_noiseSeed.x, Time.time * _noiseSpeed) - 0.5f;
            float y = Mathf.PerlinNoise(_noiseSeed.y, Time.time * _noiseSpeed) - 0.5f;

            shakeOffset = new Vector3(x, y, 0f) * shakeStrength;

            shakeStrength = Mathf.Lerp(shakeStrength, 0f, Time.deltaTime * _shakeDecay);
        }
        else
        {
            shakeStrength = 0f;
        }

        CurrentShakeOffset = shakeOffset;
        transform.localPosition = _baseLocalPosition + shakeOffset;
    }
}
