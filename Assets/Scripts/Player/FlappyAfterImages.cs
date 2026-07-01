using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyAfterImages : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private float _activeTime = 0.1f;
    private float _timeActivated;

    [Header("Alpha")]
    [SerializeField] private float _setAlpha = 0.8f;
    [SerializeField] private float _alphaDecay = 0.85f;
    private float _alpha;

    [Header("Reference")]
    private Transform _flappyVisualTransform;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _flappyVisualSpriteRenderer;

    private Color _color;

    private void OnEnable()
    {
        _flappyVisualTransform = GameObject.FindGameObjectWithTag("flappy").transform;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _flappyVisualSpriteRenderer = _flappyVisualTransform.GetComponent<SpriteRenderer>();
        
        _alpha = _setAlpha;
        _spriteRenderer.sprite = _flappyVisualSpriteRenderer.sprite;

        transform.position = _flappyVisualTransform.position;
        transform.rotation = _flappyVisualTransform.rotation;
        transform.localScale = _flappyVisualTransform.localScale;

        transform.localScale = new Vector2(_flappyVisualTransform.transform.localScale.x + 0.1f, _flappyVisualTransform.transform.localScale.y + 0.2f);

        _timeActivated = Time.time;

        if (_timeActivated > 0)
        {
            //Debug.Log("After image is instantiated and its activated");
        }

    }

    private void Update()
    {
        _alpha -= _alphaDecay * Time.deltaTime;
        _color = new Color(1f, 1f, 1f, _alpha);
        _spriteRenderer.color = _color;

        //Debug.Log("Game Started and this timer is from after images script:" + Time.time);

        if(Time.time >= (_timeActivated + _activeTime))
        {
            PoolManager.ReturnObjectToPool(gameObject, PoolManager.PoolType.AfterImages);
        }
    }
}
