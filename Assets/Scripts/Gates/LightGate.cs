using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGate : Gate {
    #region implemented abstract members of Gate

    public override uint GetNodesInCount( ) {
        return 1;
    }

    public override uint GetNodesOutCount( ) {
        return 0;
    }

    #endregion

    public bool State = false;

    private Light Light;

    // Use this for initialization
    void Start( ) {
        base.Start( );
        Light = transform.Find("Light").GetComponent<Light>( );
        Light.enabled = false;
    }

    // Update is called once per frame
    void Update( ) {
        Light.enabled = NodesIn[0].State;
        Utils.ChangeColor(this, NodesIn[0].State ? Color.green : Color.gray);
    }

    protected override bool[ ] Work(bool[ ] inputs) {
        return new bool[ ] { };
    }

    public override uint GetWidth( ) {
        return 3u;
    }

    public override uint GetHeight( ) {
        return 3u;
    }
}
