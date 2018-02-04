using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

static class GraphUtil
{
    public static int[] GetVertexDepths(Graph graph, IEnumerable <int> sources)
    {
        int[]depths=new int[graph.VertexCount]; 

        // initialize depths
        for (int i = 0; i < depths.Length; i++)
        {
            depths[i] = int.MaxValue;
        }

        var queue = new Queue<int>();

        // add source to queue
        foreach (int source in sources)
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
                if (d1<depths[v1])
                {
                    queue.Enqueue(v1);
                    depths[v1] = d1+1;
                }
            }
        }
        return depths;
    }
}