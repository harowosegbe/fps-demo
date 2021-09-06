using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Health : MonoBehaviour
{
    [Tooltip("Maximum Health Allocated")]
    [SerializeField] private int maximumHealth;

    [Tooltip("VFX to play when exploding")]
    [SerializeField] private GameObject ExplosionVFX;

    [Tooltip("VFX to play during impact")]
    [SerializeField] private GameObject ImpactVFX;

    private EnemyController enemyController;
    private int currentHealth;
    public float impactVFXLifetime = 3f;

    private GameObject[] enemyObjects;
    private Collider[] enemyCollider;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maximumHealth;
        enemyController = GetComponent<EnemyController>();
        enemyObjects = enemyController.RenderedObjects;
        enemyCollider = enemyController.ActiveColliders;
    }

    // Update is called once per frame

    public void ReduceHealth(int bulletValue, Vector3 hitpoint)
    {
        if(currentHealth > 0)
        {
            currentHealth -= bulletValue;
            enemyController.RecoilEnemy(hitpoint);

            GameObject _impactVFXInstance = Instantiate(ImpactVFX, hitpoint, Quaternion.identity);

            if (impactVFXLifetime > 0)
            {
                Destroy(_impactVFXInstance.gameObject, impactVFXLifetime);
            }
        }
        else
        {
            enemyController.RecoilEnemyDead(hitpoint);
           
            GameObject impactVFXInstance = Instantiate(ExplosionVFX, hitpoint, Quaternion.identity);

            if (impactVFXLifetime > 0)
            {
                Destroy(impactVFXInstance.gameObject, impactVFXLifetime);
            }


            enemyController.SetRigidBodyisKinematic.isKinematic = true;

            StartCoroutine(ShowHideObject(3f));

            enemyController.SetRigidBodyisKinematic.isKinematic = false;
            enemyController.SetRigidBodyisKinematic.WakeUp();
        }
               
    }

    IEnumerator ShowHideObject(float timeinseconds)
    {
        for (int i = 0; i < enemyObjects.Length; i++)
        {
            enemyObjects[i].SetActive(false);
        }        

        for (int i = 0; i < enemyCollider.Length; i++)
        {
            enemyCollider[i].enabled = false;
        }

        yield return new WaitForSeconds(timeinseconds);

        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector3(transform.position.x, 1, transform.position.z);

        for (int i = 0; i < enemyObjects.Length; i++)
        {
            enemyObjects[i].SetActive(true);
        }

        for (int i = 0; i < enemyCollider.Length; i++)
        {
            enemyCollider[i].enabled = true;
        }

        currentHealth = maximumHealth;
    }
}
