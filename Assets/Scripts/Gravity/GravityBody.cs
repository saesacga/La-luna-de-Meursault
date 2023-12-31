using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBody : MonoBehaviour
{
    [SerializeField] private GravityAttractor _gravityAttractor;
    [SerializeField] private bool _customProperties;
    [SerializeField] private float _gravityMultiplier = 1f;
    [HideInInspector] public bool useCustomGravity;
    private Transform _transform;

    private void Start()
    {
        if (_customProperties == false) { GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation; }
        GetComponent<Rigidbody>().useGravity = false;
        this._transform = transform;
    }

    private void OnEnable()
    {
        useCustomGravity = true;
    }

    private void FixedUpdate()
    {
        if (useCustomGravity)
        {
            _gravityAttractor.GravityAttraction(this._transform, this._gravityMultiplier);
        }
    }
}
