using System;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Weapon_Manager : MonoBehaviour
{
	
	#region GlobalRefrence
	
	private static Weapon_Manager _localInstance;
	public static Weapon_Manager Instance
	{
		get
		{
			if (_localInstance == null)
				_localInstance = GameObject.FindObjectOfType<Weapon_Manager>();
			return _localInstance;
		}
	}
	
	#endregion
	
	
	public GameObject[] weapons;
	public GameObject equiped;
	public GameObject buy;

	[Header ("Weapons Stats")]
	[Space(5)]
	public Slider damage;
	public Slider grip;
	public Slider range;
	[Space(5)]
	public int[] damageStat;
	public int[] gripStat;
	public int[] rangeStat;
	[Space(5)]
	public Text[] specsValue;
	public Text weaponsNameText;
	public Text weaponNumber; 
	[Space(5)]
	public string[] weaponsName;
	
	public int[] weaponPrices;
	public Text weaponPrice;
	public GameObject weaponPriceParent;

	public GameObject notEnoughCash;
	public GameObject parentObject;

	private int currentWeapon = 0;
	public GameObject[] weaponsInnerOutline;
	public int weaponsbought;


	private void Awake()
	{
		
		PlayerPrefs.SetString("WeaponIsUnlock" + 0, "Unlocked");
		currentWeapon = PlayerPrefs.GetInt ("Weapon",0);
		Debug.Log(PlayerPrefs.GetInt("WeaponsBought"));
		
		
		notEnoughCash.SetActive(false);
		UpdateStatus();
		
	}

	public void CustomSelect(int index)
	{
		currentWeapon = index;
		UpdateStatus();
		Setting.Instance.ClickAudio(1);
	}
	
	//---------------------------------------------------------------------------------------------------------------//
	private void UpdateStatus()
	{
		
		if(PlayerPrefs.HasKey("WeaponIsUnlock" + currentWeapon))
		{
			
			equiped.gameObject.SetActive (true);
			buy.gameObject.SetActive (false);
			weaponPriceParent.SetActive(false);
			
		}
		else
		{
			
			buy.gameObject.SetActive (true);
			weaponPriceParent.SetActive(true);
			weaponPrice.text = weaponPrices[currentWeapon].ToString();
			equiped.gameObject.SetActive (false);
			
		}
		
		damage.value = damageStat [currentWeapon];
		grip.value = gripStat [currentWeapon];
		range.value = rangeStat [currentWeapon];

        weaponsNameText.text = weaponsName[currentWeapon];
        weaponNumber.text = (currentWeapon + 1).ToString();
		
		specsValue[0].text = damageStat [currentWeapon].ToString();
		specsValue[1].text = gripStat [currentWeapon].ToString();
		specsValue[2].text = rangeStat [currentWeapon].ToString();
		
		/*foreach (var item in weapons) {
			item.SetActive (false);
		}
		weapons [currentWeapon].SetActive (true);*/

		for (int i = 0; i < weapons.Length; i++)
		{
			weapons[i].SetActive (false);
		}
		weapons [currentWeapon].SetActive (true);
		
		/*foreach (var item in weaponsInnerOutline) {
			item.SetActive (false);
		}
		weaponsInnerOutline[currentWeapon].SetActive(true);
		*/

		for (int i = 0; i < weaponsInnerOutline.Length; i++)
		{
			weaponsInnerOutline[i].SetActive(false);
		}
		weaponsInnerOutline[currentWeapon].SetActive(true);

	}
	
	
	public void BuyWeapon()
	{
		if(PlayerPrefs.GetInt("Cash") >= weaponPrices[currentWeapon])
		{
			PlayerPrefs.SetInt ("Cash", PlayerPrefs.GetInt ("Cash") - weaponPrices[currentWeapon]);
			PlayerPrefs.SetString ("WeaponIsUnlock" + currentWeapon, "Unlocked");
			UpdateStatus();
			MenuManager.Instance.CashShown();
			PlayerPrefs.SetInt("WeaponsBought", PlayerPrefs.GetInt("WeaponsBought")+1);
			Debug.Log(PlayerPrefs.GetInt("WeaponsBought"));
			if (PlayerPrefs.GetInt("WeaponsBought")==2)
			{
				RewardedMenu.instance.buyAllWeapon.gameObject.SetActive(false);
			}

		}
		else
		{
			notEnoughCash.SetActive (true);
			CloseParent();
		}
	}
	
	public void Equipped()
	{

		PlayerPrefs.SetInt ("Weapon", currentWeapon);
		if (!MenuManager.Instance.isWeaponClick)
		{
			MenuManager.Instance.LoadLevel(2);
		}
		
		CloseParent();
		Setting.Instance.ClickAudio(1);
		Analyticsmanager.instance.GunsTrackingMenu(currentWeapon);
		AdsController.Instance.HideBanner();

	}
	public void CloseParent()
	{
		parentObject.SetActive(false);
	}
	public void OpenParent()
	{
		parentObject.SetActive(true);
	}
	public void PurchaseAllWeapons()
	{
		for (int i = 0; i < 3; i++) {
			PlayerPrefs.SetInt ("WeaponIsUnlock" + i, 4);
		    PlayerPrefs.SetString("AllWeaponsPurchased","Done");
		}
		UpdateStatus();
	}
}

