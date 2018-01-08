using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

	public bool isIn;
	public Gate gate;
    public HashSet<Net> nets = new HashSet<Net>();
    public bool State = false;

    internal bool Done = true;
	// Use this for initialization
	void Start ( ) {
        Utils.ChangeColor(this, Color.gray);
	}
	
	// Update is called once per frame
	void Update ( ) {
        Utils.ChangeColor(this, State ? Color.green : Color.gray);
	}
}
