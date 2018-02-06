using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    private Vector3[] _vertices;
    public GameObject _text;
    private GameObject [] Tags;
    int[] depths;
    private IEnumerable<int> sources;

    private List<List<int>> _VertSets;

    private Mesh mesh;



    // Use this for initialization
    void awake()
    {
        
    }

    void Start ()
	{
		//create graph
	    _graph = GraphFactory.CreateGrid(countX, countY);

	    depths = new int [_graph.VertexCount];
	    //create block
	    CreateBlocks();
        //create mesh
	    CreateMesh();
    }

    void CreateMesh()
    {
        _vertices = new Vector3[(countX) *(countY)];
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Mesh Created";

        Vector4[] tangents = new Vector4[_vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);


        for (int index = 0,  i = 0; i <=countX-1; i++)
        {
            for (int j = 0; j <= countY-1; j++,index ++)
            {
                _vertices[index] = _blocks[index].position;
            }
        }

        mesh.vertices = _vertices;
        mesh.tangents = tangents;

        int meshX = countX - 1;
        int meshY = countY - 1;
        int[] _triangles = new int[(meshX  )* (meshY) * 6];
        for (int ti = 0, vi = 0, x = 0; x < meshX ; x++, vi++)
        {
            for (int y = 0; y < meshY ; y++, ti += 6, vi++)
            {
                if (y % 2 != 0)
                {
                    {
                        _triangles[ti + 0] = vi;
                        _triangles[ti + 1] = vi + 1;
                        _triangles[ti + 2] = vi + countY;
                        _triangles[ti + 3] = vi + countY;
                        _triangles[ti + 4] = vi + 1;
                        _triangles[ti + 5] = vi + countY +1;
                    }
                }
                else
                {
                    _triangles[ti + 0] = vi;
                    _triangles[ti + 1] = vi + 1;
                    _triangles[ti + 2] = vi + countY + 1;
                    _triangles[ti + 3] = vi + countY + 1;
                    _triangles[ti + 4] = vi + countY;
                    _triangles[ti + 5] = vi; 
                }
            }
        }

        mesh.triangles = _triangles;
        mesh.RecalculateNormals();
    }

    void CreateBlocks()
    {
        _blocks = new Transform[_graph.VertexCount];
        Tags = new GameObject [_graph.VertexCount];
       
        int index = 0;
        for (int i = 0; i < countX; i++)
        {
            for (int j = 0; j < countY; j++,index ++)
            {
                var obj = Instantiate(_blockPrefab, transform);
                obj.localPosition = new Vector3(i, 0, j);

                var txt = Instantiate(_text,transform);
                txt.GetComponent< Transform >().localPosition = new Vector3(i, 0, j);

                _blocks[index] = obj;
                Tags[index] = txt;
            }
        }
       // print(_blocks.Length);
    }

	// Update is called once per frame
    void Update()
    {
        sources = GetSourceIndices();

        if (Input.GetKeyDown(KeyCode.Space) && sources.Count() != 0)
        {
            UpdateDepths();

            heightChangeForEachSource();
            Destroy(mesh);
            CreateMesh() ;
        }
    }

    void UpdateDepths()
    {
        GraphUtil.GetVertexDepths(_graph, sources, depths);

        for (int i = 0; i < _blocks.Length; i++)
        {
            Tags[i].GetComponent<TextMesh>().text = i + ":" + depths[i].ToString();
            //scaleChangeOnDepth(depths, i);
            resetHeight(i);
        }
    }
    void scaleChangeOnDepth(int[]_depths, int id)
    {
        var t = _depths[id] * 0.1f;

        if (t == 0)
        {
            t = 0.1f;
        }
        if (t > 1)
        {
            t = 1f;
        }
        _blocks[id].localScale = new Vector3(t, t, t);
    }

    void heightChangeOnDepth(int[] _depths, int id,float _maxHeight)
    {
        var t = _depths[id];

        float y = 0;

        if (t == 0)
        {
            y = _maxHeight;
        }
        if (t == 1)
        {
            y = _maxHeight * 0.974f;
        }
        if (t == 2)
        {
            y = _maxHeight * 0.857f;
        }
        if (t == 3)
        {
            y = _maxHeight * 0.5f;
        }
        if (t == 4)
        {
            y = _maxHeight * 0.143f;
        }
        if (t == 5)
        {
            y = _maxHeight * 0.026f;
        }
        if (t > 5)
        {
            y = 0f;
        }

        var x = _blocks[id].position.x;
        var z = _blocks[id].position.z;
        Vector3 vtr= new Vector3(x, y, z);

        _blocks[id].position = vtr;

        _vertices[id] = vtr;
    }

    void resetHeight(int id)
    {
        float y = 0;

        var x = _blocks[id].position.x;
        var z = _blocks[id].position.z;
        Vector3 vtr = new Vector3(x, y, z);

        _blocks[id].position = vtr;

        _vertices[id] = vtr;
    }

    void heightChangeForEachSource()
    {
        foreach (var src in _sources)
        {
            var Set = GetClosestVertexSet(src.position);
            foreach (int ID in Set)
            {
                var h = src.position.y;
                heightChangeOnDepth(depths, ID, h);
            }
        }
    }

    private IEnumerable<int> GetSourceIndices()
    {
        foreach (var src in _sources)
        {
            var v = GetClosestVertex(src.position);

            yield return v;
        }
    }

    int GetClosestVertex(Vector3 point)
    {
        int minVert = -1;
        float minDist = float.MaxValue;

        for (int i = 0; i < _blocks.Length; i++)
        {
            Vector3 d0 = new Vector3(_blocks[i].position.x, 0, _blocks[i].position.z);

            Vector3 d = d0 - point;
            var m = d.magnitude;
          
            if (m < minDist)
            {
                minVert = i;
                minDist = m;
            }
        }
        return minVert;
    }
    List< int > GetClosestVertexSet(Vector3 point)
    {
        List<int> vertSet = new List<int>();

        var h = point.y;

        for (int i = 0; i < _blocks.Length; i++)
        {
            Vector3 d0 = new Vector3(_blocks[i].position.x, 0, _blocks[i].position.z);

            Vector3 d = d0 - point;
            var m = d.magnitude;

            var Range = Mathf.Sqrt(Mathf.Pow(m,2) - Mathf.Pow(h,2));

            if (Range < 5)
            {
                vertSet.Add(i);
            }
        }
        return vertSet;
    }

    private void OnDrawGizmos()
    {
        if (_vertices == null)
        {
            return;

        }
        Gizmos.color = Color.black;
        for (int i = 0; i < _vertices.Length; i++)
        {
            Gizmos.DrawSphere(_vertices[i], 0.1f);
        }
    }
}
