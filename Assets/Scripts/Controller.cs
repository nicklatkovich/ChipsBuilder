using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Controller : MonoBehaviour {

    private static Controller _current;

    public static Controller Current {
        get { return _current; }
    }

    enum State {
        Camera,
        BlockStand,
        BlockSelect,
        Net,
        NodeStanding,
    }

    public GameObject realPlane;
    public AndGate andGatePrefab;
    public OrGate orGatePrefab;
    public InitGate initGatePrefab;
    public Inverter inverterPrefab;
    public LightGate lightGatePrefab;
    public Node nodePrefab;
    public Net netPrefab;
    public CanvasRenderer selectPanel;

    State state = State.Camera;
    uint mapWidth = 50;
    uint mapHeight = 50;
    Plane mousePlane;
    Camera mainCam;
    Vector3 mousePos3d;
    Vector2 prevMousePos;
    bool canDrag = false;
    bool canRotate = false;
    Vector3 lastHitPoint = Vector3.zero;
    Node previousUnderNode = null;
    float mLBPressedTime = 0f;
    float mRBPressedTime = 0f;

    Gate standingBlock;
    Net standingNet;
    Node standingNode;

    Gate selectedBlock;

    public Node[ ][ ] nodesMap;
    Gate[ ][ ] gatesMap;
    bool[ ][ ] collisionMap;
    bool mousePing = false;
    internal Queue<Node> qNodes = new Queue<Node>( );

    public const float CLICK_TIME = 0.5f;

    void Start( ) {
        _current = this;
        mousePlane = new Plane(Vector3.up, Vector3.zero);
        realPlane.transform.position = new Vector3(
            mapWidth / 2f,
            0f,
            mapHeight / 2f);
        realPlane.transform.localScale = new Vector3(
            mapWidth / 10f,
            1f,
            mapHeight / 10f);
        Material realPlaneMaterial = realPlane.GetComponent<Renderer>( ).material;
        realPlaneMaterial.mainTextureScale = new Vector2(mapWidth, mapHeight);
        realPlaneMaterial.mainTextureOffset = new Vector2(0.5f, 0.5f);
        // TODO: create height-map for realPlane
        mainCam = Camera.main;
        float cameraDistance = Mathf.Max(mapWidth, mapHeight);
        mainCam.transform.position = realPlane.transform.position - new Vector3(
            0,
            0,
            cameraDistance);
        mainCam.transform.rotation = Quaternion.identity;
        mainCam.transform.RotateAround(
            realPlane.transform.position,
            Vector3.right,
            80f);
        gatesMap = Utils.Init2DArray<Gate>(mapWidth, mapHeight, null);
        nodesMap = Utils.Init2DArray<Node>(mapWidth, mapHeight, null);
        collisionMap = Utils.Init2DArray(mapWidth, mapHeight, false);
    }

    public void CreateGate(Gate gatePrefab) {
        if (state == State.Camera) {
            state = State.BlockStand;
            standingBlock = Instantiate(gatePrefab);
            //standingBlock.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
            standingBlock.ChangeAlpha(0.2f);
        }
    }

    public void CreateAndGate( ) {
        CreateGate(andGatePrefab);
        mousePing = true;
    }

    public void CreateOrGate( ) {
        CreateGate(orGatePrefab);
        mousePing = true;
    }

    public void CreateBulb( ) {
        CreateGate(lightGatePrefab);
        mousePing = true;
    }

    public void CreateInitGate( ) {
        CreateGate(initGatePrefab);
        mousePing = true;
    }

    public void CreateInvertor( ) {
        CreateGate(inverterPrefab);
        mousePing = true;
    }

    void Update( ) {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Debug.Log(qNodes.ToArray( ));
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            CreateGate(andGatePrefab);
        }
        else if (Input.GetKeyDown(KeyCode.O)) {
            CreateGate(orGatePrefab);
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            CreateGate(initGatePrefab);
        }
        else if (Input.GetKeyDown(KeyCode.L)) {
            CreateGate(lightGatePrefab);
        }
        else if (Input.GetKeyDown(KeyCode.I)) {
            CreateGate(inverterPrefab);
        }
        else if (Input.GetKeyDown(KeyCode.N)) {
            if (state == State.Camera) {
                state = State.NodeStanding;
                (standingNode = Instantiate(nodePrefab)).Done = false;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            switch (state) {
                case State.Net:
                    {
                        standingNet.from.nets.Remove(standingNet);
                        Destroy(standingNet.gameObject);
                        standingNet = null;
                        break;
                    }
                case State.BlockStand:
                    {
                        Destroy(standingBlock.gameObject);
                        standingBlock = null;
                        break;
                    }
                case State.BlockSelect:
                    {
                        selectedBlock = null;
                        selectPanel.transform.localScale = Vector3.zero;
                        break;
                    }
                case State.NodeStanding:
                    {
                        Destroy(standingNode.gameObject);
                        standingNode = null;
                        break;
                    }
            }
            state = State.Camera;
        }
        Vector2 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        bool canStandBlock = false;
        float distance;
        if (mousePlane.Raycast(ray, out distance)) {
            Vector3 hitPoint = ray.GetPoint(distance);
            int x = Mathf.FloorToInt(hitPoint.x);
            int z = Mathf.FloorToInt(hitPoint.z);
            bool mouseOverMap;
            if (mouseOverMap = x >= 0 && x < mapWidth && z >= 0 && z < mapHeight) {
                if (previousUnderNode != null) {
                    var pos = previousUnderNode.transform.localPosition;
                    pos.y = 1f;
                    previousUnderNode.transform.localPosition = pos;
                    previousUnderNode = null;
                }
                Node underNode = nodesMap[x][z];
                if (underNode != null) {
                    var pos = underNode.transform.localPosition;
                    pos.y = 2f;
                    underNode.transform.localPosition = pos;
                    previousUnderNode = underNode;
                }
                float mouseScroolWheelAxis = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(mouseScroolWheelAxis) > 0f) {
                    Vector3 mainCamPositionBackup = mainCam.transform.position;
                    mainCam.transform.position += mouseScroolWheelAxis * 10f * ray.direction;
                    if (mainCam.transform.position.y < 3f) {
                        mainCam.transform.position = mainCamPositionBackup;
                    }
                }
                if (state == State.BlockStand && (canStandBlock = mouseOverMap)) {
                    if (x < standingBlock.GetWidth( ) / 2 || mapWidth <= x + standingBlock.GetWidth( ) / 2 ||
                        z < standingBlock.GetHeight( ) / 2 || mapHeight <= z + standingBlock.GetHeight( ) / 2) {
                        canStandBlock = false;
                    }
                    else {
                        for (uint i = (uint)x - standingBlock.GetWidth( ) / 2, iTo = i + standingBlock.GetWidth( );
                        i < iTo; i++) {
                            for (uint j = (uint)z - standingBlock.GetWidth( ) / 2,
                            jTo = j + standingBlock.GetWidth( ); j < jTo; j++) {
                                if (collisionMap[i][j]) {
                                    canStandBlock = false;
                                    i = iTo - 1;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (state == State.NodeStanding) {
                    canStandBlock = collisionMap[x][z] == false && nodesMap[x][z] == null;
                }
                if (Input.GetMouseButtonDown(2)) {
                    canDrag = true;
                    mousePos3d = ray.GetPoint(distance);
                    mousePos3d.y = 0;
                }
                if (Input.GetMouseButtonDown(0)) {
                    mLBPressedTime = Time.time;
                }
                if (Input.GetMouseButtonUp(2)) {
                    canDrag = false;
                }
                if (Input.GetMouseButtonUp(0)) {
                    if (state == State.BlockStand) {
                        if (!mousePing) {
                            if (canStandBlock) {
                                state = State.Camera;
                                standingBlock.ChangeAlpha(1f);
                                for (uint i = (uint)x - standingBlock.GetWidth( ) / 2,
                                    iTo = i + standingBlock.GetWidth( ); i < iTo; i++) {
                                    for (uint j = (uint)z - standingBlock.GetHeight( ) / 2,
                                        jTo = j + standingBlock.GetHeight( ); j < jTo; j++) {
                                        gatesMap[i][j] = standingBlock;
                                        collisionMap[i][j] = true;
                                    }
                                    //collisionMap[i][z - 3] = collisionMap[i][z + 3] = true;
                                }
                                //for (uint j = (uint)z - 2, jTo = j + 5; j < jTo; j++) {
                                //    collisionMap[x - 3][j] = collisionMap[x + 3][j] = true;
                                //}
                                standingBlock.Place( );
                                standingBlock = null;
                            }
                        }
                        else {
                            mousePing = false;
                        }
                    }
                    else if (state == State.NodeStanding) {
                        if (!mousePing) {
                            if (canStandBlock) {
                                state = State.Camera;
                                standingNode.Done = true;
                                collisionMap[x][z] = true;
                                nodesMap[x][z] = standingNode;
                                standingNode = null;
                            }
                        }
                    }
                    else if (Time.time - mLBPressedTime <= CLICK_TIME) {
                        if (state == State.Camera) {
                            if (nodesMap[x][z] != null) {
                                standingNet = Instantiate(netPrefab);
                                state = State.Net;
                                standingNet.from = nodesMap[x][z];
                            }
                        }
                        else if (state == State.BlockSelect) {
                            selectedBlock = null;
                            state = State.Camera;
                            selectPanel.transform.localScale = Vector3.zero;
                        }
                        else if (state == State.Net) {
                            bool newNode = false;
                            if (nodesMap[x][z] == null) {
                                Node node = Instantiate(nodePrefab);
                                node.transform.position = new Vector3(x + 0.5f, 1.2f, z + 0.5f);
                                nodesMap[x][z] = node;
                                newNode = true;
                            }
                            if (standingNet.from != nodesMap[x][z]) {
                                standingNet.to = nodesMap[x][z];
                                standingNet.Done = true;
                                if (newNode) {
                                    standingNet = Instantiate(netPrefab);
                                    standingNet.from = nodesMap[x][z];
                                }
                                else {
                                    standingNet = null;
                                    state = State.Camera;
                                }
                            }
                        }
                    }
                }
                if (Input.GetMouseButtonDown(1)) {
                    mLBPressedTime = Time.time;
                    canRotate = true;
                    lastHitPoint = hitPoint;
                }
                if (Input.GetMouseButtonUp(1)) {
                    if (Time.time - mLBPressedTime <= CLICK_TIME) {
                        if (gatesMap[x][z] != null) {
                            state = State.BlockSelect;
                            selectedBlock = gatesMap[x][z];
                            selectPanel.transform.localScale = new Vector3(1, 1, 1);
                        }
                    }
                    canRotate = false;
                }
            }
            if (Input.GetMouseButton(2) && canDrag) {
                Vector3 mouse3DTranslation = ray.GetPoint(distance);
                mouse3DTranslation.y = 0;
                mainCam.transform.position -= (mouse3DTranslation - mousePos3d);
            }
            else if (state == State.BlockStand) {
                if (canStandBlock) {
                    standingBlock.transform.position = new Vector3(
                        x + 0.5f,
                        0,
                        z + 0.5f);
                    standingBlock.ChangeColor(
                        "GateMaterial",
                        standingBlock.BlockColor);
                    standingBlock.ChangeColor(
                        "GateTextMaterial",
                        standingBlock.TextColor);
                }
                else {
                    standingBlock.transform.position = new Vector3(
                        hitPoint.x,
                        0.6f,
                        hitPoint.z);
                    standingBlock.ChangeColor(
                        "GateMaterial",
                        new Color(
                            1f,
                            0f,
                            0f,
                            0.5f));
                    standingBlock.ChangeColor(
                        "GateTextMaterial",
                        new Color(
                            1f,
                            0f,
                            0f,
                            0.5f));
                    //standingBlock.ChangeColor("^NodeMaterial", new Color(1f, 0f, 0f, 0.5f));
                }
            }
            else if (state == State.Net) {
                // standingNet.abstractTo = hitPoint.ToVector2XZ( );
                standingNet.abstractTo = new Vector2(x + 0.5f, z + 0.5f);// hitPoint.ToVector2XZ( );
            }
            else if (state == State.NodeStanding) {
                standingNode.transform.position = new Vector3(x + 0.5f, 1.2f, z + 0.5f);
                Utils.ChangeColor(standingNode, canStandBlock ? Color.gray : Color.red);
            }
            if (Input.GetMouseButton(1) && canRotate) {
                mainCam.transform.RotateAround(
                    lastHitPoint,
                    Vector3.up,
                    mousePos.x - prevMousePos.x);
                Vector3 mainCamPositionBackup = mainCam.transform.position;
                Quaternion mainCamRotationBackup = mainCam.transform.rotation;
                mainCam.transform.RotateAround(
                    lastHitPoint,
                    Vector3.Cross(
                        Vector3.up,
                        mainCam.transform.rotation * Vector3.up),
                    prevMousePos.y - mousePos.y);
                if (mainCam.transform.rotation.eulerAngles.x < 30f || mainCam.transform.position.y < 3f) {
                    // TODO: bring camera close to the border
                    mainCam.transform.position = mainCamPositionBackup;
                    mainCam.transform.rotation = mainCamRotationBackup;
                }
            }
        }
        prevMousePos = mousePos;
        Work( );
    }

    public void Work( ) {
        Queue<Node> newQNode = new Queue<Node>( );
        HashSet<Gate> activatedGates = new HashSet<Gate>( );
        foreach (Node node in qNodes) {
            if (node.isIn) {
                activatedGates.Add(node.gate);
            }
            else {
                foreach (Net net in node.nets) {
                    Node newNode = net.from == node ? net.to : net.from;
                    if (newNode != null && newNode.State != node.State) {
                        newQNode.Enqueue(newNode);
                        newNode.State = node.State;
                    }
                }
            }
        }
        foreach (Gate gate in activatedGates) {
            if (gate.DoWort( )) {
                Node node = gate.NodesOut[0];
                newQNode.Enqueue(node);
            }
        }
        qNodes = newQNode;
    }

    public void RemoveSelectedBlock( ) {
        if (selectedBlock == null) {
            return;
        }
        int x = Mathf.RoundToInt(selectedBlock.transform.position.x - 0.5f);
        int z = Mathf.RoundToInt(selectedBlock.transform.position.z - 0.5f);
        for (uint i = (uint)x - selectedBlock.GetWidth( ) / 2, iTo = i + selectedBlock.GetWidth( ); i < iTo; i++) {
            for (uint j = (uint)z - selectedBlock.GetHeight( ) / 2, jTo = j + selectedBlock.GetHeight( );
                j < jTo; j++) {
                gatesMap[i][j] = null;
                collisionMap[i][j] = false;
                nodesMap[i][j] = null;
            }
        }
        foreach (var node in Utils.Concat(selectedBlock.NodesIn, selectedBlock.NodesOut)) {
            foreach (var net in node.nets) {
                Node otherNode = net.from == node ? net.to : net.from;
                otherNode.nets.Remove(net);
                Destroy(net.gameObject);
            }
            node.nets.Clear( );
        }
        Destroy(selectedBlock.gameObject);
    }

    public void OnChangeInitGate( ) {
        if (selectedBlock == null)
            return;
        InitGate g;
        if ((g = selectedBlock.GetComponent<InitGate>( )) == null)
            return;
        g.NodesOut[0].State = g.State = !g.State;
        qNodes.Enqueue(g.NodesOut[0]);
    }
}
