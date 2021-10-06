using System.Collections;
using UnityEngine;

public class EnemyRagDollModel : MonoBehaviour
{
    [SerializeField] private Rigidbody[] _allBones;
    [SerializeField] private Rigidbody[] _explosionBones;

    [SerializeField] private Rigidbody _body;

    [SerializeField] private float _fallDuration;
    [SerializeField] private float _yGravity;
    [SerializeField] private float _xVelocity;

    private void OnEnable()
    {
        foreach (var bone in _allBones)
        {
            bone.useGravity = false;
            bone.isKinematic = true;
        }
    }

    private IEnumerator SimulateGravity()
    {
        float yGravity = Random.Range(-5, _yGravity);      

        foreach (var bone in _explosionBones)
        {
            bone.AddExplosionForce(Random.Range(200, 300), new Vector3(_body.transform.position.x - Random.Range(1,4), _body.transform.position.y + Random.Range(0,3), _body.transform.position.z), 15);
        }

        yield return new WaitForSeconds(Random.Range(0.05f,0.25f));

        float elapsedTime = 0;
        while (elapsedTime < _fallDuration)
        {
            elapsedTime += Time.deltaTime;
            _body.velocity = Vector3.Lerp(_body.velocity, new Vector3(_body.velocity.x, yGravity, 0),0.06f);
            yield return null;
        }

        foreach (var bone in _allBones)
        {
            bone.velocity = new Vector3(0, 0, 0);
        }
    }

    public void StartFall()
    {
        foreach (var bone in _allBones)
        {
            bone.isKinematic = false;
            bone.useGravity = true;
            bone.velocity = new Vector3(0, 0, 0);
        }
        StartCoroutine(SimulateGravity());
    }

    public void SetCollisionWithBulletPoint(Vector3 point)
    {
    }
}
