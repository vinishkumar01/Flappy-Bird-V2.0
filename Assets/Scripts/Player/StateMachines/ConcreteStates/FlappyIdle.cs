using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlappyIdle : FlappyState
{
    private float _defaultGravity;
    private float _defualtMass;

    public FlappyIdle(Flappy flappy,FlappyDataSO flappyDataSO, FlappyStateMachine flappyStateMachine) : base(flappy,flappyDataSO, flappyStateMachine)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        _flappy._flappyAnim.enabled = true;

        _defaultGravity = _flappy.rb2D.gravityScale;
        _defualtMass = _flappy.rb2D.mass;
    }

    public override void ExitState()
    {
        _flappy.rb2D.gravityScale = _defaultGravity;
        _flappy.rb2D.mass = _defualtMass;  

        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //for this part we are setting the gravity and mass to normal
        _flappy.rb2D.gravityScale = 1f;
        _flappy.rb2D.mass = 1f;

        if(_flappy.WasTouchedOrClicked)
        {
            //Change the game state to Game Start
            GameManager.Instance.gamestates = GameStates.GameStarts;

            //And switch the flappy state to Move State
            _flappyStateMachine.ChangeState(_flappy.FlappyMoves);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        //Move flappy horizontally 
        _flappy.MoveFlappyHorizontally();

        //Also make it hop a little in air
        if(_flappy.rb2D.velocity.y < -1) // When it falls 
        {
            //Give a slight push upwards
            _flappy.rb2D.AddForce(new Vector2(0, _flappy.rb2D.mass * 6000 * Time.deltaTime));
        }

    }

    public override void LateFrameUpdate()
    {
        base.LateFrameUpdate();
    }
}
