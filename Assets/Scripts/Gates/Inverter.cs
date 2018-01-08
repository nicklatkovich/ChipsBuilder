using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Gate {
    #region implemented abstract members of Gate

    public override uint GetNodesInCount( ) {
        return 1;
    }

    public override uint GetNodesOutCount( ) {
        return 1;
    }

    #endregion

    void Start( ) {
        base.Start( );
    }

    void Update( ) {
    }

    protected override bool[ ] Work(bool[ ] inputs) {
        return new bool[ ] { !inputs[0] };
    }

    public override uint GetWidth( ) {
        return 3u;
    }

    public override uint GetHeight( ) {
        return 3u;
    }
}
