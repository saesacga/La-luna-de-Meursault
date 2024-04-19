using System;
using UnityEngine;
using System.Collections;

public class ForestTP : MonoBehaviour
{
    [SerializeField] private Transform _playerPos;
    [SerializeField] private Transform _forestEnter;
    
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Salida"))
        {
            _playerPos.position = new Vector3(_forestEnter.position.x, _playerPos.position.y, _playerPos.position.z);   
        }
    }
}