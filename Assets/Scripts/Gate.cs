using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gate : MonoBehaviour {

    public GameObject NodePrefab;

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
            Instantiate(
                NodePrefab, nodePosition, Quaternion.identity, this.transform);
        }
        foreach (Vector3 nodePosition in GetNodesOut( )) {
            Instantiate(
                NodePrefab, nodePosition, Quaternion.identity, this.transform);
        }
    }

    protected abstract bool[ ] Work(bool[ ] inputs);
}
