using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodySplat : MonoBehaviour
{
    [SerializeField] private CharacterJoint _joint;
    [SerializeField] private Rigidbody _body;

    public void SetJoint(Rigidbody connector)
    {
        _joint.connectedBody = connector;
    }

    public void SetGravityVelocity(float yVelocity)
    {
        _body.velocity = new Vector3(_body.velocity.x, yVelocity, _body.velocity.z);
    }
}