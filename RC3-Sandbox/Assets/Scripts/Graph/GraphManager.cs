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

    private float  MaxHeight = 0;

    private SharedBlockMaterials _sharedBlockMaterials;

    private Transform[] _sources;
    private Graph _graph;
    private Transform[] _blocks;

    private Vector3[] _vertices;
   
    int[] depths;
    private IEnumerable<int> sources;

    private List<List<int>> _VertSets;
    private List<GameObject> lines;

    private Mesh mesh;

    private Material LineMaterial;

    [SerializeField] private float ScaleX = 1f;
    private float ScaleY;



    // Use this for initialization
    void Awake()
    {
        ScaleY = Mathf.Sqrt(Mathf.Pow(ScaleX, 2) - Mathf.Pow(ScaleX / 2, 2));
        _sources = MoveSource.GetSources();
    }

    void Start ()
	{
		//create graph
	    _graph = GraphFactory.CreateGrid(countX, countY);

	    depths = new int [_graph.VertexCount];

	    lines = new List<GameObject>();
        //create block
        CreateBlocks();
        //create mesh
	    CreateMesh();

	    GetMaxHeight();
	   
	    DrawLine();
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
       
        int index = 0;
        for (int i = 0; i < countX; i++)
        {
            for (int j = 0; j < countY; j++,index ++)
            {
                var obj = Instantiate(_blockPrefab, transform);
                var v1 = new Vector3(i * ScaleX, 0f,j * ScaleY);
                var v2 = new Vector3(i * ScaleX+ScaleX /2 ,0, j* ScaleY);

                if (j % 2 != 0)
                {
                    //if (x < xSize)
                    {
                        obj.localPosition = v1;
                    }
                }
                else
                {
                    obj.localPosition = v2;
                }
                _blocks[index] = obj;
            }
        }
       // print(_blocks.Length);
    }

	// Update is called once per frame
    void Update()
    {
        _sources = MoveSource.GetSources();
        sources = GetSourceIndices();
        GetMaxHeight();

        if (Input.GetKeyDown(KeyCode.Space) && sources.Count() != 0)
        {
            UpdateDepths();
            heightChangeForEachSource();
            Destroy(mesh);
            CreateMesh() ;
            removeLines(); 
            MaxHeight = 0;
            GetMaxHeight();
            DrawLine();
        }
    }

    void UpdateDepths()
    {
        GraphUtil.GetVertexDepths(_graph, sources, depths);

        for (int i = 0; i < _blocks.Length; i++)
        {
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

    void DrawLine()
    {
        LineMaterial =new Material(Shader.Find("Sprites/Default"));
        for (int v = 0, i = 0; i < countX; i++)
        {
            for (int j = 0; j < countY; j++,v++)
            {
                float LineWidth = 0.05f;

                if (j==countY -1&&i==countX -1)
                {
                    continue;
                }
                if (j == countY - 1 )
                {
                    GameObject DrawLineTop = new GameObject();
                    LineRenderer LineTop = DrawLineTop.AddComponent<LineRenderer>();
                    LineTop.positionCount = 2;
                    LineTop.SetPosition(0, _vertices[v]); LineTop.SetPosition(1, _vertices[v+countY]);

                    var L0 = _vertices[v].y;
                    var L1 = _vertices[v + countY].y;

                    Color C0 = Color.Lerp(Color.black, Color.red, L0 / MaxHeight);
                    Color C1 = Color.Lerp(Color.black, Color.red, L1 / MaxHeight);

                    LineTop.material = LineMaterial;
                    LineTop.SetWidth(LineWidth, LineWidth);

                    LineTop.SetColors(C0, C1);

                    DrawLineTop .name =  "Line " + i + j + "_" + 0;

                    lines.Add(DrawLineTop);
                    continue;
                }
                if (i == countX - 1)
                {
                    GameObject DrawLineRight = new GameObject();
                    LineRenderer LineRight = DrawLineRight.AddComponent<LineRenderer>();
                    LineRight.positionCount = 2;
                    LineRight .SetPosition(0, _vertices[v]); LineRight .SetPosition(1, _vertices[v + 1]);

                    var L0 = _vertices[v].y;
                    var L1 = _vertices[v + 1].y;

                    Color C0 = Color.Lerp(Color.black, Color.red, L0 / MaxHeight);
                    Color C1 = Color.Lerp(Color.black, Color.red, L1 / MaxHeight);

                    LineRight .material = LineMaterial;
                    LineRight .SetWidth(LineWidth, LineWidth);

                    LineRight .SetColors(C0, C1);

                    DrawLineRight .name = "Line " + i + j + "_" + 0;

                    lines.Add(DrawLineRight );
                    continue;
                }


                GameObject DrawLine0 = new GameObject();
                GameObject DrawLine1 = new GameObject();
                GameObject DrawLine2 = new GameObject();
                LineRenderer Line0 = DrawLine0.AddComponent<LineRenderer>();
                LineRenderer Line1 = DrawLine1.AddComponent<LineRenderer>();
                LineRenderer Line2 = DrawLine2.AddComponent<LineRenderer>();
                Line0.positionCount = 2; Line1.positionCount = 2; Line2.positionCount = 2;


                Line0.SetPosition(0, _vertices[v]); Line0.SetPosition(1, _vertices[v+1]);
                var L0_0 = _vertices[v].y; var L0_1 = _vertices[v + 1].y;

                Line1.SetPosition(0, _vertices[v]); Line1.SetPosition(1, _vertices[v + countY]);
                var L1_0 = _vertices[v].y; var L1_1 = _vertices[v + countY].y;

                float L2_0;
                float L2_1;

                if (j % 2 == 0)
                {
                    Line2.SetPosition(0, _vertices[v]); Line2.SetPosition(1, _vertices[v + countY + 1]);
                    L2_0 = _vertices[v].y; L2_1 = _vertices[v + countY+1].y;
                }
                else
                {
                    Line2.SetPosition(0, _vertices[v + 1]); Line2.SetPosition(1, _vertices[v + countY]);
                    L2_0 = _vertices[v+1].y; L2_1 = _vertices[v + countY].y;
                }

                Color C0_0 = Color.Lerp(Color.black, Color.red, L0_0 / MaxHeight);
                Color C0_1 = Color.Lerp(Color.black, Color.red, L0_1 / MaxHeight);
                Color C1_0 = Color.Lerp(Color.black, Color.red, L1_0 / MaxHeight);
                Color C1_1 = Color.Lerp(Color.black, Color.red, L1_1/ MaxHeight);
                Color C2_0= Color.Lerp(Color.black, Color.red, L2_0 / MaxHeight);
                Color C2_1 = Color.Lerp(Color.black, Color.red, L2_1 / MaxHeight);

                Material mat = LineMaterial;
                Line0.material = mat; Line1.material = mat; Line2.material = mat;

                Line0.SetWidth(LineWidth , LineWidth ); Line1.SetWidth(LineWidth , LineWidth ); Line2.SetWidth(LineWidth , LineWidth);
                Line0.SetColors(C0_0, C0_1); Line1.SetColors(C1_0, C1_1); Line2.SetColors(C2_0, C2_1);

                DrawLine0.name = "Line " + i+j+"_"+0;
                DrawLine1.name = "Line " + i+j+"_"+1;
                DrawLine2.name = "Line " + i+j+"_"+2;

                lines.Add(DrawLine0);
                lines.Add(DrawLine1);
                lines.Add(DrawLine2);
            }
        }
    }

    void removeLines()
    {
        foreach (GameObject L in lines)
        {
            Destroy(L);
        }
        lines.Clear();
    }

    void GetMaxHeight()
    {
        foreach (Transform Src in _sources )
        {
            float _h = Src.position.y;
            if (_h > MaxHeight)
            {
                MaxHeight = _h;
            }
        }
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
