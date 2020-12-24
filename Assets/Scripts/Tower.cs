using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private SpriteRenderer mySpriteRenderer;
    private Enemy target;
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
        if (target == null && enemies.Count > 0)
        {
            target = enemies.Dequeue();
        }
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
            target = null;
        }
    }
}
