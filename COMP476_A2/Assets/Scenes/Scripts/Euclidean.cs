using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Euclidean : AStar
{
    Dictionary<string, MTuple> openList { get; set; }
    Dictionary<string, MTuple> closedList { get; set; }

    string start { get; set; }
    string goal { get; set; }
    string current { get; set; }

    List<MTuple> solutionPath { get; set; }

    public void initial(int startNode, int goalNode)
    {
        this.openList = new Dictionary<string, MTuple>();
        closedList = new Dictionary<string, MTuple>();
        solutionPath = new List<MTuple>();
        start = startNode.ToString();
        goal = goalNode.ToString();
        current = "";
    }

    /// <summary>
    /// search with Euclidean heuristic
    /// </summary>
    public void doSearch()
    {
        if (openList.Count == 0)
        {
            float gn = 0.0f;
            float hn = getHeuristic(Nodes.nodeBook[int.Parse(start)].transform.position);
            float fn = gn + hn;
            MTuple root = new MTuple(start, gn, "", fn);

            openList.Add(start, root);
            doSearch();
        }
        else
        {

            List<KeyValuePair<string, MTuple>> sortedOpenList = openList.ToList();
            if (sortedOpenList.Count > 1)
            {
                sortedOpenList.Sort((a, b) => a.Value.fn.CompareTo(b.Value.fn));
            }
            MTuple node = sortedOpenList[0].Value;

            openList.Remove(node.hashNode);

            if (!closedList.ContainsKey(node.hashNode))
            {
                //Nodes.nodeBook[int.Parse(node.hashNode)].GetComponent<Renderer>().material.color = new Color(0.0f,1.0f,0.0f);
                //getChildren(node);
                closedList.Add(node.hashNode, node);
            }

            if (goal != node.hashNode)
            {
                getChildren(node);
                //closedList.Add(node.hashNode, node);
                doSearch();
            }
            else
            {
                filterSolutionPath();
            }

        }
    }


    void getChildren(MTuple node)
    {

        Dictionary<int, float> children = Nodes.edgeBook[int.Parse(node.hashNode)];
        foreach (KeyValuePair<int, float> childNode in children)
        {
            float gn = node.gn + childNode.Value;
            float hn = getHeuristic(Nodes.nodeBook[childNode.Key].transform.position);
            float fn = gn + hn;
            string childHashNode = childNode.Key.ToString();
            MTuple child = new MTuple(childHashNode, gn, node.hashNode, fn);

            if (childHashNode != node.hashNode)
            {
                if (openList.ContainsKey(childHashNode))
                {
                    if (openList[childHashNode].fn > fn)
                    {
                        openList[childHashNode] = child;
                    }
                }
                else if (!closedList.ContainsKey(childHashNode))
                {
                    openList.Add(childHashNode, child);
                }

            }


        }

    }

    /// <summary>
    /// Euclidean heuristic from tar to goal
    /// </summary>
    /// <param name="pos">tar's pos</param>
    /// <returns></returns>
    float getHeuristic(Vector3 pos)
    {
        Vector3 dir = Nodes.nodeBook[int.Parse(goal)].transform.position - pos;
        return dir.magnitude;
    }


    public Dictionary<string, MTuple> getClosedList()
    {
        return closedList;
    }

    public Dictionary<string, MTuple> getOpenList()
    {
        return openList;
    }


    public void filterSolutionPath()
    {
        List<KeyValuePair<string, MTuple>> sortedClosedList = closedList.ToList();
        if (sortedClosedList.Count > 1)
        {
            sortedClosedList.Sort((a, b) => a.Value.fn.CompareTo(b.Value.fn));
        }


        List<MTuple> solPath = new List<MTuple>();
        solPath.Add(sortedClosedList[sortedClosedList.Count - 1].Value);
        string parent = solPath[solPath.Count - 1].edgeNode;
        while (parent != "")
        {
            solPath.Add(closedList[parent]);
            parent = closedList[parent].edgeNode;
        }
        solPath.Reverse();

        solutionPath = solPath;
    }

    public List<MTuple> getSolutionPath()
    {
        return solutionPath;
    }

}
