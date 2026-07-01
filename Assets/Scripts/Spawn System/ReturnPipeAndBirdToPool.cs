using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnPipeAndBirdToPool : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Pipe") || collision.CompareTag("PipePassThrough"))
        {
            Debug.Log("The pipe is colliding with the boundary");

            PoolManager.ReturnObjectToPool(collision.gameObject.transform.parent.gameObject);
        }
        
        if(collision.CompareTag("bird"))
        {
            Debug.Log("The bird is colliding with the boundary");

            //CameraShakeController.instance.Shake(1f);

            PoolManager.ReturnObjectToPool(collision.gameObject);
        }
    }
}
