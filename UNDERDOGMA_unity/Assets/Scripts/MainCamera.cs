using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class MainCamera : MonoBehaviour
{
    public GameObject _character;

    void Start()
    {
        _character = StageManager.Instance._character;
    }

    void LateUpdate()
    {
        Vector3 TargetPos = new Vector3(_character.transform.position.x * 0.5f, _character.transform.position.y * 0.5f, -10);
        transform.position = Vector3.Lerp(transform.position, TargetPos, Time.deltaTime * 1.5f);
    }
}
