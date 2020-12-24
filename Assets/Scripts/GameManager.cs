using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{ 
    public TowerBtn ClickedBtn { get; set; }

    public ObjectPool Pool { get; set; }

    private bool gameOver = false;
    [SerializeField]
    private GameObject gameOverMenu;

    //current selected tower
    private Tower selectedTower;

    private int wave = 0;

    [SerializeField]
    private Text waveText;
    private string waveNum = "first";

    [SerializeField]
    private GameObject waveBtn;

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
        }
    }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }
    // Start is called before the first frame update
    void Start()
    {
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

    public void SelectTower(Tower tower)
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }
        selectedTower = tower;
        selectedTower.Select();
    }
    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }
        selectedTower = null;
    }
// When escape is clicked, tower should be removed from cursor (or hidden)
    private void HandleEscape()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
          Hover.Instance.Deactivate();
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
            enemy.Spawn();
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

    public void QuitGame()
    {
        Application.Quit();
    }


}
