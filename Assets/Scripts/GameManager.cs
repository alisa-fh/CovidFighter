using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public delegate void CurrencyChanged(); //way of triggering event

public class GameManager : Singleton<GameManager>
{ 
    [SerializeField]
    public GameObject gameMenu;
    [SerializeField]
    public GameObject optionsMenu;
    [SerializeField]
    public GameObject winMenu;

    public event CurrencyChanged Changed;

    public TowerBtn ClickedBtn { get; set; }

    public ObjectPool Pool { get; set; }

    private bool gameOver = false;
    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject upgradePanel;
    [SerializeField]
    private GameObject upgradeText;
    [SerializeField]
    private GameObject statsPanel;

    [SerializeField]
    private Text visualText;
    [SerializeField]
    private Text upgradePrice;

    //current selected tower
    private Tower selectedTower;

    private int wave = 1;
    public int Wave 
    {
        get 
        {
            return wave;
        }
    }

    [SerializeField]
    private Text waveText;
    private string waveNum = "first";

    [SerializeField]
    private GameObject waveBtn;

    [SerializeField]
    private GameObject waveInfo;
    [SerializeField]
    private Text waveInfoText;
    [SerializeField]
    private Text waveInfoTitle;


    [SerializeField]
    private GameObject vaccineBtn;
    private int vaccinePrice = 5;
    [SerializeField]
    private Text vaccinePriceText;
    [SerializeField]
    private GameObject vaccineBar;

    [SerializeField]
    private GameObject lockdownBtn;
    private int lockdownPrice = 5;
    [SerializeField]
    private Text lockdownPriceText;
    private bool isLockdown = false;
    public bool IsLockdown 
    { 
        get 
        {
            return isLockdown;
        }
    }    

    private int enemyHealth = 12;

    private int health;
    [SerializeField]
    private Text healthText;
    public int Health 
    {
        get 
        {
            return health;
        }
        set 
        {
            this.health = value;
            healthText.text = value.ToString();
            if (health <= 0)
            {
                this.health = 0;
                healthText.text = "0";
                GameOver();
            }
            
        }
    }

    private List<Enemy> activeEnemies = new List<Enemy>();
    public bool WaveActive
    {
        get
        { 
            return activeEnemies.Count > 0;
        }
    }

    private int currency;
    [SerializeField]
    private Text currencyText;
    public int Currency
    {
        get
        {
            return currency;
        }
        set
        {
            this.currency = value;
            this.currencyText.text = value.ToString() + " coins";
            OnCurrencyChanged();
        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Changed += new CurrencyChanged(PriceCheck); //pricecheck executed every time changed is
        Health = 100;
        health = 100;
        Currency = 15;
        lockdownPriceText.text = lockdownPrice + " coins";
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleEscape(); 
    }

    public void PickTower(TowerBtn towerBtn) //run upon clicking tower (configured through Unity UI)
    {
        if (Currency >= towerBtn.Price && !WaveActive) 
        {
            this.ClickedBtn = towerBtn;
            Hover.Instance.Activate(towerBtn.Sprite);
        }
    }

    public void BuyTower() //executed when a tower is clicked in TileScript
    {
        if (Currency >= ClickedBtn.Price)
        {
            Currency -= ClickedBtn.Price;
            Hover.Instance.Deactivate();
        }
    }

    public void OnCurrencyChanged()
    {
        if (Changed != null)
        {
            Changed();
        }
    }

    public void SelectTower(Tower tower)
    {
        //checking if reclicking same tower - in which case, deselect
        if (selectedTower != tower)
        {
            if (selectedTower != null)
            {
                selectedTower.Select();
            }
            selectedTower = tower;
            selectedTower.Select();

            upgradePanel.SetActive(true);
        }
        else 
        {
            DeselectTower();
        }
    }
    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }
        upgradePanel.SetActive(false);
        selectedTower = null;
        
    }
// When escape is clicked, tower should be removed from cursor (or hidden)
    private void HandleEscape()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
          if (selectedTower == null && !Hover.Instance.IsVisible) 
          {
            ShowMenu();
          }
          else if (Hover.Instance.IsVisible)
          {
              DropTower();
          }
          else if (selectedTower != null)
          {
              DeselectTower();
          }
      }  
    }

    private void PriceCheck()
    {
        if (lockdownPrice <= GameManager.Instance.Currency)
        {
            lockdownBtn.GetComponent<Image>().color = Color.white;
        }
        else
        {
            lockdownBtn.GetComponent<Image>().color = Color.grey;
        }

        if (vaccinePrice <= GameManager.Instance.Currency)
        {
            vaccineBtn.GetComponent<Image>().color = Color.white;
        }
        else
        {
            vaccineBtn.GetComponent<Image>().color = Color.grey;
        }


    }

    public void ShowVaccineToolTip()
    {
        string ttText = string.Empty;
        ttText = string.Format("<color=#94FF67><size=30><b>Vaccine</b></size></color>\n Vaccine is prepared for 8 seconds before increasing health by 60");
        GameManager.Instance.SetTooltipText(ttText); 
        statsPanel.SetActive(true);  
    }
    public void HideVaccineToolTip()
    {
        string ttText = string.Empty;
        ttText = string.Format("<color=#94FF67><size=30><b>Vaccine</b></size></color>\n Vaccine is prepared for 8 seconds before increasing health by 60");
        GameManager.Instance.SetTooltipText(ttText); 
        statsPanel.SetActive(false);  
    }

    public void VaccineClick()
    { 
        //todo remove currency
        if (Currency >= vaccinePrice)
        {
            Currency -= vaccinePrice;
            StartCoroutine(StartVaccine());
        } 
    }

    private IEnumerator StartVaccine()
    {
        //during vaccine growth hide button
        vaccineBtn.SetActive(false);
        vaccineBar.GetComponent<BarScript>().Reset();
        vaccineBar.SetActive(true);

        //show bar and grow vaccine 
        for (int secs = 0; secs < 8; secs++)
        {
            vaccineBar.GetComponent<BarScript>().BarValue += 12.5f ;
            yield return new WaitForSeconds(1f);
        }
        //vaccine time over: remove bar and increase health
        Health += 60;
        vaccineBar.SetActive(false);
        vaccineBtn.SetActive(true);
    }

    public void ShowLockdownToolTip()
    {
        string ttText = string.Empty;
        ttText = string.Format("<color=#FF5132><size=30><b>Lockdown</b></size></color>\n Village becomes immune to all enemies that enter for 5 seconds");
        GameManager.Instance.SetTooltipText(ttText); 
        statsPanel.SetActive(true);  
    }

    public void HideLockdownToolTip()
    {
        string ttText = string.Empty;
        ttText = string.Format("<color=#FF5132><size=30><b>Lockdown</b></size></color>\n Village becomes immune to all enemies that enter for 5 seconds");
        GameManager.Instance.SetTooltipText(ttText); 
        statsPanel.SetActive(false);  
    }

    public void LockdownClick()
    { 
        //grey out button if not enough currency and do nothing onclick
        //todo remove currency
        if (Currency >= lockdownPrice)
        {
            Currency -= lockdownPrice;
            StartCoroutine(StartLockdown());
        } 
    }

    private IEnumerator StartLockdown()
    {
        //during lockdown hide button
        lockdownBtn.SetActive(false);
        //when bool true, enemies won't remove lives
        isLockdown = true;
        foreach (TileScript stoneTile in LevelManager.Instance.StoneArray) 
        {
            stoneTile.ColorTile(new Color32(121,110,110,255));
        }
        yield return new WaitForSeconds(5f);

        //lockdown over
        foreach (TileScript stoneTile in LevelManager.Instance.StoneArray) 
        {
            stoneTile.ColorTile(new Color32(255,255,255,255));
        }
        isLockdown = false;
        lockdownBtn.SetActive(true);
    }

    public void StartWave() //triggered by next wave button
    {
        DeselectTower();
        if (wave == 1) 
        {
            waveNum = "first"; 
        }
        if (wave == 2) 
        {
            waveNum = "second"; 
        }
        else if (wave == 3)
        {
            waveNum = "third";
        }
        waveText.text = string.Format(waveNum + " wave");
        StartCoroutine(SpawnWave());
        waveBtn.SetActive(false);
        waveInfo.SetActive(false);

        lockdownBtn.SetActive(true);
        vaccineBtn.SetActive(true);
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < wave * 5; i++) //num enemies is wave num * 5
        {
            LevelManager.Instance.GeneratePath();
            int monsterIndex = UnityEngine.Random.Range(0,3);
            string type = string.Empty; //set to prefab
            switch (monsterIndex)
            {
                case 0:
                    type = "BlueEnemy";
                    break;
                case 1:
                    type = "ClassicEnemy";
                    break;
                case 2:
                    type = "GreenEnemy";
                    break;
            }
            Enemy enemy = Pool.GetObject(type).GetComponent<Enemy>(); //instantiate and return a new gameobject
            activeEnemies.Add(enemy);
            enemy.Spawn(enemyHealth);
            //increase difficulty i.e. enemy health each wave
            if (wave != 1) 
            {
                enemyHealth += 1;
            }
            yield return new WaitForSeconds(2.5f); 
            
        } 
    }
    public void RemoveEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        if (!WaveActive && !gameOver)
        {
            wave++;
            if (Wave == 2)
            {
                waveBtn.SetActive(true);
                waveInfo.SetActive(true);
                waveInfoTitle.text = "Second wave";
                waveInfoText.text = String.Format("They're getting stronger. Each enemy is worth 3 coins.\n Good luck.");
            }
            else if (Wave == 3)
            {
                waveBtn.SetActive(true);
                waveInfo.SetActive(true);
                waveInfoTitle.text = String.Format("Third Wave");
                waveInfoText.text = String.Format("Almost over. They're stronger than ever. Each enemy is worth 3 coins.\n Good luck.");
            }
            else if (Wave == 4)
            {
                winMenu.SetActive(true);
            }
            
            lockdownBtn.SetActive(false);
            vaccineBtn.SetActive(false);
        }
    }
    
    public void Restart()
    {
        //if zero then pauses
        Time.timeScale = 1;
        //reloads current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);

        }
    }

    private void DropTower()
    {
        ClickedBtn = null;
        Hover.Instance.Deactivate();
    }

    public void ShowTooltip()
    {
        statsPanel.SetActive(!statsPanel.activeSelf); 
    }

    public void ShowSelectedTowerStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf); 
        UpdateUpgradeTip();
    }
    public void SetTooltipText(string text)
    {
        visualText.text = text;
    }

    public void UpdateUpgradeTip()
    {
        if (selectedTower != null)
        {
            SetTooltipText(selectedTower.GetStats());
            if (selectedTower.NextUpgrade != null) 
            {
                upgradePrice.text = selectedTower.NextUpgrade.Price.ToString() + "coins";
            }
            else 
            {
                upgradePrice.text = "Complete";
            }
        }

    }

    public void UpgradeTower()
    {
        if (selectedTower != null)
        {
            if (selectedTower.Level <= selectedTower.Upgrades.Length && Currency >= selectedTower.NextUpgrade.Price)
            {
                selectedTower.Upgrade();
            }
        }   
    }

    public void ShowMenu()
    {
        if (optionsMenu.activeSelf)
        {
            BackToMain();
        }
        else 
        {
            gameMenu.SetActive(!gameMenu.activeSelf);
            if (!gameMenu.activeSelf)
            {
                Time.timeScale = 1;
            }
            else 
            {
                Time.timeScale = 0;
            }
        }
    }

    public void ShowOptions()
    {
        gameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void BackToMain()
    {
        optionsMenu.SetActive(false);
        gameMenu.SetActive(true);
    }

    


    public void QuitGame()
    {
        Application.Quit();
    }


}
