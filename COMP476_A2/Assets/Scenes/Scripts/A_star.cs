using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


struct MTuple
{
    // node hashcode
    public string hashNode;
    // cost-so-far
    public float gn;
    // edge (parent) node hashcode
    public string edgeNode;
    // total-cost
    public float fn;

    /// <summary>
    /// where edgeNode is the parent's hashcode
    /// </summary>
    /// <param name="hashNode"></param>
    /// <param name="gn"></param>
    /// <param name="edgeNode">parent's hashcode</param>
    /// <param name="fn"></param>
    public MTuple(string hashNode, float gn, string edgeNode, float fn)
    {
        this.hashNode = hashNode;
        this.gn = gn;
        this.edgeNode = edgeNode;
        this.fn = fn;
    }
}

public class A_star : MonoBehaviour
{




    Dictionary<string, MTuple> openList;
    Dictionary<string, MTuple> closedList;

    string start;
    string goal;
    string current;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initial(int startNode, int goalNode)
    {
        openList = new Dictionary<string, MTuple>();
        closedList = new Dictionary<string, MTuple>();
        start = startNode.ToString();
        goal = goalNode.ToString();
        current = "";
    }


    void doDijkstra()
    {
        if(openList.Count == 0)
        {
            MTuple root = new MTuple(start, 0.0f, "", 0.0f);
           
            openList.Add(start, root);
            doDijkstra();
        }
        else
        {
            List<KeyValuePair<string, MTuple>> sortedOpenList = openList.ToList();
            if (sortedOpenList.Count > 1)
            {
                sortedOpenList.Sort((a, b) => a.Value.gn.CompareTo(b.Value.gn));
            }
            MTuple node = sortedOpenList[0].Value;
            openList.Remove(node.hashNode);
            closedList.Add(node.hashNode, node);
            if(goal != node.hashNode)
            {
                getChildren(node);
            }

        }
    }


    void getChildren(MTuple node)
    {
        
        Dictionary<int, float> children = Nodes.edgeBook[int.Parse(node.hashNode)];
        //List<MTuple> childList = new List<MTuple>();
        foreach(KeyValuePair<int, float> childNode in children)
        {
            float gn = node.gn + childNode.Value;
            float fn = gn;
            string childHashNode = childNode.Key.ToString();
            MTuple child = new MTuple(childHashNode, gn, node.hashNode, fn);

            if (openList.ContainsKey(childHashNode))
            {
                if(openList[childHashNode].gn> gn)
                {
                    openList[childHashNode] = child;
                }
            }else if (!closedList.ContainsKey(childHashNode))
            {
                openList.Add(childHashNode, child);
            }
            
        }
        
    }

    
    
}
