using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlappyMoves : FlappyState
{
    public FlappyMoves(Flappy flappy,FlappyDataSO flappyDataSO, FlappyStateMachine flappyStateMachine):base(flappy, flappyDataSO, flappyStateMachine)
    {

    }

    public override void EnterState()
    {
        base.EnterState();

        GameManager.Instance.gamestates = GameStates.GameStarts;
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public override void FrameUpdate()
    {
        base.FrameUpdate();

        //Get Input Action 
        if (_flappy.WasTouchedOrClicked)
        {
            _flappy.FlapToFly();
        }

        //Check for Doubke Tap
        _flappy.WasDoubleTouched();

       

        if(_flappy._flappyData.doubleTap && !_flappy.isDashing && Time.time - _flappy.lastDash >= _flappy._flappyData.dashCoolDown)
        {
            _flappyStateMachine.ChangeState(_flappy.FlappyDash);

            _flappy._flappyData.doubleTap = false;
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        //Apply the rotation for the bird
        _flappy.FlappyRotation();

        //Also move the bird horizontally
        _flappy.MoveFlappyHorizontally();

    }

    public override void LateFrameUpdate()
    {
        base.LateFrameUpdate();
    }

}
