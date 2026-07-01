using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyPipe : MonoBehaviour
{
    [SerializeField] private FlappyDataSO _flappyData;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;

    [SerializeField] private PipeParticleScript _pipeParticleScript;

    // Pipe Destroy Score
    public int pipeDestroyScore = 2;

    public bool isBroken;

    public void BreakPipe()
    {
        if(isBroken)
        {
            return;
        }

        _spriteRenderer.enabled = false;
        _collider.enabled = false;

        _pipeParticleScript.CallSpawnPipeParticles();

        isBroken = true;
    }

    //We have to do this so that when the pipe column is reused from the pool we need the pipe to be active
    public void ResetPipe()
    {
        _spriteRenderer.enabled = true;
        _collider.enabled = true;
        isBroken = false;
    }
}
