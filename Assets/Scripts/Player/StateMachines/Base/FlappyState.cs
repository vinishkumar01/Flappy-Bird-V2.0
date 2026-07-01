using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlappyState
{
    protected Flappy _flappy; 
    protected FlappyDataSO _flappyDataSO;
    protected FlappyStateMachine _flappyStateMachine;

    public FlappyState (Flappy flappy,FlappyDataSO _flappyDataSO , FlappyStateMachine flappyStateMachine)
    {
        this._flappy = flappy;
        this._flappyStateMachine = flappyStateMachine;
    }

    public virtual void EnterState(){ }
    public virtual void ExitState(){ }
    
    public virtual void FrameUpdate(){ }
    public virtual void PhysicsUpdate(){ }
    public virtual void LateFrameUpdate(){ }

}
