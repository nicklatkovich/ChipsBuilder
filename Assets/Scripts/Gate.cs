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

    public static Controller Main {
        get { return Controller.Current; }
    }
    public static Node NodePrefab {
        get { return Main.nodePrefab; }
    }

    public Vector3[ ] GetNodesOut( ) {
        return new Vector3[ ] {
            transform.position + new Vector3(2, 1, 0),
        };
    }

    private void createNodes(Vector3[ ] nodesPosition, bool isIn) {
        foreach (Vector3 nodePosition in nodesPosition) {
            Node node = Instantiate(NodePrefab, nodePosition, Quaternion.identity, this.transform);
            node.isIn = isIn;
            node.gate = this;
        }
    }

    public void Start( ) {
        createNodes(GetNodesIn( ), true);
        createNodes(GetNodesOut( ), false);
    }

    protected abstract bool[ ] Work(bool[ ] inputs);
}
