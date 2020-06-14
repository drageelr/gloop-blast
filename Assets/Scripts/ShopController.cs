using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour {

    //Madness num = 0
    //Rainbow num = 1
    //Slicer num = 2

    //Main Stuff
    public GameObject shopObj;
    public Text dispCoins;

    //Shop Page
    public GameObject pgShop;
    public Text[] shopCount = new Text[3];
    private int[] puPrice = new int[] {100, 200, 150};

    //Buy Page
    public GameObject pgBuy;

    //Upgrade Page
    public GameObject pgUpgrade;
    public RectTransform[] upgradePUsGauge = new RectTransform[3];
    public Text[] upgradePUsPrice = new Text[3];
    private float[] upgradesPosX = new float[] 
    {
        25.8f, 28.39f, 30.68f, 33.12f, 35.41f, 37.65f, 40.3f, 42.6f, 44.6f, 46.9f, 49.85f 
    };
    private float[] upgradesWidth = new float[] 
    {
        5.4f, 9.07f, 12.32f, 15.78f, 19.2f, 22.3f, 26.1f, 29.2f, 32.2f, 35.45f, 39.57f
    };
    private List<int[]> upgradesPrice = new List<int[]>();
    private int[] upgradesPriceMadness = new int[]
    {
        50, 100, 150, 250, 500, 1000, 2000, 3250, 5000, 6250
    };

    private int[] upgradesPriceRainbow = new int[] 
    {
        100, 150, 250, 500, 850, 1250, 2500, 4000, 7000, 8500,
    };

    private int[] upgradesPriceSlicer = new int[]
    {
        75, 125, 200, 400, 750, 1500, 2250, 3500, 6000, 7500
    };



    void Awake()
    {
        upgradesPrice = new List<int[]>();
        upgradesPrice.Add(upgradesPriceMadness);
        upgradesPrice.Add(upgradesPriceRainbow);
        upgradesPrice.Add(upgradesPriceSlicer);
    }

    private void UpdateCoins()
    {
        dispCoins.text = GameData.current.coins.ToString();
    }

    private void SaveData()
    {
        SaveLoad.Save();
    }

    //UPGRADE PAGE --[[
    private void UpdateUpgradePage()
    {
        for (int i = 0; i < upgradePUsGauge.Length; i++)
        {
            int currentlvl = GameData.current.powerupLvls[i];

            if (currentlvl <= 10)
            {
                upgradePUsPrice[i].text = upgradesPrice[i][currentlvl - 1].ToString();
            }
            else
            {
                upgradePUsPrice[i].text = "----";
            }
            upgradePUsGauge[i].sizeDelta = new Vector2(upgradesWidth[currentlvl - 1], upgradePUsGauge[i].sizeDelta.y);
            upgradePUsGauge[i].localPosition = new Vector3(upgradesPosX[currentlvl - 1], upgradePUsGauge[i].localPosition.y, upgradePUsGauge[i].localPosition.z);
        }
    }

    private void UpgradePowerUp(int puNum)
    {
        int currentlvl = GameData.current.powerupLvls[puNum];
        int price = upgradesPrice[puNum][currentlvl - 1];
        if (price <= GameData.current.coins)
        {
            GameData.current.coins -= price;
            GameData.current.powerupLvls[puNum] += 1;
            UpdateUpgradePage();
            UpdateCoins();
            SaveData();
        }
    }

    private void UpgradeMadness()
    {
        UpgradePowerUp(0);
    }

    private void UpgradeRainbow()
    {
        UpgradePowerUp(1);
    }

    private void UpgradeSlicer()
    {
        UpgradePowerUp(2);
    }
    //UPGRADE PAGE ]]--



    //SHOP PAGE --[[
    private void UpdateShopPage()
    {
        for (int i = 0; i < shopCount.Length; i++)
        {
            shopCount[i].text = GameData.current.powerupCount[i].ToString();
        }
    }

    private void BuyPowerUp(int puNum)
    {
        if(puPrice[puNum] <= GameData.current.coins)
        {
            GameData.current.coins -= puPrice[puNum];
            UpdateShopPage();
            UpdateCoins();
            SaveData();
        }
    }

    private void BuyMadness()
    {
        BuyPowerUp(0);
    }

    private void BuyRainbow()
    {
        BuyPowerUp(1);
    }

    private void BuySlicer()
    {
        BuyPowerUp(2);
    }
    //SHOP PAGE ]]--
}
