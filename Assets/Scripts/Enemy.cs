﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Singleton<Enemy>
{
    [SerializeField]
    private float speed;
    public float Speed 
    {
        get 
        {
            return speed;
        }
        set 
        {
            this.speed = value;
        }
    }
    private int treeIndex;
    private Stack<Node> path;
    private List<Debuff> debuffs = new List<Debuff>();
    private List<Debuff> debuffsToRemove = new List<Debuff>();
    private List<Debuff> newDebuffs = new List<Debuff>();
    public Point GridPosition {get; set;}
    private Vector3 destination;
    public SpawnPlace[] TreeScript;
    private Animator myAnimator;
    public Animator MyAnimator
    {
        get 
        {
            return myAnimator;
        }
    }
    private SpriteRenderer spriteRenderer;
    public bool IsActive{get; set;}
    public float MaxSpeed{ get; set;}
    public bool dying;

    [SerializeField]
    private Stat enemyHealth;
    public bool Alive 
    {
        get {return enemyHealth.CurrentVal > 0;}
    }

    private void Awake()
    {
        MaxSpeed = speed;
        dying = false;
        enemyHealth.Initialise();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        HandleDebuffs();
        Move();
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime); //go from current point to destination
        if (transform.position == destination) //if hit destination i.e. next tile
        {
            if (path != null && path.Count > 0)
            {
                GridPosition = path.Peek().GridPosition; //not removed from stack
                destination = path.Pop().WorldPosition;
            }
        }
    }

    private void SetPath(Stack<Node> newPath)
    {
        
        if (newPath != null)
        {
            this.path = newPath;
            GridPosition = path.Peek().GridPosition;
            destination = path.Pop().WorldPosition;
        }
    }
    public void Spawn(int enemyHealth)
    {
        SpawnPlace[] TreeScript = LevelManager.Instance.TreeScript;
        treeIndex = UnityEngine.Random.Range(0,9);
        transform.position = TreeScript[treeIndex].transform.position;
        
        this.enemyHealth.Bar.Reset();
        this.enemyHealth.MaxVal = enemyHealth;
        this.enemyHealth.CurrentVal = this.enemyHealth.MaxVal;

        Awake();
        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1,1), false));
        myAnimator = GetComponent<Animator>();

        SetPath(LevelManager.Instance.PathArray[treeIndex]);
    }

    public IEnumerator Scale(Vector3 from, Vector3 to, bool remove)
    {
        float progress = 0;
        while (progress <= 1)
        {
            transform.localScale = Vector3.Lerp(from, to, progress);
            progress += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = to;
        IsActive = true;
        if (remove)
        {
            Release();
        }
        
    }

    //call this when hit
    private void Animate()
    {
        //if hit by bullet then myAnimator.SetInteger("Dying", 1);

    }

    private IEnumerator VillageEntered(TileScript stoneTile) 
    {
        stoneTile.ColorTile(new Color32(255,118,118,255));
        yield return new WaitForSeconds(0.2f);
        Debug.Log("after wait");
        stoneTile.ColorTile(new Color32(255,255,255,255));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if reached village
        if (other.tag == "WaterBridge")
        {
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));

            //reduce health if no lockdown
            if (!GameManager.Instance.IsLockdown && !dying)
            {
                //reduce health and make village red
                foreach (TileScript stoneTile in LevelManager.Instance.StoneArray) 
                {
                    StartCoroutine(VillageEntered(stoneTile));
                }
                GameManager.Instance.Health = GameManager.Instance.Health - 20 ;     
            }
        }

        //not yet reached village but make sure enemy above tile
        else if (other.tag == "Tile")
        {
            spriteRenderer.sortingOrder = other.GetComponent<TileScript>().GridPosition.Y;
        }
    }

    public void Release()
    {
        debuffs.Clear();
        IsActive = false;
        GameManager.Instance.Pool.ReleaseObject(gameObject);
        GameManager.Instance.RemoveEnemy(this);
    }

    public void TakeDamage(float damage, Element dmgSource )
    {
        if (IsActive)
        {
            enemyHealth.CurrentVal -= damage;
            if (enemyHealth.CurrentVal <= 0)
            {
                SoundManager.Instance.PlaySFX("enemydeath");
                if (GameManager.Instance.Wave == 1) 
                {
                    GameManager.Instance.Currency += 2;
                }
                else
                {
                    GameManager.Instance.Currency += 3;
                }
                
                myAnimator.SetTrigger("Dying");
                IsActive = false;
                dying = true;
                GetComponent<SpriteRenderer>().sortingOrder--; //other monsters can step over it
            }
        }
   
    }

    public void AddDebuff(Debuff debuff)
    {
        if (!debuffs.Exists(x => x.GetType() == debuff.GetType()))
        {
            newDebuffs.Add(debuff);
        }
    }

    public void RemoveDebuff(Debuff debuff)
    {
        debuffsToRemove.Add(debuff);
    }

    private void HandleDebuffs()
    {
        if (newDebuffs.Count > 0)
        {
            debuffs.AddRange(newDebuffs);
            newDebuffs.Clear();
        }
        foreach (Debuff debuff in debuffsToRemove)
        {
            debuffs.Remove(debuff);            
        }
        debuffsToRemove.Clear();
        foreach (Debuff debuff in debuffs)
        {
            debuff.Update();
        }
    }
}