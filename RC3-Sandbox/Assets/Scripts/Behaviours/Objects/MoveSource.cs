using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Types;

public class MoveSource : MonoBehaviour
{
    [SerializeField] private Transform[] Sources = new Transform[2];

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
        
    }
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    pivot.position = Vector3 .Lerp( pivot .position ,Sources[sourceID].position,0.1f);

	    var x = Sources[sourceID].rotation.eulerAngles.x;

	    var z = Sources[sourceID].rotation.eulerAngles.z;

	    var y = pivot.rotation.eulerAngles.y;

	    Sources[sourceID].rotation = Quaternion.Euler(x, y, z);
	    changeSource();
	    moveSource();
	    SourceLook();
	   
	}

    void changeSource()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (sourceID < Sources.Length-1)
            {
                sourceID++;
            }
            else if (sourceID ==Sources .Length-1)
            {
                sourceID = 0;
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
        for (int i = 0; i < Sources.Length; i++)
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

    void LateUpdate()
    {
        //rotateSource();
    }

    void rotateSource()
    {
        _rotation = Sources[sourceID].rotation.eulerAngles;
        if (Input.GetMouseButton(1))
        {
            _rotation.y += Input.GetAxis("Mouse X") * _sensitivity;
            _rotation.x = Sources[sourceID].rotation.eulerAngles.x;
        }

        var q = Quaternion.Euler(_rotation.x, _rotation.y, 0.0f);
        Sources [sourceID ].rotation = Quaternion.Lerp(Sources [sourceID ].rotation, q, Time.deltaTime * _stiffness);
    }
}
