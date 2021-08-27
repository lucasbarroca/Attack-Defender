using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Transform target;
    public float speed = 1f;
    public float hitRange = 1f;

    bool _targetHitted;
    public bool targetHitted
    {
        get { return _targetHitted; }
        set
        {
            _targetHitted = value;

            if (value)
            {
                OnTargetHit();
            }
        }
    }

    // Vector3 startPosition;
    float distance;
    Vector3 destination;
    float shootTime;
    float cadence = 1f;

    // Start is called before the first frame update
    void Start()
    {
        // startPosition = transform.position;
        // distance = GameConst.getRange(transform.position, target.position);
        destination = target.position;
        destination.y = transform.position.y;

        // Cadence configuration
        shootTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetHitted)
        {
            // Move projectile
            // transform.Translate(Vector3.forward * speed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, destination, .2f * Time.deltaTime);

            // Cadence
            var elapsedTime = Time.time - shootTime;
            destination.y = Mathf.Clamp(destination.y - elapsedTime / 25f * cadence, destination.y - .5f, transform.position.y);
            // if (GameConst.getRange(startPosition, transform.position) >= distance - hitRange)
            // {
            //     targetHitted = true;
            // }
        }
    }

    void OnTargetHit()
    {
        // Destroy itself
        Destroy(gameObject);
    }

    public void Shoot()
    {

    }

    void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, hitRange);
    }
}
