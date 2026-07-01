using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    [SerializeField] private float _rotateUpSpeed = 1f;
    [SerializeField] private float _rotateDownSpeed = 1f;
    [SerializeField] private float _velocityPerFlap = 3f;
    [SerializeField] private float _timeBetweenFlap = 0.7f;
    [SerializeField] private float _horizontalVelocity = 1f;

    [SerializeField] private Vector3 _birdRotation = Vector3.zero;

    private Rigidbody2D _rb;

    [Header("Knock Back for the Bird")]
    [SerializeField] private float _knockBackTime = 0.2f;
    [SerializeField] private float _hitDirectionForce = 25f;
    [SerializeField] private float _constForce = 5f;

    private bool _isBeingKnockedBack { get; set; }

    private Coroutine _knockBackCoroutine;

    private bool _isDead;

    [Header("Bird Score")]
    public int birdScore = 5;

    [Header("Bird Animation and Sprite")]
    [SerializeField] private Animator _birdAnimator;
    [SerializeField] private SpriteRenderer _birdSpriteRenderer;
    [SerializeField] private Sprite _deathSprite;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _birdAnimator = GetComponent<Animator>();
        _birdSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _isDead = false;
        _isBeingKnockedBack = false;

        _birdAnimator.enabled = true;

        StartCoroutine(Flap());
    }

    private void FixedUpdate()
    {
        MoveBirdOnXAxis();

        BirdRotation();
    }

    private void MoveBirdOnXAxis()
    {
        if(_isDead) return;

        if (!_isBeingKnockedBack)
        {
            //transform.position += new Vector3(Time.deltaTime * -_horizontalVelocity, 0, 0);

            _rb.velocity = new Vector2(-_horizontalVelocity, _rb.velocity.y);
        }
    }

    private void BoostOnYAxis()
    {
        _rb.velocity = new Vector2(0, _velocityPerFlap);
    }

    private IEnumerator Flap()
    {

        while (true)
        {
            if(!_isDead && !_isBeingKnockedBack)
            {
                BoostOnYAxis();
            }

            yield return new WaitForSeconds(_timeBetweenFlap);
        }
        
    }

    private void BirdRotation()
    {
        float degreesToAdd = 0f;

        if(_rb.velocity.y > 0)
        {
            degreesToAdd = 6f * _rotateUpSpeed;
        }
        else if(_rb.velocity.y < 0)
        {
            degreesToAdd = -3f * _rotateDownSpeed;
        }

        _birdRotation = new Vector3(0,0, Mathf.Clamp(_birdRotation.z + degreesToAdd, -90, 45));

        transform.eulerAngles = -_birdRotation ;
    }

    public void IsKilled()
    {
        if (_isDead) return;

        GameManager.Instance.HandleScore(birdScore);

        _isDead = true;

        _birdAnimator.enabled = false;
        _birdSpriteRenderer.sprite = _deathSprite;
    }

    public void CallKnockBackRoutine(Vector2 hitDirection, Vector2 constForceDirection)
    {
        if(_knockBackCoroutine != null)
        {
            StopCoroutine(_knockBackCoroutine);
            _knockBackCoroutine = null;
        }
        _knockBackCoroutine = StartCoroutine(KnockBackTheBird(hitDirection, constForceDirection));
    }

    private IEnumerator KnockBackTheBird(Vector2 hitDirection, Vector2 constForceDirection)
    {
        _isBeingKnockedBack = true;

        Vector2 hitForce;
        Vector2 constForce;
        Vector2 knockBackForce;
        Vector2 combinedForce;

        hitForce = hitDirection * _hitDirectionForce;
        constForce = constForceDirection * _constForce;

        float elapsedTime = 0f;
        while(elapsedTime < _knockBackTime)
        {
            elapsedTime += Time.fixedDeltaTime;

            //Combine hitForce + constant Force
            knockBackForce = hitForce + constForce;

            combinedForce = knockBackForce;

            //Applying knockBack to the rigidBody
            _rb.velocity = combinedForce;

            yield return new WaitForFixedUpdate();
        }

        _isBeingKnockedBack = false;

        yield return new WaitForSeconds(2f);

        PoolManager.ReturnObjectToPool(this.gameObject, PoolManager.PoolType.GameObjects);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
