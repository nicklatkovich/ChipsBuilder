using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour {

    public Controller Main {
        get { return Controller.Current; }
    }

    private Node _from;
    public Node from {
        get { return _from; }
        set {
            _from = value; 
            value.nets.Add(this);
        }
    }
    private Node _to;
    public Node to {
        get { return _to; }
        set {
            _to = value;
            value.nets.Add(this);
        }
    }
    public Vector2 abstractTo;
    private bool _done = false;
    public bool Done {
        get { return _done; }
        set {
            _done = value;
            Main.qNodes.Enqueue(from);
            Main.qNodes.Enqueue(to);
        }
    }

    void Start( ) {

    }

    void Update( ) {
        Vector2 f = from.transform.position.ToVector2XZ();
        Vector2 t = Done ? to.transform.position.ToVector2XZ() : abstractTo;
        transform.localScale = new Vector3((f - t).magnitude, 1, 1);
        transform.position = ((f + t) / 2f).ToVector3XZ(1f);
        transform.localRotation = Quaternion.Euler(0, -(t - f).GetAtan2Deg( ), 0);
        if (from != null && to != null) {
            Utils.ChangeColor(this, from.State && to.State ? Color.green : Color.gray);
        }
    }
}
