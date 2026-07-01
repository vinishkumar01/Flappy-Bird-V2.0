using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipePassThrough : MonoBehaviour
{
    private int pipeScore = 1;

    private bool _hasScored = false;

    private void OnEnable()
    {
        _hasScored = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_hasScored)
        {
            return;
        }

        if(collision.gameObject.CompareTag("flappy"))
        {
            Flappy Flappy = collision.gameObject.GetComponent<Flappy>();

            Flappy._flappyAudioSource.PlayOneShot(Flappy._flappyData.scoredAudioClip);

            GameManager.Instance.HandleScore(pipeScore);
            _hasScored = true;
        }
    }
}
