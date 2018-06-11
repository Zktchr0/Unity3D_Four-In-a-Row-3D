using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoleScript : MonoBehaviour {

    public Color natural;
    private Renderer poleRend;

    void Start () {
        poleRend = GetComponent<Renderer>();
	}
	
    public void Mark(Color playerColor)
    {
        poleRend.material.color = playerColor;
    }

    public void Unmark()
    {
        poleRend.material.color = natural;
    }
}
