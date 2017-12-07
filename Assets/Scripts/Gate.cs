using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gate : MonoBehaviour {

    public abstract uint GetWidth( );
    public abstract uint GetHeight( );
    public abstract uint GetNodesInCount( );
    public abstract uint GetNodesOutCount( );

    public Vector3[ ] GetNodesIn( ) {
        return new Vector3[ ] {
            transform.position + new Vector3(-2, 1, -1),
            transform.position + new Vector3(-2, 1,  1)
        };
    }

    public Vector3[ ] GetNodesOut( ) {
        return new Vector3[ ] {
            transform.position + new Vector3(2, 1, 0),
        };
    }

    public void Start( ) {
        foreach (Vector3 nodePosition in GetNodesIn( )) {
            Node node = Instantiate(Controller.Current.nodePrefab, nodePosition, Quaternion.identity, this.transform);
            node.isIn = true;
            node.gate = this;

        }
        foreach (Vector3 nodePosition in GetNodesOut( )) {
            Node node = Instantiate(Controller.Current.nodePrefab, nodePosition, Quaternion.identity, this.transform);
            node.isIn = false;
            node.gate = this;
        }
    }

    protected abstract bool[ ] Work(bool[ ] inputs);
}
