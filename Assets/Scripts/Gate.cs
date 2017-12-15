using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gate : MonoBehaviour {

    public abstract uint GetWidth( );
    public abstract uint GetHeight( );
    public abstract uint GetNodesInCount( );
    public abstract uint GetNodesOutCount( );

    List<Node> NodesIn = new List<Node>( );
    List<Node> NodesOut = new List<Node>( );

    public Vector3[ ] GetNodesInPosition( ) {
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

    public Vector3[ ] GetNodesOutPosition( ) {
        return new Vector3[ ] {
            transform.position + new Vector3(2, 1, 0),
        };
    }

    private void createNodes(Vector3[ ] nodesPosition, bool isIn) {
        foreach (Vector3 nodePosition in nodesPosition) {
            Node node = Instantiate(NodePrefab, nodePosition, Quaternion.identity, this.transform);
            node.isIn = isIn;
            node.gate = this;
            (isIn ? NodesIn : NodesOut).Add(node);
        }
    }

    public void Start( ) {
        createNodes(GetNodesInPosition( ), true);
        createNodes(GetNodesOutPosition( ), false);
    }

    protected abstract bool[ ] Work(bool[ ] inputs);

    private void placeNodes(ICollection<Node> nodes) {
        foreach (Node node in nodes) {
            Main.nodesMap.Set(node.transform.position.ToVector2XZ( ).Add(-0.5f), node);
        }
    }

    public virtual void Place( ) {
        placeNodes(NodesIn);
        placeNodes(NodesOut);
    }
}
