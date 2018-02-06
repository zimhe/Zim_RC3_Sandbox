using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class GraphUtil
{
    public static void  GetVertexDepths(Graph graph, IEnumerable <int> sources, int [] _depths)
    {
        // initialize depths
        for (int i = 0; i < _depths.Length; i++)
        {
            _depths[i] = int.MaxValue;
        }

        var queue = new Queue<int>();

        // add source to queue
        foreach (int source in sources)
        {
            queue.Enqueue(source);
            _depths[source] = 0;
        }
        while (queue.Count > 0)
        {
            int v0 = queue.Dequeue();
            int d1 = _depths[v0]+1;

            foreach (int v1 in graph.GetConnectedVertices(v0))
            {
                if (d1<_depths[v1])
                {
                    queue.Enqueue(v1);
                    _depths[v1] = d1;
                }
            }
        }
    }

    public static void GetVertexDepthsIgnore(Graph graph, IEnumerable<int> sources,IEnumerable< int> ignore, int[] _depths)
    {
        // initialize depths
        for (int i = 0; i < _depths.Length; i++)
        {
            _depths[i] = int.MaxValue;
        }

        var queue = new Queue<int>();

        // add source to queue
        foreach (int source in sources)
        {
            queue.Enqueue(source);
            _depths[source] = 0;
        }

        foreach (int index in ignore)
        {
            _depths[index] = 0;
        }

        while (queue.Count > 0)
        {
            int v0 = queue.Dequeue();
            int d1 = _depths[v0] + 1;

            foreach (int v1 in graph.GetConnectedVertices(v0))
            {
                if (d1 < _depths[v1])
                {
                    queue.Enqueue(v1);
                    _depths[v1] = d1;
                }
            }
        }
    }



    public static int[] GetVertexDepths2Sources(Graph graph, IEnumerable<int> sources,int ID0,int ID1)
    {
        int[] depths = new int[graph.VertexCount];

        // initialize depths
        for (int i = 0; i < depths.Length; i++)
        {
            depths[i] = int.MaxValue;
        }

        var SourcesSelected = new int[2];
        SourcesSelected[0] = sources.ToArray()[ID0];
        SourcesSelected[1] = sources.ToArray()[ID1];

        var queue = new Queue<int>();

        // add source to queue
        foreach (int source in SourcesSelected)
        {
            queue.Enqueue(source);
            depths[source] = 0;
        }
        while (queue.Count > 0)
        {
            int v0 = queue.Dequeue();
            int d1 = depths[v0];

            foreach (int v1 in graph.GetConnectedVertices(v0))
            {
                if (d1 < depths[v1])
                {
                    queue.Enqueue(v1);
                    depths[v1] = d1 + 1;
                }
            }
        }
        return depths;
    }

    public static List<int>  GetShortestPathBetween2Points(Graph _g, IEnumerable< int> sources, int[]_depths,int start, int end)
    {
        var _sourceID = sources.ToArray();

        List<int> _blockID = new List<int>();

        var q = new Queue<int>();

        q.Enqueue(_sourceID [start]);

        if (q.Count > 0)
        {
            int V0 = q.Dequeue();
            var v = _g.GetConnectedVertices(V0);
            int _minDepth = int.MaxValue;

            if (V0 != _sourceID[end])
            {
                foreach (int V1 in v)
                {
                    if (_depths[V1] < _minDepth)
                    {
                        _minDepth = _depths[V1];
                    }
                }
                foreach (int V2 in v)
                {
                    if (_depths[V2] == _minDepth)
                    {
                        q.Enqueue(V2);
                        _blockID.Add(V2);
                    }
                }
            }
        }
        return _blockID;
    }
}