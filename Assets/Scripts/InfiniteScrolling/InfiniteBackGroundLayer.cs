using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InfiniteBackGroundLayer : MonoBehaviour
{
    private Transform[] _tiles;
    private Vector3[] _initialPosition;
    private Transform _camera;
    private float _tilesWidth;

    private void Awake()
    {
        //Assign Camera
        _camera = Camera.main.transform;

        //Get the Child Objects and store it.
        _tiles = new Transform[transform.childCount];
        _initialPosition = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            _tiles[i] = transform.GetChild(i);

            //Store the initial position
            _initialPosition[i] = _tiles[i].localPosition;
        }

        //As all the background's are gonna be same we just have to take the first bounds size
        _tilesWidth = _tiles[0].GetComponent<SpriteRenderer>().bounds.size.x;
        //Debug.Log(_tilesWidth);
    }

    private void Update()
    {
        foreach(Transform tile in _tiles)
        {
            if(_camera.position.x > tile.position.x + (_tilesWidth * 0.9f))
            {
                MoveTileToForward(tile);
            }
        }
    }


    private void MoveTileToForward(Transform tile)
    {
        Transform rightMost = _tiles[0];

        foreach(Transform t in _tiles)
        {
            if(t.position.x > rightMost.position.x)
            {
                rightMost = t;
            }
        }

        tile.position = new Vector3(rightMost.position.x + _tilesWidth, tile.position.y, tile.position.z);
    }

    public void ResetBackGround()
    {
        for(int i =0; i < _tiles.Length; i++)
        {
            _tiles[i].localPosition = _initialPosition[i];   
        }
    }
}
