using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    private float cameraz;
    [SerializeField] private Transform _player;
    
    void Start()
    {
        cameraz = transform.position.z;
    }

    
    void Update()
    {
        transform.position = new Vector3(_player.position.x + 1.5f, 0, cameraz);
    }
}
