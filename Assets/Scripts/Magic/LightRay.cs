using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using UnityEngine;
using Collision = UnityEngine.Collision;

public class LightRay : MonoBehaviour
{
    private int _nextPosCount;
    [SerializeField] private Collider _invisibleWall;
    [SerializeField] private Animator _cinemachineBlend;
    [SerializeField] private bool _moving;
    [HideInInspector] public int _starsCount;
    [HideInInspector] public int _starsInPosition;
    public int _starsRequired;
    private bool _alreadyExecuted;

    private void Update()
    {
        if (_starsRequired == _starsInPosition && _alreadyExecuted == false)
        {
            GetComponent<Animator>().SetBool("activateRay", true);
            _invisibleWall.isTrigger = true;
            _alreadyExecuted = true;
        }

        if (_moving)
        {
            _cinemachineBlend.Play("LightBeamCam");
            ActionMapReference.cinemachineInputProvider.enabled = false;
        }
        else
        {
            _cinemachineBlend.Play("POVCam");
            ActionMapReference.cinemachineInputProvider.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            _nextPosCount++;
            GetComponent<Animator>().SetInteger("nextPosition", _nextPosCount);
            Debug.Log(_nextPosCount);            
        }
    }
}
