using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MTuple
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
public interface AStar
{
    void initial(int startNode, int goalNode);

    void doSearch();

    //void getChildren(MTuple node);

    Dictionary<string, MTuple> getOpenList();

    Dictionary<string, MTuple> getClosedList();

    List<MTuple> GetSolutionPath();



}
