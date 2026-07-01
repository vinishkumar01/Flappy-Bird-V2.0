using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyDash : FlappyState
{
    //Dash Conifgs
    private float _dashTimeLeft;

    private float _defaultGravity;

    public FlappyDash(Flappy flappy, FlappyDataSO _flappyDataSO, FlappyStateMachine flappyStateMachine) : base(flappy, _flappyDataSO, flappyStateMachine)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        if (Time.time - _flappy.lastDash < _flappy._flappyData.dashCoolDown)
        {
            _flappyStateMachine.ChangeState(_flappy.FlappyMoves);

            return;
        }

        _flappy.isDashing = true;
        _dashTimeLeft = _flappy._flappyData.dashTime;
        _flappy.lastDash = Time.time;

        //After Images Configs goes here
        _flappy.lastImageXPos = _flappy.transform.position.x;

        //we will set the current image of the flappy of x coordinate to the last image x position

        //Set gravity to default
        _defaultGravity = _flappy.rb2D.gravityScale;

        //Set the Animation and trail
        _flappy._flappyAnim.SetBool("isDashing", _flappy.isDashing);
        _flappy._flappyTrailRenderer.enabled = _flappy.isDashing;

        if (_flappy.isDashing)
        {
            //Set the gravity scale to 0
            _flappy.rb2D.gravityScale = 0;
            
            //We are setting the position of x and y same untill the dash finishes
            _flappy.transform.position = new Vector2(_flappy.transform.position.x, _flappy.transform.position.y);

            //
            _flappy.rb2D.velocity = new Vector2(_flappy.rb2D.velocity.x, 0f);

            //Ignore the pipe and bird layer so that we dont collide with them
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Flappy"), LayerMask.NameToLayer("Birds"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Flappy"), LayerMask.NameToLayer("Pipes"), true);

            //Calling the Dash Method that is created below
            Dash();

            //Play Dash Sound
            _flappy._flappyAudioSource.PlayOneShot(_flappy._flappyData.dashAudioClip);
        }
    }

    public override void ExitState()
    {
        base.ExitState();

        //Flappy's Ignore layer stays active for 0.25 seconds
        _flappy.StartCoroutine(_flappy.DashRecovery());
        _flappy.isDashing = false;
        _flappy.rb2D.gravityScale = _defaultGravity;

        _flappy._flappyAnim.SetBool("isDashing",_flappy.isDashing);
        _flappy._flappyTrailRenderer.enabled = _flappy.isDashing;
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Lock Rotation in z axis
        _flappy.transform.eulerAngles = Vector3.zero;

        //Applying the After Images here while dashing
        if(Mathf.Abs(_flappy.transform.position.x - _flappy.lastImageXPos) > _flappy.distanceBetweenImages)
        {
            PoolManager.SpawnObject(_flappy.flappyAfterImage, _flappy.transform.position, Quaternion.identity, PoolManager.PoolType.AfterImages);

            _flappy.lastImageXPos = _flappy.transform.position.x;
        }


        // here we are decreasing the dashtime every frame so that the flappy doesnt keep dashing the whole time
        _dashTimeLeft -= Time.deltaTime;

        if(_dashTimeLeft <= 0)
        {
            _flappyStateMachine.ChangeState(_flappy.FlappyMoves);
        }

        //Getting the direction of the flappy to apply for the knock bak for the bird
        Vector3 direction = _flappy.rb2D.velocity.normalized;

        //Draw Circle around the bird to check its colliding with the pipe
        Collider2D pipeHit = Physics2D.OverlapCircle(_flappy.transform.position + new Vector3(0.19f, 0, 0), 0.35f, _flappy._flappyData.pipe);

        if(pipeHit != null)
        {
            DestroyPipe pipe = pipeHit.GetComponent<DestroyPipe>();

            if(pipe != null)
            {
                //Disables the pipe sprite and Collider
                pipe.BreakPipe();

                //Play pipe and bird collideSound
                _flappy._flappyAudioSource.PlayOneShot(_flappy._flappyData.pipeAndBirdCollideAudioClip);

                //Update the Score
                GameManager.Instance.HandleScore(pipe.pipeDestroyScore);
            }
        }

        //Check For the birds
        Collider2D BirdHit = Physics2D.OverlapCircle(_flappy.transform.position + new Vector3(0.19f, 0, 0), 0.35f, _flappy._flappyData.bird);

        if (BirdHit != null)
        {
            Bird bird = BirdHit.GetComponent<Bird>();

            if (bird != null)
            {
                //Apply Knock for the bird
                bird.CallKnockBackRoutine(direction, Vector2.up);

                //Play pipe and bird collideSound
                _flappy._flappyAudioSource.PlayOneShot(_flappy._flappyData.pipeAndBirdCollideAudioClip);

                //This method contains the Score Updation part
                bird.IsKilled();
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void LateFrameUpdate()
    {
        base.LateFrameUpdate();
    }

    private void Dash()
    {
        _flappy.rb2D.velocity = new Vector2(_flappy._flappyData.dashVelocity, 0);

        //GameplayPunchForBackgrounds.instance.Punch();
        CameraShakeController.instance.Shake(0.6f);
    }
}
