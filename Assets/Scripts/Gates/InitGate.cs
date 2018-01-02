using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitGate : Gate {
    #region implemented abstract members of Gate

    public override uint GetNodesInCount( ) {
        return 0;
    }

    public override uint GetNodesOutCount( ) {
        return 1;
    }

    #endregion

    public bool State = false;

    private Transform One;
    private Transform Zero;

    // Use this for initialization
    void Start( ) {
        base.Start( );
        One = transform.Find("InitGate").Find("Text1");
        Zero = transform.Find("InitGate").Find("Text0");
    }

    // Update is called once per frame
    void Update( ) {
        One.gameObject.SetActive(State);
        Zero.gameObject.SetActive(!State);
    }

    protected override bool[ ] Work(bool[ ] inputs) {
        return new bool[ ] { State };
    }

    public override uint GetWidth( ) {
        return 3u;
    }

    public override uint GetHeight( ) {
        return 3u;
    }
}
