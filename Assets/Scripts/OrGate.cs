using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrGate : Gate {
    #region implemented abstract members of Gate

    public override uint GetNodesInCount( ) {
        return 2;
    }

    public override uint GetNodesOutCount( ) {
        return 1;
    }

    #endregion

    // Use this for initialization
    void Start( ) {
        base.Start( );
    }

    // Update is called once per frame
    void Update( ) {

    }

    protected override bool[ ] Work(bool[ ] inputs) {
        foreach (var input in inputs) {
            if (input) {
                return new bool[ ] { true };
            }
        }
        return new bool[ ] { false };
    }

    public override uint GetWidth( ) {
        return 5u;
    }

    public override uint GetHeight( ) {
        return 5u;
    }
}
