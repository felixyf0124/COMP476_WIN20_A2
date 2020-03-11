using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameController : MonoBehaviour
{

    public Camera camera;

    public Text debug;

    public Text gameState;


    bool isCTRL;

    bool toggleSingleNodeLink;

    List<GameObject> pickedNodes;

    string searchModeTXT;

    int searchMode;

    AStar aStar;

    // Start is called before the first frame update
    void Start()
    {
        debug.text = "";
        isCTRL = false;
        toggleSingleNodeLink = false;
        disableCursor();
        
        pickedNodes = new List<GameObject>();

        switchSearchMode(3);
    }

    // Update is called once per frame
    void Update()
    {
        //Nodes.nodesBook;
        inputListener();

        if (isCTRL)
        {
            onMouseSelect();
        }

        if (!isCTRL && toggleSingleNodeLink)
        {
            displaySingleNodeLink();
        }
    }

    void inputListener()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCTRL = true;
            enableCursor();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCTRL = false;
            disableCursor();
        }

        if (Input.GetKey(KeyCode.R))
        {
            //for(int i = 0; i < pickedNodes.Count; ++i)
            //{
            //    pickedNodes[i].GetComponent<Renderer>().material.color = Colors.lightBlue;
            //}
            debug.text = "";
            pickedNodes.Clear();
            resetNodesAndLinks();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            toggleSingleNodeLink = !toggleSingleNodeLink;
            if (toggleSingleNodeLink)
            {
                enableCursor();
                pickedNodes.Clear();

                resetNodesAndLinks();
            }
            else
            {
                disableCursor();
            }

        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            resetNodesAndLinks();
            switchSearchMode(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            resetNodesAndLinks();
            switchSearchMode(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            resetNodesAndLinks();
            switchSearchMode(3);
        }

        gameState.text = searchModeTXT;

    }

    void enableCursor()
    {
        if (!Cursor.visible)
        {
            Cursor.visible = true;
        }
    }

    void disableCursor()
    {
        if (Cursor.visible)
        {
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// on mouse select input
    /// </summary>
    void onMouseSelect()
    {
        // if left button pressed ...
        if (Input.GetMouseButtonDown(0))
        { 
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //GameObject selected = hit.transform.GetComponent<GameObject>();
                if(hit.transform.tag == "node")
                {
                    debug.text += hit.transform.gameObject.GetHashCode() + "\n";

                    if (pickedNodes.Count < 2)
                    {
                        if (pickedNodes.Count == 1
                            && pickedNodes[0].GetHashCode() == hit.transform.gameObject.GetHashCode())
                        {
                            hit.transform.GetComponent<Renderer>().material.color = Colors.lightBlue;
                            pickedNodes.RemoveAt(0);
                        }
                        else
                        {
                            pickedNodes.Add(hit.transform.gameObject);
                            if(pickedNodes.Count == 1)
                            {
                                hit.transform.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                            }
                            else
                            {
                                hit.transform.GetComponent<Renderer>().material.color = new Color(255, 255, 0);
                            }
                        }

                    }
                    if (pickedNodes.Count == 2)
                    {
                        aStar.initial(pickedNodes[0].GetHashCode(), pickedNodes[1].GetHashCode());
                        aStar.doSearch();

                        Dictionary<string, MTuple> openList = aStar.getOpenList();
                        
                        Dictionary<string, MTuple> closedList = aStar.getClosedList();
                        
                        List<MTuple> path  = aStar.getSolutionPath();

                        colorPathFindingResult(openList, closedList, path);

                    }
                }
            }
        }
    }

    /// <summary>
    /// colorPathFindingResult
    /// </summary>
    /// <param name="openList"></param>
    /// <param name="closedList"></param>
    /// <param name="path"></param>
    void colorPathFindingResult(Dictionary<string, MTuple> openList, Dictionary<string, MTuple> closedList, List<MTuple> path)
    {

        foreach (KeyValuePair<string, MTuple> node in openList)
        {

            Nodes.nodeBook[int.Parse(node.Key)].GetComponent<Renderer>().material.color
                = new Color(1.0f, 1.0f, 1.0f);
        }
        foreach (KeyValuePair<string, MTuple> node in closedList)
        {
            Nodes.nodeBook[int.Parse(node.Key)].GetComponent<Renderer>().material.color
                = new Color(0.0f, 0.0f, 0.0f);
        }

        for (int i = 0; i < path.Count; ++i)
        {
            Color col = new Color(1.0f, i / (float)(path.Count - 1), 0.0f);
            Nodes.nodeBook[int.Parse(path[i].hashNode)].GetComponent<Renderer>().material.color
                = col;
            if (i < path.Count - 1)
            {
                Color col2 = new Color(1.0f, (i + 1.0f) / (float)(path.Count - 1), 0.0f);
                colorLink(int.Parse(path[i].hashNode), int.Parse(path[i + 1].hashNode), col, col2);
            }
;
        }
    }
    

    /// <summary>
    /// color solution path link
    /// </summary>
    /// <param name="node"></param>
    /// <param name="child"></param>
    /// <param name="col"></param>
    /// <param name="col2"></param>
    void colorLink(int node, int child, Color col, Color col2)
    {
        GameObject nodeObj = Nodes.nodeBook[node];
        GameObject childObj = Nodes.nodeBook[child];
        Transform[] links = nodeObj.GetComponentsInChildren<Transform>();
        //GameObject[] links = nodeObj.transform.GetComponentsInChildren<GameObject>();
        foreach (Transform link in links)
        {
            if (link.tag == "link")
            {
                float magnitude = (float)Math.Round(10000 *
                    (link.localScale.z * nodeObj.transform.localScale.z)) / 10000.0f;
                float magnitude2 = (float)Math.Round(10000 * Nodes.edgeBook[node][child]) / 10000.0f;
                Vector3 dirc = childObj.transform.position - nodeObj.transform.position;
                float ang = (float)Math.Round(10000 * (Vector3.Angle(new Vector3(0, 0, 1), dirc))) / 10000.0f;
                if (dirc.x < 0)
                {
                    ang *= -1.0f;
                }
                Quaternion angQuat = Quaternion.Euler(0, ang, 0);
                   
                if (magnitude == magnitude2
                    && angQuat == link.localRotation)
                {
                    link.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
                    LineRenderer linkRenderer = link.gameObject.GetComponent<LineRenderer>();
                    linkRenderer.startColor = col;
                    linkRenderer.endColor = col2;
                    linkRenderer.SetWidth(0.6f, 0.6f);

                }
            }

        }
    }

    /// <summary>
    /// reset Nodes' And Links' colors and sizes and positions
    /// </summary>
    void resetNodesAndLinks()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");
        GameObject[] links = GameObject.FindGameObjectsWithTag("link");

        foreach(GameObject node in nodes)
        {
            node.GetComponent<Renderer>().material.color = Colors.lightBlue; 
        }

        foreach (GameObject link in links)
        {
            link.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            LineRenderer linkRenderer = link.GetComponent<LineRenderer>();
            linkRenderer.startColor = Colors.green;
            linkRenderer.endColor = Colors.green;
            linkRenderer.SetWidth(0.1f, 0.1f);
        }

    }


    void displaySingleNodeLink()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(Cursor.visible && toggleSingleNodeLink)
            {
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    //GameObject selected = hit.transform.GetComponent<GameObject>();
                    if (hit.transform.tag == "node")
                    {
                        debug.text += hit.transform.gameObject.GetHashCode() + "\n";
                        if (pickedNodes.Count >0) 
                        {
                            pickedNodes.Clear(); 
                        }

                        resetNodesAndLinks();
                        int nodeHash = hit.transform.gameObject.GetHashCode();

                        hit.transform.GetComponent<Renderer>().material.color = Color.red;

                        foreach(Transform link in hit.transform)
                        {
                            if(link.tag == "link")
                            {
                                link.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
                                LineRenderer linkRenderer = link.gameObject.GetComponent<LineRenderer>();
                                linkRenderer.startColor = Color.red;
                                linkRenderer.endColor = Color.yellow;
                                linkRenderer.SetWidth(0.6f, 0.6f);
                            }
                        }

                        foreach(KeyValuePair<int,float> child in Nodes.edgeBook[nodeHash])
                        {
                            colorNode(child.Key, Color.yellow);
                        }

                    }
                }
            }
        }

    }

    void colorNode(int hashNode, Color col)
    {
        Nodes.nodeBook[hashNode].GetComponent<Renderer>().material.color
                = col;
    }



    void switchSearchMode(int caseId)
    {
        switch (caseId)
        {
            case 1:
                searchMode = 1;
                searchModeTXT = "Search Mode: \n"
                + "g(n): cost-so-far \n"
                + "h(n): null \n";

                resetNodesAndLinks();
                aStar = new Dijkstra();
                if(pickedNodes.Count == 2)
                {
                    aStar.initial(pickedNodes[0].GetHashCode(), pickedNodes[1].GetHashCode());
                    aStar.doSearch();

                    Dictionary<string, MTuple> openList = aStar.getOpenList();

                    Dictionary<string, MTuple> closedList = aStar.getClosedList();

                    List<MTuple> path = aStar.getSolutionPath();

                    colorPathFindingResult(openList, closedList, path);
                }

                break;

            case 2:
                searchMode = 2;
                searchModeTXT = "Search Mode: \n"
                + "g(n): cost-so-far \n"
                + "h(n): Euclidean \n";

                aStar = new Euclidean();
                if (pickedNodes.Count == 2)
                {
                    aStar.initial(pickedNodes[0].GetHashCode(), pickedNodes[1].GetHashCode());
                    aStar.doSearch();

                    Dictionary<string, MTuple> openList = aStar.getOpenList();

                    Dictionary<string, MTuple> closedList = aStar.getClosedList();

                    List<MTuple> path = aStar.getSolutionPath();

                    colorPathFindingResult(openList, closedList, path);
                }

                break;
            case 3:
                searchMode = 3;
                searchModeTXT = "Search Mode: \n"
                + "g(n): cost-so-far \n"
                + "h(n): Cluster \n";

                aStar = new Cluster();
                if (pickedNodes.Count == 2)
                {
                    aStar.initial(pickedNodes[0].GetHashCode(), pickedNodes[1].GetHashCode());
                    aStar.doSearch();

                    Dictionary<string, MTuple> openList = aStar.getOpenList();

                    Dictionary<string, MTuple> closedList = aStar.getClosedList();

                    List<MTuple> path = aStar.getSolutionPath();

                    colorPathFindingResult(openList, closedList, path);

                }

                break;
        }
    }

}
