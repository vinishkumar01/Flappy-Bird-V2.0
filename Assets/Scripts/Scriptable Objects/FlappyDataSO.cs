using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scripts/Scriptable Objects/FlappyDataSO")]
public class FlappyDataSO : ScriptableObject
{
    [Header("Flappy SFX")]
    public AudioClip flyAudioClip;
    public AudioClip deathAudioClip;
    public AudioClip scoredAudioClip;
    public AudioClip dashAudioClip;
    public AudioClip pipeAndBirdCollideAudioClip;

    [Header("Flappy flying config")]
    public float rotateUpSpeed = 1f;
    public float rotateDownSpeed = 1f;
    public float velocityPerFlap = 3f;
    public float horizontalVelocity = 1f;

    public Vector3 birdRotation = Vector3.zero;

    [Header("Double Tap setting")]
    public float _doubleTapTimeLimit = 0.2f;
    public bool doubleTap = false;

    [Header("Flappy Dash")]
    public float dashTime = 1f;
    public float dashVelocity = 50f;
    public float dashCoolDown = 3f;
    public bool canDash = false;

    [Header("Check For Pipes for collision")]
    public bool hasTouchedThePipe;
    public LayerMask pipe;
    public LayerMask bird;
}
