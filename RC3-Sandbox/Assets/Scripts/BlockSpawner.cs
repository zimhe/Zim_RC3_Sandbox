using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour {
    

    [SerializeField ]
    private Transform _blockPrefab;
    [SerializeField]
    private int _countX = 10;
    [SerializeField]
    private int _countY = 10;
    [SerializeField]
    float _scaleX = 2.0f;
    [SerializeField]
    float _scaleY = 2.0f;

    private List<Rigidbody> _blocks;

    private List<Transform> blockObject;

    public Transform _camTarget;


    private void Awake()
    {
        setCamPivot();
        _blocks = new List<Rigidbody>();
        blockObject = new List<Transform>();

        for(int j=0; j<_countY; j++)
        {
            for(int i=0;i<_countX; i++)
            {
                var block = Instantiate(_blockPrefab, transform);
                block.localPosition = new Vector3(i * _scaleX, 0, j * _scaleY);

                blockObject.Add(block);
                _blocks.Add(block.GetComponent<Rigidbody>());
            }
        }

        CreateJoints();
    }

 
    private void Start()
    {
       
        fix(0); fix(_countX - 1); fix(_countX * _countY - 1); fix(_countX * _countY - _countX);
    }

    private void Update()
    {
        
    }

    private void CreateJoints()
    {
        for (int j=0;j<_countY; j++)
        {
            for (int i=0;i<_countX; i++)
            {
                var b0 = _blocks[ToIndex(i, j)];

                if (i > 0)
                {
                    Join(b0, _blocks[ToIndex(i-1, j)]);

                }
                if (j > 0)
                {
                    Join(b0, _blocks[ToIndex(i, j-1)]);
                }
            }
        }
    }


    private void Join(Rigidbody body0, Rigidbody body1)
    {
        var joint = body0.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = body1;
    }



    private int ToIndex(int x, int y)
    {
        return x + y * _countX;
    }


    private void fix(int index)
    {
        var handler = blockObject[index].GetComponent<ISelectionHandler>();

        handler.OnSelected();
    }


    void setCamPivot()
    {
        var _p = new Vector3(_countX * _scaleX * 0.5f, 0, _countY * _scaleY * 0.5f);

        _camTarget.position = _p;
    }

}
