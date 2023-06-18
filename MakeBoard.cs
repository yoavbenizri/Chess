using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeBoard : MonoBehaviour
{
    public GameObject PrefabPanel;
    public Material LightMaterial;
    public Material DarkMaterial;
    public Material SelectedMaterial;
    public Vector2Int Selected;
    public GameObject PrefabPossibleMove;
    public GameController controller;
    public GameObject PossibleMoveParent;
    
    GameObject[,] Panels = new GameObject[8, 8];

    void Start()
    {
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                GameObject Panel = Instantiate(PrefabPanel, new Vector3(i + 0.5f, 0, j + 0.5f), new Quaternion(0, 0, 0, 0));
                Panel.transform.SetParent(this.transform);
                Panels[i, j] = Panel;
                if (i % 2 != j % 2)
                    Panel.GetComponent<MeshRenderer>().material = LightMaterial;
                else
                    Panel.GetComponent<MeshRenderer>().material = DarkMaterial;
            }
        }
    }

    public void ChangeSelection(int x, int y)
    {
        foreach (Vector2Int possibleMove in controller.AllMovesOfAPiece(x, y, controller.Pieces[y, x],
                     controller.Pieces))
        {
            GameObject possibleMoveObject = Instantiate(PrefabPossibleMove, new Vector3(possibleMove.x + 0.5f, 0, possibleMove.y + 0.5f),
                new Quaternion(0, 0, 0, 0));
            possibleMoveObject.transform.SetParent(this.PossibleMoveParent.transform);
        }
        if (Selected.x % 2 != Selected.y % 2)
            Panels[Selected.x, Selected.y].GetComponent<MeshRenderer>().material = LightMaterial;
        else
            Panels[Selected.x, Selected.y].GetComponent<MeshRenderer>().material = DarkMaterial;
        Panels[x, y].GetComponent<MeshRenderer>().material = SelectedMaterial;
        Selected = new Vector2Int(x, y);
    }
    public void ResetSelection()
    {
        if (Selected.x % 2 != Selected.y % 2)
            Panels[Selected.x, Selected.y].GetComponent<MeshRenderer>().material = LightMaterial;
        else
            Panels[Selected.x, Selected.y].GetComponent<MeshRenderer>().material = DarkMaterial;
        foreach (Transform child in PossibleMoveParent.transform)
            Destroy(child.gameObject);
    }
}
