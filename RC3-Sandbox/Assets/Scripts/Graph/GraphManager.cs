using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    [SerializeField]
    private Transform _blockPrefab;
    [SerializeField ]
    private int countX = 5;
    [SerializeField]
    private int countY = 5;

    private SharedBlockMaterials _sharedBlockMaterials;

    [SerializeField]
    private Transform[] _sources;
    private Graph _graph;
    private Transform[] _blocks;

	// Use this for initialization
	void Start ()
	{
		//create graph
	    _graph = GraphFactory.CreateGrid(countX, countY);
	    //create block
	    CreateBlocks();
	    //c
	}

    void CreateBlocks()
    {
        _blocks = new Transform[_graph.VertexCount];

        //print(_graph.VertexCount);
        int index = 0;
        for (int i = 0; i < countX; i++)
        {
            for (int j = 0; j < countY; j++,index ++)
            {
                var obj = Instantiate(_blockPrefab, transform);
                obj.localPosition = new Vector3(i, 0, j);

                _blocks[index] = obj;
            }
        }
       // print(_blocks.Length);
    }

    private IEnumerable<int> GetSourceIndices()
    {
        foreach (var src in _sources)
        {
            var v = GetClosestVertex(src.position);

            yield return v;
        }
    }

	// Update is called once per frame
	void Update ()
	{
	    var sources = GetSourceIndices();

	    var depths = GraphUtil.GetVertexDepths(_graph, sources);

        for (int i = 0; i < _blocks.Length; i++)
	    {
	        var t = depths[i] * 0.1f;

	        if (t < 1)
	        {
	            _blocks[i].localScale = new Vector3(t, t, t);
            }
	        else
	        {
	            _blocks[i].localScale = new Vector3(1, 1, 1);

	        }
	       
	    }
	}

    int GetClosestVertex(Vector3 point)
    {
        int minVert = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < _blocks.Length; i++)
        {
            Vector3 d = _blocks[i].position - point;
        
            var m = d.magnitude;
          
            if (m < minDist)
            {
                minVert = i;
                minDist = m;
            }
        }
        return minVert;
    }
}
