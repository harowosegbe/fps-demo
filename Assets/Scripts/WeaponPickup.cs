using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class WeaponPickup : MonoBehaviour
{
    [Tooltip("Weapon Pickup Audio")]
    [SerializeField] private AudioClip pickUpAudio;

    [Tooltip("Ammo RectTransform")]
    [SerializeField] private RectTransform AmmoUI;

    [Tooltip("UI Text Reference to the Weapon")]
    [SerializeField] private TextMeshProUGUI[] WeaponNameUI;

    private WeaponController myWeaponController;

    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        myWeaponController = GetComponent<WeaponController>();
        audioSource = GetComponent<AudioSource>();
        myWeaponController.UIAmmo = AmmoUI;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {       
        if(other.transform.parent.CompareTag("Player"))
        {
            if (other.transform.gameObject.GetComponentInParent<PlayerController>())
            {
                other.transform.gameObject.GetComponentInParent<PlayerController>().SetPickedWeapons.Add(myWeaponController);
                transform.position = other.transform.gameObject.GetComponentInParent<PlayerController>().WeaponPosition().position;
                transform.rotation = other.transform.gameObject.GetComponentInParent<PlayerController>().WeaponPosition().rotation;
                transform.SetParent(other.transform.parent);
                myWeaponController.ReadyToShoot = true;
                myWeaponController.PlayerGameobject = other.transform.parent.gameObject;
                myWeaponController.isAttached = true;
                audioSource.PlayOneShot(pickUpAudio);
                other.transform.gameObject.GetComponentInParent<PlayerController>().SetPickedWeaponsNames.Add(myWeaponController.NameofWeapon);
                other.transform.gameObject.GetComponentInParent<PlayerController>().CurrentWeaponIndex = other.transform.gameObject.GetComponentInParent<PlayerController>().SetPickedWeaponsNames.Count;

                if (GetComponentInChildren<Canvas>())
                {
                    GetComponentInChildren<Canvas>().gameObject.SetActive(false);
                }

                if (other.transform.gameObject.GetComponentInParent<PlayerController>().SetPickedWeapons.Count > 1)
                {
                    int count = other.transform.gameObject.GetComponentInParent<PlayerController>().SetPickedWeapons.Count;

                    for (int i = 0; i < count - 1; i++)
                    {
                        other.transform.gameObject.GetComponentInParent<PlayerController>().SetPickedWeapons[i].gameObject.SetActive(false);
                        other.transform.gameObject.GetComponentInParent<PlayerController>().SetPickedWeapons[i].CanShoot = false;
                    }

                    myWeaponController.CanShoot = true;
                    HandleUI(other);
                }
                else
                {
                    myWeaponController.CanShoot = true;

                    HandleUI(other);
                }
            }

            GetComponentInChildren<BoxCollider>().enabled = false;
            
        }
    }

    private void HandleUI(Collider other)
    {
        int count = other.transform.gameObject.GetComponentInParent<PlayerController>().SetPickedWeaponsNames.Count;
        other.transform.gameObject.GetComponentInParent<PlayerController>().weaponListText.text = "Weapon List: " + count;

        if (WeaponNameUI.Length >= 1)
        {
            for (int i = 0; i < WeaponNameUI.Length; i++)
            {
                if(i + 1 <= count)
                {
                    WeaponNameUI[count - 1 - i].text = (i+1) +"     "+ other.transform.gameObject.GetComponentInParent<PlayerController>().SetPickedWeaponsNames[i];
                }
                else
                {
                    WeaponNameUI[i].text = "";
                }
                
            }
        }

        if (AmmoUI.GetComponentInChildren<Slider>())
        {
            AmmoUI.GetComponentInChildren<Slider>().value = myWeaponController.maximumAmmo;
            AmmoUI.GetComponentInChildren<TextMeshProUGUI>().text = "Ammo: " + myWeaponController.maximumAmmo.ToString() + " / " + myWeaponController.maximumAmmo.ToString();
        }
    }
}
