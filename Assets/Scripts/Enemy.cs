using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private int treeIndex;
    private Stack<Node> path;
    public Point GridPosition {get; set;}
    private Vector3 destination;
    public SpawnPlace[] TreeScript;
    private Animator myAnimator;
    public bool IsActive{get; set;}

    private void Update()
    {
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
            //Debug.Log(newPath.Count);
            this.path = newPath;
            //Debug.Log(this.path.Count);
            GridPosition = path.Peek().GridPosition;
            destination = path.Pop().WorldPosition;
        }
    }
    public void Spawn()
    {
        SpawnPlace[] TreeScript = LevelManager.Instance.TreeScript;
        treeIndex = UnityEngine.Random.Range(0,9);
        transform.position = TreeScript[treeIndex].transform.position;
        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1,1), false));
        myAnimator = GetComponent<Animator>();
        //transform.position = LevelManager.Instance.TreeScript[treeIndex].transform.position;
        //Debug.Log(LevelManager.Instance.PathArray.Length);
        //Debug.Log(LevelManager.Instance.PathArray[treeIndex].Count);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "WaterBridge")
        {
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f), true));
            GameManager.Instance.Health = GameManager.Instance.Health - 30 ;
        }
    }

    private void Release()
    {
        IsActive = false;
        myAnimator.SetInteger("Dying", 0);
        GameManager.Instance.Pool.ReleaseObject(gameObject);
        GameManager.Instance.RemoveEnemy(this);
    }
}
