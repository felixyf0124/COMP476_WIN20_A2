using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class Nodes : MonoBehaviour
{
    public static Dictionary<int, GameObject> nodeBook;
    public static Dictionary<int, Dictionary<int, float>> edgeBook;
    public static Dictionary<int, HashSet<int>> clusterBook;

    // <cluster hashcode, <cluster hash code, closest hash node>
    public static Dictionary<int, Dictionary<int, int>> clusterTable;

    public GameObject linkLine;

    public Text debug;
    // Start is called before the first frame update
    void Start()
    {
        debug.text = "";
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("node");
        GameObject[] clusters = GameObject.FindGameObjectsWithTag("cluster");
        nodeBook = new Dictionary<int, GameObject>();
        edgeBook = new Dictionary<int, Dictionary<int, float>>();
        clusterBook = new Dictionary<int, HashSet<int>>();
        clusterTable = new Dictionary<int, Dictionary<int, int>>();
        //debug.text = "" + nodes.Length; 
        //debug.text = "" + Vector3.Angle(new Vector3(0,0,1), new Vector3(1,0,-1).normalized);

        //initial clusterBook
        for (int i = 0; i < clusters.Length; ++i)
        {
            clusterBook.Add(clusters[i].GetHashCode(), new HashSet<int>());
            clusterTable.Add(clusters[i].GetHashCode(), new Dictionary<int, int>());
        }


        //initial nodes books
        for (int i=0;i<nodes.Length; ++i)
        {
            nodeBook.Add(nodes[i].GetHashCode(), nodes[i]);
            edgeBook.Add(nodes[i].GetHashCode(), new Dictionary<int, float>());

            GameObject myCluster = nodes[i].transform.parent.gameObject;
            if (!clusterBook[myCluster.GetHashCode()].Contains(nodes[i].GetHashCode()))
            {
                clusterBook[myCluster.GetHashCode()].Add(nodes[i].GetHashCode());
            }

        }

        
        // link nodes
        foreach (KeyValuePair<int, GameObject> nodeA in nodeBook)
        {
            foreach(KeyValuePair<int,GameObject> nodeB in nodeBook)
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
                    if (Physics.Raycast(nodeA.Value.transform.position,dirc.normalized,out hit, 1000.0f, layerMaskFixedOnly))
                    {
                        if (hit.distance > dirc.magnitude)
                        {
                            
                            GameObject link = Instantiate(linkLine, nodeA.Value.transform);
                            link.transform.rotation = Quaternion.Euler(0, ang, 0);
                            Vector3 pScale = nodeA.Value.transform.localScale;
                            Vector3 reScale = new Vector3(1.0f / pScale.x, 1.0f / pScale.y, 1.0f / pScale.z);
                            link.transform.localScale = new Vector3(reScale.x, reScale.y, dirc.magnitude * reScale.z);

                            // add to the link book
                            edgeBook[nodeA.Key].Add(nodeB.Key, dirc.magnitude);
                        }
                    }
                }
            }
        }

        // link clusters
        cookClusterTable();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void cookClusterTable()
    {
        AStar aStar = new Euclidean();

        GameObject[] clusters = GameObject.FindGameObjectsWithTag("cluster");

        

        for(int i = 0; i < clusters.Length; ++i)
        {
            int cluHC = clusters[i].GetHashCode();

            for (int j = i + 1; j < clusters.Length; ++j)
            {
                if (i != j)
                {
                    int cluHC2 = clusters[j].GetHashCode();

                    List<int> shortestHashNodePair = new List<int>();
                    float shortestDistance = -1.0f;

                    foreach (int hashNode in clusterBook[cluHC])
                    {
                        foreach (int hashNode2 in clusterBook[cluHC2])
                        {
                            aStar.initial(hashNode, hashNode2);
                            aStar.doSearch();
                            List<MTuple> path = aStar.getSolutionPath();

                            bool isEitherCluster = true;

                            for (int k = 0; k < path.Count; ++k)
                            {
                                if (clusterBook[cluHC].Contains(int.Parse(path[k].hashNode))
                                    || clusterBook[cluHC2].Contains(int.Parse(path[k].hashNode)))
                                {
                                    //continue;
                                }
                                else
                                {
                                    isEitherCluster = false;
                                    break;
                                }
                            }
                            if (isEitherCluster)
                            {
                                if (shortestDistance < 0)
                                {
                                    shortestHashNodePair.Add(int.Parse(path[0].hashNode));
                                    shortestHashNodePair.Add(int.Parse(path[path.Count - 1].hashNode));
                                    shortestDistance = getTotalCost(path);
                                }
                                if (shortestDistance > 0)
                                {
                                    float distance = getTotalCost(path);
                                    if (distance < shortestDistance)
                                    {
                                        shortestDistance = distance;
                                        shortestHashNodePair.Add(int.Parse(path[0].hashNode));
                                        shortestHashNodePair.Add(int.Parse(path[path.Count - 1].hashNode));
                                    }
                                }
                            }
                        }
                    }

                    // there is a direct short path between two cluster
                    if (shortestDistance > 0)
                    {
                        if (clusterBook[cluHC].Contains(shortestHashNodePair[0])) {

                            if (clusterTable[cluHC].ContainsKey(cluHC2))
                            {
                                clusterTable[cluHC][cluHC2] = shortestHashNodePair[1];
                            }
                            else
                            {
                                clusterTable[cluHC]
                                    .Add(cluHC2, shortestHashNodePair[1]);
                            }

                            if (clusterTable[cluHC2].ContainsKey(cluHC))
                            {
                                clusterTable[cluHC2][cluHC] = shortestHashNodePair[0];
                            }
                            else
                            {
                                clusterTable[cluHC2]
                                    .Add(cluHC, shortestHashNodePair[0]);
                            }

                        }
                        else
                        {
                            if (clusterTable[cluHC].ContainsKey(cluHC2))
                            {
                                clusterTable[cluHC][cluHC2] = shortestHashNodePair[0];
                            }
                            else
                            {
                                clusterTable[cluHC]
                                    .Add(cluHC2, shortestHashNodePair[0]);
                            }

                            if (clusterTable[cluHC2].ContainsKey(cluHC))
                            {
                                clusterTable[cluHC2][cluHC] = shortestHashNodePair[1];
                            }
                            else
                            {
                                clusterTable[cluHC2]
                                    .Add(cluHC, shortestHashNodePair[1]);
                            }

                        }

                    }

                }

            }
        }

    }

    /// <summary>
    /// total cost (g(n))
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    float getTotalCost(List<MTuple> path)
    {
        float ttlCost = 0.0f;

        for(int i = 0; i < path.Count - 1; ++i)
        {
            Vector3 nodePos = nodeBook[int.Parse(path[i].hashNode)].transform.position;
            Vector3 nodePos1 = nodeBook[int.Parse(path[i+1].hashNode)].transform.position;
            Vector3 dir = nodePos - nodePos1;
            ttlCost += dir.magnitude;
        }
        return ttlCost;
    }

}
