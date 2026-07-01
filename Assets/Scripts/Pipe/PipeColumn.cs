using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeColumn : MonoBehaviour
{
    private void OnEnable()
    {
        DestroyPipe[] pipes = GetComponentsInChildren<DestroyPipe>();

        foreach(var pipe in pipes)
        {
            pipe.ResetPipe();
        }
    }
}
