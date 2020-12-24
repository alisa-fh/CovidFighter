using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//parent class of all towers
public class Tower : MonoBehaviour
{
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
    private SpriteRenderer mySpriteRenderer;
    private Enemy target;
    public Enemy Target
    {
        get 
        {
            return target;
        }
    }
    private bool canAttack = true;
    private float attackTimer; //when able to attack again
    [SerializeField]
    private float attackCooldown; //how often can attack
    private Queue<Enemy> enemies = new Queue<Enemy>();
    // Start is called before the first frame update
    void Start()
    {
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
        Debug.Log(target);
    }

    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
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
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();
        projectile.transform.position = transform.position;
        projectile.Initialise(this);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collision");
        if (other.tag == "Enemy") 
        {
            enemies.Enqueue(other.GetComponent<Enemy>());
        }
    }

    public void OnTriggerExit2D(Collider2D other) 
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("enemy exit");
            target = null;
        }
    }
}
