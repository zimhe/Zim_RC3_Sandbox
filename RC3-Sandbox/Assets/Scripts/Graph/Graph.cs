using System ;
using System.Collections .Generic;
using System .Linq ;
using System .Text;

public class Graph
{
    public List<List<int>> _adj;

    public Graph()
    {
        _adj = new List<List<int>>();
    }

    public int VertexCount
    {
        get {return _adj.Count;}
    }

    public void AddVortex()
    {
        _adj.Add(new List<int>());
    }

    public void AddEdge(int v0, int v1)
    {
        _adj[v0].Add(v1);
        _adj[v1].Add(v0);
    }

    public IEnumerable<int> GetConnectedVertices(int vertex)
    {
        return _adj[vertex];
    }

}