﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Enemy target;
    private Tower parent;
    private Animator myAnimator;
    private Element elementType;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
    }

    public void Initialise(Tower parent)
    {
        this.target = parent.Target;
        this.parent = parent;
        this.elementType = parent.ElementType;
    }
    private void MoveToTarget()
    {
        if (target != null && target.IsActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * parent.ProjectileSpeed);
            Vector2 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            target.MyAnimator.SetInteger("Dying", 1);
            //StartCoroutine(ExecuteAfterTime(2));
        }
        else if (!target.IsActive)
        {
            GameManager.Instance.Pool.ReleaseObject(gameObject);
        }

    }

    private IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        target.MyAnimator.SetInteger("Dying", 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            if (target.gameObject == other.gameObject)
            {
                target.TakeDamage(parent.Damage, elementType); //takes tower (parent)'s damage
                myAnimator.SetTrigger("Impact");
                ApplyDebuff();
            }
            Enemy hitInfo = other.GetComponent<Enemy>();
            
        }
    }

    private void ApplyDebuff()
    {
        float roll = Random.Range(0, 100);
        if (roll <= parent.Proc)
        {
            target.AddDebuff(parent.GetDebuff());
        }

    }
    
}