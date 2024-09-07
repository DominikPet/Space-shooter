using Assets.__Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour
{
    
    static public Main S;
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;
    public int score = 0;
    public int coins = 0;
    public int speedCost = 10;
    public int shootingDelayCost = 10;
    public float countdownDelay = 1f;
    public float controlsFadeTime = 1f;
    private int i = 1;

    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies;
    public static float enemySpawnPerSecond = 0.5f;
    public static float enemySpawnPerSecondEasing = 1200;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2;
    public float upgradeDelay = 0.2f;
    public float shootingDelayUpgrade = 0;
    public GameObject prefabPowerUp;
    public TextMeshPro textScore;
    public TextMeshPro textHighScore;
    public TextMeshProUGUI textCoin;
    public TextMeshProUGUI textCountdown;
    public GameObject explosionPrefab;
    public GameObject impactExplosionPrefab;
    public GameObject impactBasicPrefab;
    public AudioSource audioSource;
    public AudioClip explosionAudio;
    public AudioClip pingSound;
    public Button speedUpgradeButton;
    public Button shootingDelayButton;
    public TextMeshProUGUI speedCostText;
    public TextMeshProUGUI projectileSpeedCostText;
    public Canvas controlsCanvas;


    public eWeaponType[] powerUpFrequency = new eWeaponType[]
    {
        eWeaponType.blaster, eWeaponType.missile, eWeaponType.spread, eWeaponType.shield, eWeaponType.missile
    };


    static public void SHIP_DESTROYED(Enemy e)
    {
        S.explosionPrefab.transform.position = e.transform.position;
        Instantiate<GameObject>(S.explosionPrefab);
        S.audioSource.PlayOneShot(S.explosionAudio, 0.6f);
        if (UnityEngine.Random.value <= e.powerUpDropChance)
        {
            int ndx = UnityEngine.Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];
            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            pUp.SetType(pUpType);
            pUp.transform.position = e.transform.position;
        }
    }

    private void ShowControls()
    {
        controlsCanvas.enabled = true;
    }
    private void HideControls()
    {
        controlsCanvas.enabled = false;
    }

    private void DrawCoins()
    {
        textCoin.text = coins + "$";
    }

    public WeaponDefinition[] weaponDefinitions;

    private BoundsCheck bndCheck;

    void DelayedRestart()
    {
        Invoke(nameof(Restart), gameRestartDelay);
    }

    private void Restart()
    {
        SceneManager.LoadScene("__Scene_StartMenu");
    }

    static public void HERO_DIED()
    {
        S.NewHighScore();
        S.LoadHighScore();
        S.DelayedRestart();
    }

    private void NewHighScore()
    {
            if (score > PlayerPrefs.GetInt("hiScore"))
            {
                textHighScore.text = "New high score: " + score;
            }
    }

    static public WeaponDefinition GET_WEAPON_DEFINITION(eWeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        return (new WeaponDefinition());
    }

    private void Awake()
    {
        enemySpawnPerSecond = 0.4f;
        upgradeDelay += Time.time;

        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        ShowControls();

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    private void Start()
    {
        StartCountdown();
        LoadHighScore();
        CheckUpgrades();
    }

    private void StartCountdown()
    {
        if (i < 4)
        {
            textCountdown.text = i + "";
            i++;
            audioSource.PlayOneShot(pingSound);
            Invoke(nameof(StartCountdown), countdownDelay);
        }
        else
        {
            HideControls();
            textCountdown.text = "";
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
        }
    }

    public void UpgradeSpeed()
    {
        if (upgradeDelay < Time.time)
        {
            coins -= speedCost;
            speedCost *= 2;
            speedCostText.text = speedCost + "$";
            CheckUpgrades();
            Hero.S.speed += 5; 
            upgradeDelay = Time.time + 0.2f;
        }
    }
    
    public void UpgradeShootingDelay()
    {
        if (upgradeDelay < Time.time)
        {
            coins -= shootingDelayCost;
            shootingDelayCost *= 2;
            projectileSpeedCostText.text = shootingDelayCost + "$";
            CheckUpgrades();
            shootingDelayUpgrade += 0.03f; 
            upgradeDelay = Time.time + 0.2f;
        }
    }

    private void LoadHighScore()
    {
        if (PlayerPrefs.HasKey("hiScore"))
        {
            if (score > PlayerPrefs.GetInt("hiScore"))
            {
                textHighScore.text = "High score: " + score;
                PlayerPrefs.SetInt("hiScore", score);
                PlayerPrefs.Save();
            }
            else
            {
                textHighScore.text = "High score: " + PlayerPrefs.GetInt("hiScore");
            }
        }
        else
        {
                PlayerPrefs.SetInt("hiScore", score);
                PlayerPrefs.Save();
        }
    }

    private void SpawnEnemy()
    {
        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);
            return;
        }
        int ndx = UnityEngine.Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
        Debug.Log("Enemy spawned");
        float enemyInset = enemyInsetDefault;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset/2;
        float xMax = bndCheck.camHeight - enemyInset;
        pos.x = UnityEngine.Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;
        go.transform.position = pos;

        Invoke(nameof(SpawnEnemy), 1f / enemySpawnPerSecond);

        
    }

    private void Update()
    {
        DrawScore();
        DrawCoins();

        if (Input.GetAxis("Cancel") == 1)
        {
            SceneManager.LoadScene("__Scene_StartMenu");
        }
    }

    public static void CheckUpgrades()
    {
        if (S.coins >= S.speedCost)  //check if speed upgrade can be bought
        {
            S.speedUpgradeButton.enabled = true;
            S.speedUpgradeButton.gameObject.SetActive(true);
        }
        else
        {
            S.speedUpgradeButton.enabled = false;
            S.speedUpgradeButton.gameObject.SetActive(false);
        }

        if (S.coins >= S.shootingDelayCost)   //check if delay upgrade can be bought
        {
            S.shootingDelayButton.enabled = true;
            S.shootingDelayButton.gameObject.SetActive(true);
        }
        else
        {
            S.shootingDelayButton.enabled = false;
            S.shootingDelayButton.gameObject.SetActive(false);
        }
    }

    private void DrawScore()
    {
        textScore.text = "Score: " + score;
    }
}
