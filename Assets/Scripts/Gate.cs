using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gate : MonoBehaviour {

    public abstract uint GetWidth( );
    public abstract uint GetHeight( );
    public abstract uint GetNodesInCount( );
    public abstract uint GetNodesOutCount( );

    public Vector2[ ] GetNodesIn( ) =>
        new Vector2[ ] { transform.position + new Vector2(-2, -1), transform.position + new Vector2(-2, 1) };

    protected abstract bool[ ] Work(bool[ ] inputs);
}
