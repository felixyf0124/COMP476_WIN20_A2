using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cluster : AStar
{
    Dictionary<string, MTuple> openList { get; set; }
    Dictionary<string, MTuple> closedList { get; set; }

    string start { get; set; }
    string goal { get; set; }

    string startCluster { get; set; }
    string goalCluster { get; set; }

    string current { get; set; }

    List<MTuple> solutionPath { get; set; }

    string innerStart;
    string innerGoal;


    public void initial(int startNode, int goalNode)
    {
        this.openList = new Dictionary<string, MTuple>();
        closedList = new Dictionary<string, MTuple>();
        solutionPath = new List<MTuple>();
        start = startNode.ToString();
        goal = goalNode.ToString();
        innerStart = "";
        innerGoal = "";
        startCluster = Nodes.nodeBook[startNode].transform.parent.gameObject.GetHashCode().ToString();
        goalCluster = Nodes.nodeBook[goalNode].transform.parent.gameObject.GetHashCode().ToString();


        current = "";
    }

    /// <summary>
    /// search with Cluster heuristic
    /// </summary>
    public void doSearch()
    {
        float gn, hn, fn;

        if (startCluster == goalCluster)
        {
            innerStart = start;
            innerGoal = goal;
            doInnerSearch();
        }
        else
        {
            ClusterPath cPath = Nodes.clusterTable[int.Parse(startCluster)][int.Parse(goalCluster)];
            if (cPath.getFirst() != start)
            {// do A* search in this cluster till hit the exit
                innerStart = start;
                innerGoal = cPath.getFirst();
                doInnerSearch();
            }
            else
            {
                innerGoal = cPath.getLast();
                gn = 0.0f;
                hn = getHeuristic(Nodes.nodeBook[int.Parse(start)].transform.position);
                fn = gn + hn;
                MTuple root = new MTuple(start, gn, "", fn);
                closedList.Add(start, root);
            }

            openList.Clear();

            innerGoal = cPath.getLast();


            if (cPath.path.Count > 2)
            {
                doClusterTransition(cPath);
            }

            // goal cluster

            innerStart = cPath.getLast();
            innerGoal = goal;
            gn = closedList[cPath.path[cPath.path.Count - 2]].gn
                + Nodes.edgeBook[int.Parse(cPath.getLast())][int.Parse(cPath.path[cPath.path.Count - 2])];
            hn = getHeuristic(Nodes.nodeBook[int.Parse(cPath.getLast())].transform.position);
            fn = gn + hn;
            MTuple transitionNode = new MTuple((cPath.getLast()), gn, cPath.path[cPath.path.Count - 2], fn);
            openList.Add(innerStart, transitionNode);
            doInnerSearch();
        }

        filterSolutionPath();
}

    /// <summary>
    /// search with Cluster heuristic
    /// </summary>
    public void doInnerSearch()
    {
        if (openList.Count == 0)
        {
            float gn = 0.0f;
            float hn = getHeuristic(Nodes.nodeBook[int.Parse(innerStart)].transform.position);
            float fn = gn + hn;
            MTuple root = new MTuple(innerStart, gn, "", fn);

            openList.Add(innerStart, root);
            doInnerSearch();
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
                //getChildren(node);
                closedList.Add(node.hashNode, node);
            }

            if (innerGoal != node.hashNode)
            {
                getChildren(node);
                //closedList.Add(node.hashNode, node);
                doInnerSearch();
            }
            else
            {
                //filterSolutionPath();
            }

        }
    }

    void doClusterTransition(ClusterPath cPath)
    {
        for(int i = 1; i < cPath.path.Count-1; ++i)
        {
            float gn = closedList[cPath.path[i - 1]].gn + Nodes.edgeBook[int.Parse(cPath.path[i])][int.Parse(cPath.path[i - 1])];
            float hn = getHeuristic(Nodes.nodeBook[int.Parse(cPath.path[i])].transform.position);
            float fn = gn + hn;
            MTuple transitionNode = new MTuple((cPath.path[i]), gn, cPath.path[i - 1], fn);
            closedList.Add(transitionNode.hashNode, transitionNode);
        }

    }

    void getChildren(MTuple node)
    {

        string currentCluster = "";

        
        Transform currentClu = Nodes.nodeBook[int.Parse(node.hashNode)].transform.parent;
        currentCluster = currentClu.gameObject.GetHashCode().ToString();

        Dictionary<int, float> children = Nodes.edgeBook[int.Parse(node.hashNode)];
        foreach (KeyValuePair<int, float> childNode in children)
        {
            string clusterHC = "";
            Transform cluTransfer = Nodes.nodeBook[childNode.Key].transform.parent;
            clusterHC = cluTransfer.gameObject.GetHashCode().ToString();
            if (clusterHC == currentCluster)
            //if (true)
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

    }

    /// <summary>
    /// Euclidean heuristic from tar to goal
    /// </summary>
    /// <param name="pos">tar's pos</param>
    /// <returns></returns>
    float getHeuristic(Vector3 pos)
    {
        float hn = 0.0f;
        if (startCluster != goalCluster)
        {
            ClusterPath cPath = Nodes.clusterTable[int.Parse(startCluster)][int.Parse(goalCluster)];
            if(innerGoal == cPath.getFirst())
            {
                Vector3 dir = Nodes.nodeBook[int.Parse(innerGoal)].transform.position - pos;
                Vector3 dir2 = Nodes.nodeBook[int.Parse(cPath.getFirst())].transform.position 
                    - Nodes.nodeBook[int.Parse(innerGoal)].transform.position;
                Vector3 dir3 = Nodes.nodeBook[int.Parse(goal)].transform.position 
                    - Nodes.nodeBook[int.Parse(cPath.getFirst())].transform.position;
                
                hn = dir.magnitude + dir2.magnitude + dir3.magnitude;
            }
            else
            {
                Vector3 dir = Nodes.nodeBook[int.Parse(innerGoal)].transform.position - pos;
                Vector3 dir2 = Nodes.nodeBook[int.Parse(goal)].transform.position - Nodes.nodeBook[int.Parse(innerGoal)].transform.position;
                hn = dir.magnitude + dir2.magnitude;
            }
        }
        else
        {
            Vector3 dir = Nodes.nodeBook[int.Parse(goal)].transform.position - pos;
            hn = dir.magnitude;
        }

        return hn;
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
