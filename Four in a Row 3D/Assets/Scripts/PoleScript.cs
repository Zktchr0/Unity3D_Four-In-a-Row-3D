using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleScript : MonoBehaviour {

    public Color natural;
    public Color marker;

    private Renderer poleRend;

    void Start () {
        poleRend = GetComponent<Renderer>();
	}
	
    public void Mark()
    {
        poleRend.material.color = marker;
    }

    public void Unmark()
    {
        poleRend.material.color = natural;

    }
}
