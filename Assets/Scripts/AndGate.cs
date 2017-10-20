using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndGate : Gate {

    // Use this for initialization
    void Start( ) {

    }

    // Update is called once per frame
    void Update( ) {

    }

    protected override bool[ ] Work(bool[ ] inputs) {
        foreach (var input in inputs) {
            if (!input) {
                return new bool[ ] { false };
            }
        }
        return new bool[ ] { true };
    }

    public override uint GetWidth( ) {
        return 5u;
    }

    public override uint GetHeight( ) {
        return 5u;
    }
}
