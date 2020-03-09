using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nodes : MonoBehaviour
{
    public static Dictionary<int, GameObject> nodesBook;
    public static Dictionary<int, Dictionary<int, GameObject>> nodesLinkBook;

    public GameObject linkLine;

    public Text debug;
    // Start is called before the first frame update
    void Start()
    {
        debug.text = "";
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");
        nodesBook = new Dictionary<int, GameObject>();
        nodesLinkBook = new Dictionary<int, Dictionary<int, GameObject>>();
        //debug.text = "" + nodes.Length; 
        debug.text = "" + Vector3.Angle(new Vector3(0,0,1), new Vector3(1,0,-1).normalized);
        //initial nodes books
        for (int i=0;i<nodes.Length; ++i)
        {
            //debug.text += nodes[0].GetHashCode().ToString() + "\n";
            nodesBook.Add(nodes[i].GetHashCode(), nodes[i]);
            nodesLinkBook.Add(nodes[i].GetHashCode(), new Dictionary<int, GameObject>());
        }

        foreach(KeyValuePair<int, GameObject> nodeA in nodesBook)
        {
            foreach(KeyValuePair<int,GameObject> nodeB in nodesBook)
            {
                if(nodeA.Key != nodeB.Key)
                {
                    //debug.text += "1";
                    
                    
                    Vector3 dirc = nodeB.Value.transform.position - nodeA.Value.transform.position;
                    float ang = Vector3.Angle(new Vector3(0, 0, 1), dirc);
                    if (dirc.x < 0)
                    {
                        ang *= -1;
                    }

                    int layerMaskFixedOnly = LayerMask.GetMask("fixed");
                    RaycastHit hit;
                    if (Physics.Raycast(nodeA.Value.transform.position,dirc.normalized,out hit, 80.0f, layerMaskFixedOnly))
                    {
                        if (hit.distance > dirc.magnitude)
                        {
                            
                            GameObject link = Instantiate(linkLine, nodeA.Value.transform);
                            link.transform.rotation = Quaternion.Euler(0, ang, 0);
                            Vector3 pScale = nodeA.Value.transform.localScale;
                            Vector3 reScale = new Vector3(1.0f / pScale.x, 1.0f / pScale.y, 1.0f / pScale.z);
                            link.transform.localScale = new Vector3(reScale.x, reScale.y, dirc.magnitude * reScale.z);

                            // add to the link book
                            nodesLinkBook[nodeA.Key].Add(nodeB.Key, nodeB.Value);
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
