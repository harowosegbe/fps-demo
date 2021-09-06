using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponController : MonoBehaviour
{
    public enum WeaponType
    { 
        Automatic,
        Manual
    }

    [Tooltip("Weapon Type")]
    [SerializeField] private WeaponType ShootingType = WeaponType.Manual;

    [Tooltip("Name of the Weapon")]
    [SerializeField] private string WeaponName;

    [Tooltip("Bullet shooting position")]
    [SerializeField] private Transform ShootPoint;

    [Tooltip("Bullet shooting position")]
    [SerializeField] private Transform RecoilPosition;

    [SerializeField] private ProjectileBase Bullet;

    [Tooltip("Force that will push back the weapon after each shot")]
    [Range(0f, 2f)]
    public float recoilForce = 1f;
    public float recoilSharpness = 1f;
    public float recoilRestitutionSharpness = 10f;

    float m_LastTimeShot = Mathf.NegativeInfinity;
    private float ammo = 5;
    public float maximumAmmo = 100f;
    public float delayBetweenShots = 0.1f;
    public float maximumRecoilforce;
    private GameObject player;
    public Vector3 m_AccumulatedRecoil;
    public Vector3 m_WeaponRecoilLocalPosition;
    public Vector3 m_WeaponInitialPosition;
    public Vector3 muzzleVelocity;

    [SerializeField] private bool isActiveforShooting = false;
    [SerializeField] private bool isReadyforShooting = false;

    [Header("Audio Controller")]
    [Tooltip("Weapon Shoot Audio")]
    [SerializeField] private AudioClip shootingAudio;

    [Tooltip("Weapon Relaod Audio")]
    [SerializeField] private AudioClip reloadingAudio;

    [Tooltip("Weapon Swap Audio")]
    [SerializeField] private AudioClip swappingAudio;

    private AudioSource audioSource;
    private RectTransform AmmoUI;


    GameObject bullet;

    public bool fired;
    public bool isAttached = false;

    private bool mobileFired = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        m_WeaponRecoilLocalPosition = RecoilPosition.localPosition;
        player = this.gameObject;
        ammo = maximumAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveforShooting && ReadyToShoot)
        {
            switch (ShootingType)
            {
                case WeaponType.Manual:

                    if (/*Input.GetMouseButtonDown(0) ||*/ mobileFired)
                    {
                        fired = TryShoot();
                        mobileFired = false;
                    }
                    else
                    {
                        fired = false;
                    }

                    break;

                case WeaponType.Automatic:

                    if (/*Input.GetMouseButton(0) ||*/ mobileFired)
                    {
                        fired = TryShoot();
                        mobileFired = false;
                    }
                    else
                    {
                        fired = false;
                    }

                    break;

                default:
                    break;
            }
            
            if (fired)
            {
                m_AccumulatedRecoil += Vector3.back * recoilForce;
                m_AccumulatedRecoil = Vector3.ClampMagnitude(m_AccumulatedRecoil, maximumRecoilforce);
            }           
        }

    }

    public void ShootProjectile()
    {
        mobileFired = true;
    }

    bool TryShoot()
    {
        if (ammo >= 1f
            && m_LastTimeShot + delayBetweenShots < Time.time)
        {
            HandleShoot();
            ammo -= 1f;
            HandleAmmoUI();

            return true;
        }

        return false;
    }

    public void HandleAmmoUI()
    {
        if (AmmoUI.GetComponentInChildren<Slider>())
        {
            AmmoUI.GetComponentInChildren<Slider>().value = ammo;
            AmmoUI.GetComponentInChildren<TextMeshProUGUI>().text = "Ammo: " + ammo.ToString() + " / " + maximumAmmo.ToString();
        }
    }
    public void HandleShoot()
    {
        bullet = Instantiate(Bullet.gameObject, ShootPoint.transform.position, ShootPoint.transform.rotation);
        bullet.GetComponent<ProjectileBase>().Shoot(this);
        m_LastTimeShot = Time.time;
        PlayAudioOnce(shootingAudio);

        
    }

    public void PlayAudioOnce(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
    private void LateUpdate()
    {
        if (isAttached)
        {
            UpdateWeaponRecoil();
        }
    }

    void UpdateWeaponRecoil()
    {
        m_WeaponInitialPosition = player.GetComponent<PlayerController>().WeaponPositionOnPickup.localPosition;
        m_WeaponRecoilLocalPosition = transform.localPosition;

        // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
        if (m_WeaponRecoilLocalPosition.z >= m_AccumulatedRecoil.z * 0.99f)
        {
            m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, new Vector3(m_WeaponRecoilLocalPosition.x, m_WeaponRecoilLocalPosition.y,m_AccumulatedRecoil.z), recoilSharpness * Time.deltaTime);
        }
        // otherwise, move recoil position to make it recover towards its resting pose
        else
        {
            m_WeaponRecoilLocalPosition = Vector3.Lerp(m_WeaponRecoilLocalPosition, m_WeaponInitialPosition, recoilRestitutionSharpness * Time.deltaTime);
            m_AccumulatedRecoil = m_WeaponRecoilLocalPosition;
        }

        transform.localPosition = m_WeaponRecoilLocalPosition;
        //transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x,transform.position.y,transform.position.z - 0.5f), recoilSharpness * Time.deltaTime);

    }

    public bool CanShoot
    {
        get { return isActiveforShooting; } 
        set { isActiveforShooting = value; }
    }

    public bool ReadyToShoot
    {
        get { return isReadyforShooting; }
        set { isReadyforShooting = value; }
    }

    public GameObject PlayerGameobject
    {
        get { return player; }
        set { player = value; }
    }

    public RectTransform UIAmmo
    {
        set { AmmoUI = value; }
    }

    public string NameofWeapon
    {
        get { return WeaponName; }
    }

    public AudioClip RealoadingAudio
    {
        get { return reloadingAudio; }
    }

    public AudioClip SwappingAudio
    {
        get { return swappingAudio; }
    }

    public float CurrentAmmo
    {
        set { ammo = value; }
        get { return ammo; }
    }
}
