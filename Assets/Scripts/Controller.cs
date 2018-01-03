using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour {

    private static Controller _current;

    public static Controller Current {
        get { return _current; }
    }

    public GameObject realPlane;
    public AndGate andGatePrefab;
    public OrGate orGatePrefab;
    public InitGate initGatePrefab;
    public Node nodePrefab;
    public Net netPrefab;
    public CanvasRenderer selectPanel;

    enum State {
        Camera,
        BlockStand,
        BlockSelect,
        Net,
    }

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

    Gate standingBlock;
    Net standingNet;

    Gate selectedBlock;

    public Node[ ][ ] nodesMap;
    Gate[ ][ ] gatesMap;
    bool[ ][ ] collisionMap;
    bool mousePing = false;
    Queue<Node> qNodes = new Queue<Node>( );

    public const float CLICK_TIME = 0.5f;

    void Start( ) {
        _current = this;
        mousePlane = new Plane(Vector3.up, Vector3.zero);
        realPlane.transform.position = new Vector3(mapWidth / 2f, 0f, mapHeight / 2f);
        realPlane.transform.localScale = new Vector3(mapWidth / 10f, 1f, mapHeight / 10f);
        Material realPlaneMaterial = realPlane.GetComponent<Renderer>( ).material;
        realPlaneMaterial.mainTextureScale = new Vector2(mapWidth, mapHeight);
        realPlaneMaterial.mainTextureOffset = new Vector2(0.5f, 0.5f);
        // TODO: create height-map for realPlane
        mainCam = Camera.main;
        float cameraDistance = Mathf.Max(mapWidth, mapHeight);
        mainCam.transform.position = realPlane.transform.position - new Vector3(0, 0, cameraDistance);
        mainCam.transform.rotation = Quaternion.identity;
        mainCam.transform.RotateAround(realPlane.transform.position, Vector3.right, 80f);
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

    public void CreateInitGate( ) {
        CreateGate(initGatePrefab);
        mousePing = true;
    }

    void Update( ) {
        if (Input.GetKeyDown(KeyCode.A)) {
            CreateGate(andGatePrefab);
        } else if (Input.GetKeyDown(KeyCode.O)) {
            CreateGate(orGatePrefab);
        } else if (Input.GetKeyDown(KeyCode.I)) {
            CreateGate(initGatePrefab);
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            switch (state) {
                case State.Net: {
                        standingNet.from.nets.Remove(standingNet);
                        Destroy(standingNet.gameObject);
                        standingNet = null;
                        break;
                    }
                case State.BlockStand: {
                        Destroy(standingBlock.gameObject);
                        standingBlock = null;
                        break;
                    }
                case State.BlockSelect: {
                        selectedBlock = null;
                        selectPanel.transform.localScale = Vector3.zero;
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
                    previousUnderNode.ChangeColor(Color.gray);
                    previousUnderNode = null;
                }
                Node underNode = nodesMap[x][z];
                if (underNode != null) {
                    underNode.ChangeColor(Color.green);
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
                    for (uint i = (uint)x - 2, iTo = i + 5; i < iTo; i++) {
                        for (uint j = (uint)z - 2, jTo = j + 5; j < jTo; j++) {
                            if (collisionMap[i][j]) {
                                canStandBlock = false;
                                i = iTo - 1;
                                break;
                            }
                        }
                    }
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
                                for (uint i = (uint)x - 2, iTo = i + 5; i < iTo; i++) {
                                    for (uint j = (uint)z - 2, jTo = j + 5; j < jTo; j++) {
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
                    } else if (Time.time - mLBPressedTime <= CLICK_TIME) {
                        if (state == State.Camera) {
                            if (nodesMap[x][z] != null) {
                                standingNet = Instantiate(netPrefab);
                                state = State.Net;
                                standingNet.from = nodesMap[x][z];
                            } else if (gatesMap[x][z] != null) {
                                state = State.BlockSelect;
                                selectedBlock = gatesMap[x][z];
                                selectPanel.transform.localScale = new Vector3(1, 1, 1);
                            }
                        } else if (state == State.BlockSelect) {
                            selectedBlock = null;
                            state = State.Camera;
                            selectPanel.transform.localScale = Vector3.zero;
                        } else if (state == State.Net) {
                            if (nodesMap[x][z] != null && standingNet.from != nodesMap[x][z]) {
                                standingNet.to = nodesMap[x][z];
                                standingNet.Done = true;
                                standingNet = null;
                                state = State.Camera;
                            }
                        }
                    }
                }
                if (Input.GetMouseButtonDown(1)) {
                    canRotate = true;
                    lastHitPoint = hitPoint;
                }
                if (Input.GetMouseButtonUp(1)) {
                    canRotate = false;
                }
            }
            if (Input.GetMouseButton(2) && canDrag) {
                Vector3 mouse3DTranslation = ray.GetPoint(distance);
                mouse3DTranslation.y = 0;
                mainCam.transform.position -= (mouse3DTranslation - mousePos3d);
            } else if (state == State.BlockStand) {
                if (canStandBlock) {
                    standingBlock.transform.position = new Vector3(x + 0.5f, 0, z + 0.5f);
                    standingBlock.ChangeColor("GateMaterial", standingBlock.BlockColor);
                    standingBlock.ChangeColor("GateTextMaterial", standingBlock.TextColor);
                } else {
                    standingBlock.transform.position = new Vector3(hitPoint.x, 0.6f, hitPoint.z);
                    standingBlock.ChangeColor("GateMaterial", new Color(1f, 0f, 0f, 0.5f));
                    standingBlock.ChangeColor("GateTextMaterial", new Color(1f, 0f, 0f, 0.5f));
                    //standingBlock.ChangeColor("^NodeMaterial", new Color(1f, 0f, 0f, 0.5f));
                }
            } else if (state == State.Net) {
                standingNet.abstractTo = hitPoint.ToVector2XZ( );
            }
            if (Input.GetMouseButton(1) && canRotate) {
                mainCam.transform.RotateAround(lastHitPoint, Vector3.up, mousePos.x - prevMousePos.x);
                Vector3 mainCamPositionBackup = mainCam.transform.position;
                Quaternion mainCamRotationBackup = mainCam.transform.rotation;
                mainCam.transform.RotateAround(lastHitPoint,
                    Vector3.Cross(Vector3.up, mainCam.transform.rotation * Vector3.up), prevMousePos.y - mousePos.y);
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
                    newQNode.Enqueue(newNode);
                    newNode.State = node.State;
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
        for (uint i = (uint)x - 2, iTo = i + 5; i < iTo; i++) {
            for (uint j = (uint)z - 2, jTo = j + 5; j < jTo; j++) {
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
        if (selectedBlock == null) return;
        InitGate g;
        if ((g = selectedBlock.GetComponent<InitGate>( )) == null) return;
        g.NodesOut[0].State = g.State = !g.State;
        qNodes.Enqueue(g.NodesOut[0]);
    }
}
