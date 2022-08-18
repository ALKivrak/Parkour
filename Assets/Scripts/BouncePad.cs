using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    struct BouncePadTarget
    {
        public float ContactTime;
        public Vector3 contactVelocity;
    }

    [SerializeField]
    float launchDelay = 0.1f;

    [SerializeField]
    float launchForce = 10f;

    [SerializeField]
    ForceMode launchMode = ForceMode.Impulse;

    [SerializeField]
    float ImpactVelocityScale = 0.05f;

    [SerializeField]
    float MaxImpactVelocityScale = 2f;

    Dictionary<Rigidbody, BouncePadTarget> Targets = new Dictionary<Rigidbody, BouncePadTarget>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    List<Rigidbody> TargetsToClear = new List<Rigidbody>();
    private void FixedUpdate()
    {
        float thresholdTime = Time.timeSinceLevelLoad - launchDelay;
        foreach (var kvp in Targets)
        {
            if(kvp.Value.ContactTime >= thresholdTime)
            {
                Launch(kvp.Key, kvp.Value.contactVelocity);
                TargetsToClear.Add(kvp.Key);
            }
        }

        foreach(var target in TargetsToClear)
            Targets.Remove(target);
        TargetsToClear.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb;
        if(collision.gameObject.TryGetComponent<Rigidbody>(out rb))
        {
            Targets[rb] = new BouncePadTarget() { ContactTime = Time.timeSinceLevelLoad,
                                                  contactVelocity = collision.relativeVelocity };
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        
    }

    void Launch(Rigidbody targetRB, Vector3 contactVelocity)
    {
        Vector3 launchVector = transform.up;

        float contactProjection = Vector3.Dot(contactVelocity, transform.up);
        if(contactProjection < 0)
        {
          launchVector *= Mathf.Min(MaxImpactVelocityScale, 1f + Mathf.Abs(contactProjection * ImpactVelocityScale));
        }
        targetRB.AddForce(transform.up * launchForce, launchMode);
    }
}
