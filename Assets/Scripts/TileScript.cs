using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public Point GridPosition {get; private set;}
    public bool IsEmpty {get; private set;}
    private Color32 fullColor = new Color32(255,118,118,255);
    private Color32 emptyColor = new Color32(96,255,90,255);
    //CHANGE FOLLOWING TO PRIVATE
        public SpriteRenderer spriteRenderer;
    public bool Walkable {get; set;}
    public bool Debugging {get; set;}
    private Tower myTower;

    public Vector2 WorldPosition
    {
        get
        {
            return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y/2));
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
    {
        if (this.name.Contains("stone")){ //|| (gridPos.X == 6 && gridPos.Y == 4)
            IsEmpty = false;
            Walkable = false;
        }
        else
        {
            Walkable = true;
            IsEmpty = true;
        }
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null) //only tries to place a tower if mouse is not over a button etc
        {
            if (IsEmpty && !Debugging)
            {
                ColorTile(emptyColor);
            }
            if (!IsEmpty && ! Debugging)
            {
                ColorTile(fullColor);
            }
            else if (Input.GetMouseButtonDown(0)) //If button is clicked (not held)
            {
                PlaceTower();
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn == null && Input.GetMouseButtonDown(0))
        {
            if (myTower != null)
            {
                GameManager.Instance.SelectTower(myTower);
            }
            else
            {
                GameManager.Instance.DeselectTower();
            }
        }
    }

    private void OnMouseExit()
    {
        if (!Debugging)
        {
            ColorTile(Color.white);
        }
    }

    private void PlaceTower()
    {
        GameObject tower = (GameObject)Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, transform.position, Quaternion.identity);
        tower.GetComponent<SpriteRenderer>().sortingOrder = GridPosition.Y;
        tower.transform.SetParent(transform);
        this.myTower = tower.transform.GetChild(0).GetComponent<Tower>();
        IsEmpty = false;
        ColorTile(Color.white);
        myTower.Price = GameManager.Instance.ClickedBtn.Price;
        GameManager.Instance.BuyTower();
        Walkable = false;
    }

    private void ColorTile(Color32 newColor)
    {
        spriteRenderer.color = newColor;
    }

}
