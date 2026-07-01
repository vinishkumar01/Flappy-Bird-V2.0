using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flappy : MonoBehaviour
{
    //Flappy Initial Position
    private Vector3 _flappyInitialPosition;

    //States
    public FlappyStateMachine _flappyStateMachine;

    public FlappyIdle FlappyIdle { get; set; }
    public FlappyMoves FlappyMoves { get; set; }
    public FlappyDash FlappyDash { get; set; }

    [Header("Flappy Data")]
    public FlappyDataSO _flappyData;

    //Access RigidBody and Collider
    [HideInInspector] public Rigidbody2D rb2D;
    [HideInInspector] public Animator _flappyAnim;
    [HideInInspector] public AudioSource _flappyAudioSource;

    //Access to SpriteRenderer and Sprite
    [HideInInspector] public SpriteRenderer _flappySpriteRenderer;
    [Header("Death sprite")]
    [SerializeField] private Sprite _deathFlappySprite;

    [Header("Trail Renderer")]
    public TrailRenderer _flappyTrailRenderer;

    [Header("Shield")]
    public GameObject _flappyShield;

    [Header("Double Tap Configs")]
    private float _lastTapTime;
    private float _timeSinceLastTap;

    [Header("Dash and After Images Config")]
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public float lastDash;

    [HideInInspector] public float lastImageXPos;
    public float distanceBetweenImages;
    public GameObject flappyAfterImage;

    [SerializeField] private LayerMask _hazardous;

    //Invincible
    public bool IsInvincible { get; private set; }

    private void OnEnable()
    {
        GameManager.Instance.isFlappyDead = false;
    }

    private void Awake()
    {
        _flappyStateMachine = new FlappyStateMachine();

        FlappyIdle = new FlappyIdle(this, _flappyData, _flappyStateMachine);
        FlappyMoves = new FlappyMoves(this, _flappyData, _flappyStateMachine);
        FlappyDash = new FlappyDash(this, _flappyData, _flappyStateMachine);

        //Reset Bird Rotation in z axis to 0 as we are storing it in SO the last play value is stored as -90
        _flappyData.birdRotation = new Vector3(0, 0, 0);
    }

    private void Start()
    {
        //Get the RigidBody 
        rb2D = GetComponent<Rigidbody2D>();
        _flappyAnim = GetComponent<Animator>();
        _flappyAudioSource = GetComponent<AudioSource>();
        _flappySpriteRenderer = GetComponent<SpriteRenderer>();
        _flappyTrailRenderer = GetComponent<TrailRenderer>();

        _flappyTrailRenderer.enabled = false;

        //Initializing flappy idle state as the default.
        _flappyStateMachine.Initialize(FlappyIdle);
    }

    // Flappy Control
    public bool WasTouchedOrClicked => Input.GetButtonUp("Jump") || 
     Input.GetMouseButtonDown(0) || 
      (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended) ? true : false;

    public void WasDoubleTouched()
    {

        if (GameManager.Instance.isFlappyDead) return;

        if(Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0))
        {
            _timeSinceLastTap = Time.time - _lastTapTime;

            if(_timeSinceLastTap <= _flappyData._doubleTapTimeLimit)
            {
                _flappyData.doubleTap = true;
                Debug.Log("Double Tapped");
                _lastTapTime = -Mathf.Infinity;
            }
            else
            {
                _flappyData.doubleTap = false;
                _lastTapTime = Time.time;
            }
        }

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if(touch.phase == TouchPhase.Began && touch.tapCount == 2)
            {
                _flappyData.doubleTap = true;
                Debug.Log("Double Tapped");
            }
            else
            {
                _flappyData.doubleTap = false;
            }
        }
    }

    private void Update()
    {
        _flappyStateMachine._currentState.FrameUpdate();

        UpdateDashUI();
    }

    private void FixedUpdate()
    {
        _flappyStateMachine._currentState.PhysicsUpdate();
    }

    private void LateUpdate()
    {
        _flappyStateMachine._currentState.LateFrameUpdate();
    }

    // All the method which are implemented here will be called in the particualar states

    //Move the Flappy Horizontally
    public void MoveFlappyHorizontally()
    {
        if(GameManager.Instance.isFlappyDead) return;

        transform.position += new Vector3(Time.deltaTime * _flappyData.horizontalVelocity, 0, 0);
    }

    public void FlapToFly()
    {
        if(GameManager.Instance.isFlappyDead) return;

        rb2D.velocity = new Vector2(0,_flappyData.velocityPerFlap);

        //Play Flapping Audio 
        _flappyAudioSource.PlayOneShot(_flappyData.flyAudioClip);
    }


    //Applies Rotation for the bird 
    public void FlappyRotation()
    {
        if(GameManager.Instance.gamestates == GameStates.GameOver)
        {
            return;
        }

        float degreesToAdd = 0f;

        //Going Up
        if(rb2D.velocity.y > 0)
        {
            degreesToAdd = 6f * _flappyData.rotateUpSpeed;
        }
        //Falling Down
        else if(rb2D.velocity.y < 0)
        {
            degreesToAdd = -3f * _flappyData.rotateDownSpeed;
        }

        _flappyData.birdRotation = new Vector3(0,0,Mathf.Clamp(_flappyData.birdRotation.z + degreesToAdd, -90, 45));

        transform.eulerAngles = _flappyData.birdRotation;
    }

    public IEnumerator DashRecovery()
    {
        IsInvincible = true;
        _flappyShield.SetActive(true);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Flappy"), LayerMask.NameToLayer("Birds"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Flappy"), LayerMask.NameToLayer("Pipes"), true);

        yield return new WaitForSeconds(0.8f);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Flappy"), LayerMask.NameToLayer("Birds"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Flappy"), LayerMask.NameToLayer("Pipes"), false);

        IsInvincible = false;
        _flappyShield.SetActive(false);
    }

    private void UpdateDashUI()
    {
        float elapsed = Time.time - lastDash;

        float fill = Mathf.Clamp01(elapsed / _flappyData.dashCoolDown);

        //Update the dash Image every frame
        GameManager.Instance.HandleDashImage(fill);
    }

    public void ResetFlappiesPositionWhenPressedRetry()
    {
        // Set the flappy position to initial position
        transform.position = _flappyInitialPosition;

        transform.eulerAngles = new Vector3(0, 0, 0);

        rb2D.velocity = Vector2.zero;
        rb2D.angularVelocity = 0f;

        GameManager.Instance.isFlappyDead = false;
        isDashing = false;
        lastDash = Time.time;

        //Reset the ignoring layer
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Flappy"), LayerMask.NameToLayer("Birds"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Flappy"), LayerMask.NameToLayer("Pipes"), false);

        _flappyStateMachine.ChangeState(FlappyIdle);
    }

    private bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return (mask.value & (1 << obj.layer)) != 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance.isFlappyDead)
            return;

        if (IsInLayerMask(collision.gameObject, _hazardous))
        {
            Debug.Log("Is Dead");
            GameManager.Instance.isFlappyDead = true;

            //Dead Audio
            _flappyAudioSource.PlayOneShot(_flappyData.deathAudioClip);

            //Disable the animator
            _flappyAnim.enabled = false;
            _flappySpriteRenderer.sprite = _deathFlappySprite;

            rb2D.velocity = Vector3.zero;

            //Change the game state to Game Start
            GameManager.Instance.gamestates = GameStates.GameOver;
        }
    }

    //private void OnGUI()
    //{
    //    GUIStyle mystyle = new GUIStyle(GUI.skin.label);

    //    mystyle.fontSize = 24;
    //    mystyle.alignment = TextAnchor.MiddleCenter;

    //    GUI.Label(new Rect(650, 10, 500, 300), $"CurrentState: {_flappyStateMachine._currentState?.GetType().Name}", mystyle);
    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position + new Vector3(0.19f, 0, 0), 0.35f);
    //}

   
}
 