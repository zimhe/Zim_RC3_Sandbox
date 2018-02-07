using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking.Types;

public class MoveSource : MonoBehaviour
{
    [SerializeField] private Transform SourcePrefab;

    private static List<Transform> Sources;

    [SerializeField] private float MoveSpeed = 0.3f;

    [SerializeField]private Transform pivot;

    [SerializeField]
    private float _sensitivity = 5.0f;

    [SerializeField]
    private float _stiffness = 10.0f;

    private Vector3 _rotation;
    // Use this for initialization
    int sourceID = 0;
    void Awake()
    {
        Sources = new List<Transform>();

        Transform Source0 = Instantiate(SourcePrefab, transform);

        Sources.Add(Source0);
    }
	void Start ()
	{
	    

	}
	
	// Update is called once per frame
	void Update ()
	{
	    pivot.position = Vector3 .Lerp( pivot .position ,Sources[sourceID].position,0.1f);

	    var x = Sources [sourceID].rotation.eulerAngles.x;

	    var z = Sources[sourceID].rotation.eulerAngles.z;

	    var y = pivot.rotation.eulerAngles.y;

	    Sources[sourceID].rotation = Quaternion.Euler(x, y, z);
	    changeSource();
	    moveSource();
	    SourceLook();
	    addSource();
	    deletSource();

	}

    void changeSource()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (sourceID < Sources.Count-1)
            {
                sourceID++;
            }
            else if (sourceID ==Sources .Count -1)
            {
                sourceID = 0;
            }
        }
    }

    void addSource()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            int L = Sources.Count ;
            var S = Instantiate(Sources[sourceID], transform);
            Sources.Add(S);
        }
    }

    void deletSource()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (Sources.Count > 1)
            {
                Destroy(Sources[sourceID].gameObject);
                Sources.Remove(Sources[sourceID]);
                if (sourceID > 0)
                {
                    sourceID = sourceID - 1;
                }
                else
                {
                    sourceID = 0 ;
                }
             
            }
        }
    }

    void moveSource()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Sources[sourceID].transform.Translate(Vector3.forward * MoveSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Sources[sourceID].transform.Translate(Vector3.back * MoveSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Sources[sourceID].transform.Translate(Vector3.left * MoveSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Sources[sourceID].transform.Translate(Vector3.right * MoveSpeed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Sources[sourceID].transform.Translate(Vector3.up * MoveSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            Sources[sourceID].transform.Translate(Vector3.down * MoveSpeed);
        }
    }

    void SourceLook()
    {
        for (int i = 0; i < Sources.Count; i++)
        {
            if (i == sourceID)
            {
                Sources[i].GetComponent<MeshRenderer>().material.color = Color.red;
                Sources[i].localScale = new Vector3(1, 1, 1);
            }
            else
            {
                Sources[i].GetComponent<MeshRenderer>().material.color = Color.black;
                Sources[i].localScale = new Vector3(0.5f,0.5f,0.5f);
            }
        }
    }

    public static Transform [] GetSources()
    {
        return Sources.ToArray();
    }

    


}
