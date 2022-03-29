using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element {FIRE, STONE, NONE}
//parent class of all towers
public abstract class Tower : MonoBehaviour
{
    public Element ElementType {get; protected set;}
    private int level; //current level
    public int Level {get; protected set;}
    public TowerUpgrade[] Upgrades {get; protected set;}
    public int Price 
    {
        get; set;
    }
    [SerializeField]
    private string projectileType;
    [SerializeField]
    private float projectileSpeed;
    public float ProjectileSpeed
    {
        get 
        {
            return projectileSpeed;
        }
    }
    [SerializeField]
    private float debuffDuration;
    public float DebuffDuration 
    {
        get 
        {
            return debuffDuration;
        }
        set 
        {
            this.debuffDuration = value;
        }
    }
    [SerializeField]
    private float proc; //chance debuff applied
    public float Proc 
    {
        get 
        {
            return proc;
        }
        set 
        {
            this.proc = value;
        }
    }
    private SpriteRenderer mySpriteRenderer;
    public SpriteRenderer MySpriteRenderer 
    {
        get 
        {
            return mySpriteRenderer;
        }
    }
    private Enemy target;
    public Enemy Target
    {
        get 
        {
            return target;
        }
    }

    [SerializeField]
    private int damage;
    public int Damage
    {
        get 
        {
            return damage;
        }
    }

    public TowerUpgrade NextUpgrade 
    {
        get 
        {
            if (Upgrades.Length > Level - 1 )
            {
                return Upgrades[Level - 1];
            }
            return null;
        }
    }

    private bool canAttack = true;
    private float attackTimer; //when able to attack again
    [SerializeField]
    private float attackCooldown; //how often can attack
    private Queue<Enemy> enemies = new Queue<Enemy>();
    // Start is called before the first frame update
    void Awake()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        Level = 1;       
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
        GameManager.Instance.UpdateUpgradeTip();
    }

    public void Attack()
    {
        if (!canAttack) //has attacked
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
        if (target == null && enemies.Count > 0)
        {
            target = enemies.Dequeue();
        }
        if (target != null && target.IsActive)
        {
            if (canAttack)
            {
                Shoot();
                canAttack = false;
            }
            
        }
        //uncomment to change target once one has died
        else if (enemies.Count > 0)
        {
            target = enemies.Dequeue();
        }
        if (target != null && !target.Alive || target != null && !target.IsActive )
        {
            target = null;
        }
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.Initialise(this);
    }

    public virtual void Upgrade()
    {
        GameManager.Instance.Currency -= NextUpgrade.Price;
        Price += NextUpgrade.Price;
        this.damage += NextUpgrade.Damage;
        this.proc += NextUpgrade.ProcChance;
        this.DebuffDuration += NextUpgrade.DebuffDuration;
        Level++;
        GameManager.Instance.UpdateUpgradeTip();

    }

    public virtual string GetStats()
    {
        if (NextUpgrade != null) 
        {
            return string.Format("\nLevel: {0} \nDamage: {1} <color=#00ff00ff> + {4}</color>\n Chance of special Attack:{2}% <color=#00ff00ff>+{5}%</color>\nSpecial attack duration: {3} sec <color=#00ff00ff>+{6}</color>", Level, damage, proc, DebuffDuration, NextUpgrade.Damage, NextUpgrade.ProcChance, NextUpgrade.DebuffDuration);
        }
        return string.Format("\nLevel: {0}\n Damage: {1}\nChance of special attack: {2}%\n Special attack duration: {3}sec", Level, damage, proc, DebuffDuration);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy") 
        {
            enemies.Enqueue(other.GetComponent<Enemy>());
        }
    }

    public abstract Debuff GetDebuff(); //return debuff of specific type

    public void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag == "Enemy")
        {
            target = null;
        }
    }
}
