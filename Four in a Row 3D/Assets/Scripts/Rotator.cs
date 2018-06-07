using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    public float speed = 8f;
    private float turnInputValue;


    void Update () {
        turnInputValue = Input.GetAxis("Rotate");
    }

    void FixedUpdate () {
        float turn = turnInputValue * speed * Time.deltaTime;
            transform.Rotate(0f, turn, 0f);
    }
}
