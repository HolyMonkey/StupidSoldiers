using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenRagDoll : MonoBehaviour
{
    [SerializeField] private Rigidbody[] _allBones;

    private void OnEnable()
    {
        foreach(var bone in _allBones)
        {
            //bone.useGravity = false;
            bone.isKinematic = true;
        }                
    }

    public void EnableRagDoll()
    {
        foreach (var bone in _allBones)
        {
            //bone.useGravity = true;
            bone.isKinematic = false;
        }
    }

    public void DisableRagdoll()
    {
        foreach (var bone in _allBones)
        {
            //bone.useGravity = false;
            bone.isKinematic = true;
        }

    }
}