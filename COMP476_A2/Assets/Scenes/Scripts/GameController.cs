using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{

    public Camera camera;

    public Text debug;


    bool isCTRL;

    List<GameObject> pickedNodes;

    A_star aStar;

    // Start is called before the first frame update
    void Start()
    {
        debug.text = "";
        isCTRL = false;
        disableCursor();

        pickedNodes = new List<GameObject>();
        aStar = new A_star();
    }

    // Update is called once per frame
    void Update()
    {
        //Nodes.nodesBook;
        inputListener();

        onMouseSelect();
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
            for(int i = 0; i < pickedNodes.Count; ++i)
            {
                pickedNodes[i].GetComponent<Renderer>().material.color = Colors.lightBlue;
            }

            pickedNodes.Clear();
        }
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

    // on mouse select input
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
                            hit.transform.GetComponent<Renderer>().material.color = new Color(255, 0, 0);
                        }

                    }
                    if (pickedNodes.Count == 2)
                    {
                        aStar.initial(pickedNodes[0].GetHashCode(), pickedNodes[1].GetHashCode());
                        aStar.doDijkstra();
                        List<KeyValuePair<string, MTuple>> path  = aStar.GetSolutionPath();
                        debug.text = path[0].Value +"\n";
                        foreach(KeyValuePair<string, MTuple> node in path)
                        {
                            Nodes.nodeBook[int.Parse(node.Key)].GetComponent<Renderer>().material.color
                                = new Color(0.0f, 1.0f, 0.0f);
                        }
                    }
                }
            }
        }
    }

}
