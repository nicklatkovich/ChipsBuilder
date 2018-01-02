using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gate : MonoBehaviour {

    public abstract uint GetWidth( );
    public abstract uint GetHeight( );
    public abstract uint GetNodesInCount( );
    public abstract uint GetNodesOutCount( );

    public Color BlockColor;
    public Color TextColor;

    public List<Node> NodesIn = new List<Node>( );
    public List<Node> NodesOut = new List<Node>( );

    public Vector3[ ] GetNodesInPosition( ) {
        uint nodesCount = GetNodesInCount( );
        int x = -((int)GetWidth( ) - 1) / 2;
        Vector3[ ] result = new Vector3[nodesCount];
        for (int i = 0; i < nodesCount; i++) {
            result[i] =
                this.transform.position +
                new Vector3(x, 1, (i % 2 == 0 ? -1 : 1) * (nodesCount % 2 == 0 ? i / 2 * 2 + 1 : (i + 1) / 2 * 2));
        }
        return result;
    }

    public static Controller Main {
        get { return Controller.Current; }
    }
    public static Node NodePrefab {
        get { return Main.nodePrefab; }
    }

    public Vector3[ ] GetNodesOutPosition( ) {
        uint nodesCount = GetNodesOutCount( );
        int x = ((int)GetWidth( ) - 1) / 2;
        Vector3[ ] result = new Vector3[nodesCount];
        for (int i = 0; i < nodesCount; i++) {
            result[i] =
                this.transform.position +
                new Vector3(x, 1, (i % 2 == 0 ? -1 : 1) * nodesCount % 2 == 0 ? i / 2 * 2 + 1 : (i + 1) / 2 * 2);
        }
        return result;
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
