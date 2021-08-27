using System.Collections;
using UnityEngine;

public class ZombieRagDollModel : MonoBehaviour
{
    [SerializeField] private Rigidbody[] _bones;
    [SerializeField] private Vector3 _backDirection;
    [SerializeField] private float _backForce;
    [SerializeField] private float _upForce;
    [SerializeField] private float _fallDuration;
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _explosionForceRadius;
    [SerializeField] private Transform _explosionPosition;

    private void OnEnable()
    {
        StartCoroutine(Fall());

        foreach (var bone in _bones)
        {
            bone.AddExplosionForce(_explosionForce, _explosionPosition.position, _explosionForceRadius);
        }
    }

    private IEnumerator Fall()
    {
        float elapsedTime = 0;
        while (elapsedTime < _fallDuration)
        {
            elapsedTime += Time.deltaTime;

            foreach (var bone in _bones)
            {
//                bone.AddExplosionForce(_explosionForce, _explosionPosition.position, _explosionForceRadius);
                bone.velocity = Vector3.Lerp(bone.velocity, new Vector3(_backDirection.x*_backForce *Time.deltaTime, _backDirection.y * _upForce * Time.deltaTime, bone.velocity.z),0.5f);
            }
            yield return null;
        }

        
        foreach (var bone in _bones)
        {
            bone.useGravity = false;
            bone.isKinematic = true;
        }
        
    }

    private IEnumerator Jump()
    {

        yield return null;
    }
}
