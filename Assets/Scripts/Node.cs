﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

	public bool isIn;
	public Gate gate;

	// Use this for initialization
	void Start ( ) {
        Utils.ChangeColor(this, Color.gray);
	}
	
	// Update is called once per frame
	void Update ( ) {
	}
}
