using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public delegate void CurrencyChanged(); //way of triggering event

public class GameManager : Singleton<GameManager>
{ 
    [SerializeField]
    public GameObject gameMenu;
    [SerializeField]
    public GameObject optionsMenu;
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

    private int wave = 0;

    [SerializeField]
    private Text waveText;
    private string waveNum = "first";

    [SerializeField]
    private GameObject waveBtn;

    private int enemyHealth = 18;

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
        Debug.Log("gamestart");
        Health = 100;
        health = 100;
        Currency = 15;
        
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
        if (selectedTower != null)
        {
            selectedTower.Select();
        }
        selectedTower = tower;
        selectedTower.Select();

        upgradePanel.SetActive(true);
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
    public void StartWave() //triggered by next wave button
    {
        wave++;
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
        

    }
    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < wave * 5; i++) //num enemies is wave num * 5
        {
            LevelManager.Instance.GeneratePath();
            int monsterIndex = Random.Range(0,3);
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
                enemyHealth += 3;
            }
            yield return new WaitForSeconds(2.5f); 
            
        } 
    }
    public void RemoveEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        if (!WaveActive && !gameOver)
        {
            waveBtn.SetActive(true);
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
