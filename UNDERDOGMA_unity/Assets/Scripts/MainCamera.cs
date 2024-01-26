using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;


public class MainCamera : MonoBehaviour
{
    public GameObject _character;
    private Vector3 originalTransform;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.2f;
    private float dampingSpeed = 1.0f;

    void Start()
    {
        _character = StageManager.Instance._character;
        originalTransform = transform.position;
    }

    // 호출하여 쉐이크 시작
    public void Shake(float duration)
    {
        shakeDuration = duration;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            transform.localPosition = originalTransform + UnityEngine.Random.insideUnitSphere * shakeMagnitude;

            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.position = originalTransform;
        }
    }
}
