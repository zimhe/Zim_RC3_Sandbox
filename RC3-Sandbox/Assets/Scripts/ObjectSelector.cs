using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour {

    private Camera _camera;

	// Use this for initialization
	void Start ()
    {
        _camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                var handler = hit.transform.GetComponent<ISelectionHandler>();

                if (handler != null)
                {
                    if (handler.IsSelected)
                    {
                        handler.OnDeselected();
                    }
                    else
                    {
                        handler.OnSelected();
                    }
                }
            }
        }
	}
}
