using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public static class GraphFactory

{
    public static Graph CreateGrid(int countX, int countY)
    {
        var g = new Graph();

        int n = countX * countY;

        for (int i = 0; i < n; i++)
        {
            g.AddVortex();
        }
        for (int i = 0; i < countY; i++)
        {
            for (int j = 0; j < countX; j++)
            {
                var id0 = j + i * countX;
                //
                if (i > 0)
                {
                    int id1 = j + (i - 1) * countX;

                    g.AddEdge(id0, id1);
                }
                if (j > 0)
                {
                    int id1 = j - 1 + i * countX;

                    g.AddEdge(id0, id1);
                }
            }
        }
        return g;
    }
}
