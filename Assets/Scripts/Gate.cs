using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gate : MonoBehaviour {

    public abstract uint GetWidth( );
    public abstract uint GetHeight( );

    protected abstract bool[ ] Work(bool[ ] inputs);
}
