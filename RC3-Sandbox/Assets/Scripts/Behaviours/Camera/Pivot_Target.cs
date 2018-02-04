using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot_Target : MonoBehaviour
{

    public Transform _target;

	// Use this for initialization
	void Start ()
	{

	    var p=_target.position;

	    transform.localPosition = p;

        print(p);
	}
	
	// Update is called once per frame
	void Update ()
	{


    }
}
