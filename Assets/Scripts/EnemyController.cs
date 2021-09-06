using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    [Tooltip("Force that will push player when shot")]
    [Range(0f, 2f)]
    [SerializeField] private float recoilForce;

    [Tooltip("Recoil Radius")]
    [Range(0f, 10f)]
    [SerializeField] private float recoilRadius;

    private float maximumRecoilforce = 0.5f;

    [Tooltip("How sharp is the recoil")]
    [SerializeField] private float recoilSharpness;

    public Vector3 m_AccumulatedRecoil;
    public Vector3 m_WeaponRecoilLocalPosition;
    private Rigidbody rb;

    [Tooltip("Actual Rendered Objects")]
    [SerializeField] private GameObject[] enemyObjects;

    [Tooltip("Active Colliders")]
    [SerializeField] private Collider[] enemyColliders;
    // Start is called before the first frame update
    void Start()
    {
        m_WeaponRecoilLocalPosition = transform.localPosition;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        //UpdateRecoil();
    }
    public void RecoilEnemy(Vector3 hitpoint)
    {
        rb.AddExplosionForce(recoilForce * 125f, hitpoint, recoilRadius);
        Debug.Log("Recoiled Enemy");
    }

    public void RecoilEnemyDead(Vector3 hitpoint)
    {
        rb.AddExplosionForce(recoilForce * 300f, hitpoint, recoilRadius);
        Debug.Log("Recoiled Enemy");
    }

    public Rigidbody SetRigidBodyisKinematic
    {
        set { rb = value; }
        get { return rb; }
        
    }
    public GameObject[] RenderedObjects
    {
        get { return enemyObjects; }
    }

    public Collider[] ActiveColliders
    {
        get { return enemyColliders; }
    }
}
