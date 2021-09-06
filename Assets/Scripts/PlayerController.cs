using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Position for Weapon after Pickup")]
    [SerializeField] private Transform WeaponPickUpPoint;

    [Tooltip("Movement Speed of the Kart")]
    [SerializeField] private float MovementSpeed = 2f;

    [Header("Section for Mouse Look Control")]
    [Tooltip("Player Camera")]
    [SerializeField] private Camera playerCamera;

    [Tooltip("Rotational Speed")]
    [SerializeField] private float rotationSpeed = 5f;

    [Tooltip("Rotational Multiplier")]
    [SerializeField] private float rotationMultiplier = 1f;

    [Tooltip("UI Text Reference to the Weapon")]
    [SerializeField] private TextMeshProUGUI[] WeaponNameUI;

    private float horizontal = 0;
    private float vertical = 0;
    private Vector3 playerPos = Vector3.zero;
    private Rigidbody playerRigidBody;
    private float m_CameraVerticalAngle = 0;

    [SerializeField] private List<WeaponController> pickedWeapons;

    [SerializeField] private List<string> pickedWeaponsNames;

    [SerializeField] private FixedJoystick mobileInput;
    [SerializeField] private VariableJoystick mobileInputRot;
    [SerializeField] private GameObject pauseScreen;

    public TextMeshProUGUI weaponListText;

    private int currentWeaponIndex;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();

        weaponListText.text = "Weapon List: " + pickedWeaponsNames.Count;
        pauseScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();

        //Enable for PC
        /*WeaponSwitching();
        WeaponReloading();*/
    }


    private void FixedUpdate()
    {
        Vector3 localPositionOffset = playerPos * Time.fixedDeltaTime * MovementSpeed;
        Vector3 newPosition = playerRigidBody.position + transform.TransformDirection(localPositionOffset);
        playerRigidBody.MovePosition(newPosition);
        //playerRigidBody.MovePosition(transform.localPosition + playerPos * Time.fixedDeltaTime * MovementSpeed);
    }

    public Transform WeaponPosition()
    {
        return WeaponPickUpPoint;
    }

    public List<WeaponController> SetPickedWeapons
    {
        set { pickedWeapons = value; }
        get { return pickedWeapons; }
    }

    public List<string> SetPickedWeaponsNames
    {
        set { pickedWeaponsNames = value; }
        get { return pickedWeaponsNames; }
    }

    public Transform WeaponPositionOnPickup
    {
        get { return WeaponPickUpPoint; }
    }

    public int CurrentWeaponIndex
    {
        set { currentWeaponIndex = value; }
    }

    private void PlayerMovement()
    {
        //PC Input
        /*horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");*/

        //Mobile Input using Free Joystick
        horizontal = mobileInput.Horizontal;
        vertical = mobileInput.Vertical;

        playerPos = new Vector3(horizontal, 0, vertical);

        //Debug.Log("Horizontal: "+ mobileInput.Horizontal+ " Vertical: " + mobileInput.Vertical);
        
        //Cursor.lockState = CursorLockMode.Locked;
        // horizontal character rotation
        {
            // rotate the transform with the input speed around its local Y axis
            transform.Rotate(new Vector3(0f, (GetLookInputsHorizontal() * rotationSpeed * rotationMultiplier), 0f), Space.Self);
        }

        // vertical camera rotation
        {
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += GetLookInputsVertical() * rotationSpeed * rotationMultiplier;

            // limit the camera's vertical angle to min/max
            m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            //playerCamera.transform.localEulerAngles = new Vector3(m_CameraVerticalAngle, 0, 0);
        }
    }

    public void Shoot()
    {
        if(pickedWeapons.Count > 0)
        {
            pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().ShootProjectile();
        }
    }
    public void SwitchWeapon ()
    {
        WeaponSwitching();
    }

    public void ReloadWeapon()
    {
        WeaponReloading();
    }
    private void WeaponSwitching()
    {
        int currentNum = currentWeaponIndex + 1;

        if (currentNum > pickedWeapons.Count)
        {
            currentNum = 1;
        }

        #region PC Weapon Switcher
        /*if(pickedWeapons.Count > 1)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                for (int i = 0; i < pickedWeapons.Count; i++)
                {
                    if (i + 1 == 1)
                    {
                        currentWeaponIndex = 1;
                        pickedWeapons[i].gameObject.SetActive(true);
                        pickedWeapons[i].gameObject.GetComponent<WeaponController>().CanShoot = true;
                        pickedWeapons[i].gameObject.GetComponent<WeaponController>().PlayAudioOnce(pickedWeapons[i].gameObject.GetComponent<WeaponController>().SwappingAudio);
                        pickedWeapons[i].gameObject.GetComponent<WeaponController>().HandleAmmoUI();

                        WeaponNameUI[i].text = (i + 1) + "     " + pickedWeaponsNames[i];
                    }
                    else
                    {
                        pickedWeapons[i].gameObject.SetActive(false);
                        pickedWeapons[i].gameObject.GetComponent<WeaponController>().CanShoot = false;
                        WeaponNameUI[i].text = (i + 1) + "     " + pickedWeaponsNames[i];
                    }
                   
                }
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                for (int i = 0; i < pickedWeapons.Count; i++)
                {
                    if (i + 1 == 2)
                    {
                        currentWeaponIndex = 2;
                        pickedWeapons[i].gameObject.SetActive(true);
                        pickedWeapons[i].gameObject.GetComponent<WeaponController>().CanShoot = true;
                        pickedWeapons[i].gameObject.GetComponent<WeaponController>().PlayAudioOnce(pickedWeapons[i].gameObject.GetComponent<WeaponController>().SwappingAudio);
                        pickedWeapons[i].gameObject.GetComponent<WeaponController>().HandleAmmoUI();

                        WeaponNameUI[pickedWeapons.Count - 1 - i].text = (i + 1) + "     " + pickedWeaponsNames[i];
                    }
                    else
                    {
                        pickedWeapons[i].gameObject.SetActive(false);
                        pickedWeapons[i].gameObject.GetComponent<WeaponController>().CanShoot = false;

                        WeaponNameUI[pickedWeapons.Count - 1 - i].text = (i + 1) + "     " + pickedWeaponsNames[i];
                    }

                }
            }
            else
            {

            }

        }*/

        #endregion


        //Mobile
        #region Mobile Weapon Switcher
        if (pickedWeapons.Count > 1)
        {
            for (int i = 0; i < pickedWeapons.Count; i++)
            {
                if (i + 1 == currentNum)
                {
                    currentWeaponIndex = currentNum;
                    pickedWeapons[i].gameObject.SetActive(true);
                    pickedWeapons[i].gameObject.GetComponent<WeaponController>().CanShoot = true;
                    pickedWeapons[i].gameObject.GetComponent<WeaponController>().PlayAudioOnce(pickedWeapons[i].gameObject.GetComponent<WeaponController>().SwappingAudio);
                    pickedWeapons[i].gameObject.GetComponent<WeaponController>().HandleAmmoUI();

                    WeaponNameUI[i].text = (i + 1) + "     " + pickedWeaponsNames[i];
                }
                else
                {
                    pickedWeapons[i].gameObject.SetActive(false);
                    pickedWeapons[i].gameObject.GetComponent<WeaponController>().CanShoot = false;
                    WeaponNameUI[i].text = (i + 1) + "     " + pickedWeaponsNames[i];
                }

            }
        }
        #endregion
    }

    private void WeaponReloading()
    {
        if(pickedWeapons.Count > 0)
        {
            #region PC Reload
            /*if (Input.GetKeyDown(KeyCode.R))
            {
                pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().CurrentAmmo += 50f;
                pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().CurrentAmmo = Mathf.Clamp(pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().CurrentAmmo,0, pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().maximumAmmo);
                pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().PlayAudioOnce(pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().RealoadingAudio);
                pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().HandleAmmoUI();
            }*/
            #endregion

            #region Mobile Reload

                pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().CurrentAmmo += 50f;
                pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().CurrentAmmo = Mathf.Clamp(pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().CurrentAmmo, 0, pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().maximumAmmo);
                pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().PlayAudioOnce(pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().RealoadingAudio);
                pickedWeapons[currentWeaponIndex - 1].gameObject.GetComponent<WeaponController>().HandleAmmoUI();

            #endregion
        }


    }

    public float GetLookInputsHorizontal()
    {
        //Section for PC
        //return Input.GetAxis("Mouse X");

        //Section for Mobile
        return mobileInputRot.Horizontal;
    }

    public float GetLookInputsVertical()
    {
        //Section for PC
        //return Input.GetAxis("Mouse Y");

        //Section for Mobile
        return mobileInputRot.Vertical;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }
}
