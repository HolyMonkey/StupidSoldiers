using System.Collections;
using UnityEngine;

public class ZombieRagDollModel : MonoBehaviour
{
    [SerializeField] private Rigidbody[] _bones;
    [SerializeField] private Vector3 _backDirection;
    [SerializeField] private float _backForce;
    [SerializeField] private float _upForce;
    [SerializeField] private float _fallDuration;

    private void OnEnable()
    {
        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        float elapsedTime = 0;
        while (elapsedTime < _fallDuration)
        {
            elapsedTime += Time.deltaTime;

            foreach (var bone in _bones)
            {
                bone.velocity = new Vector3(_backDirection.x*_backForce *Time.deltaTime, _backDirection.y * _upForce * Time.deltaTime, bone.velocity.z);
                yield return null;
            }
        }
    }

    private IEnumerator Jump()
    {

        yield return null;
    }
}
