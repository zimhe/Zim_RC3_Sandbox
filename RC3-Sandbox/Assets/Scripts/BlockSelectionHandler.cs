using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSelectionHandler : MonoBehaviour,ISelectionHandler
{
    [SerializeField]
    private SharedBlockMeshes _meshes;

    [SerializeField]
    private SharedBlockMaterials _materials;

    private Rigidbody _rigidbody;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    private float _scaleSelected = 1.0f;

    private float _scaleDefault = 1.0f;


    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
       
        OnDeselected();
    }


    public bool IsSelected
    {
        
        get { return _rigidbody.isKinematic; }
    }

    public void OnDeselected()
    {
        _rigidbody.isKinematic = false;
        _meshFilter.mesh = _meshes.DefaultMesh;
        _meshRenderer.material = _materials.DefaultMaterial;
        transform.localScale = new Vector3(_scaleDefault, _scaleDefault, _scaleDefault);
    }

    public void OnSelected()
    {
        _rigidbody.isKinematic = true;
        _meshFilter.mesh = _meshes.SelectedMesh;
        _meshRenderer.material = _materials.SelectedMaterial;
        transform.localScale = new Vector3(_scaleSelected, _scaleSelected, _scaleSelected);
    }

    // Use this for initialization
   

}
