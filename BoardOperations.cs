using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardOperations : MonoBehaviour
{
    public GameController controller;
    GameObject Parent;
    public GameObject SpritePrefab;
    public GameObject WhiteSide;
    public GameObject BlackSide;

    public Material WhiteMaterial;
    public Material BlackMaterial;
    public Material CheckMaterial;

    public Sprite Lk;
    public Sprite Lr;
    public Sprite Ln;
    public Sprite Lb;
    public Sprite Lq;
    public Sprite Lp;

    public Sprite Dk;
    public Sprite Dr;
    public Sprite Dn;
    public Sprite Db;
    public Sprite Dq;
    public Sprite Dp;

    void Start()
    {
        controller = GetComponent<GameController>();
    }

    public void ResetBoard()
    {
        controller.Pieces = new string[8, 8];
        controller.selected = false;
        controller.WhiteTurn = true;
        controller.WhiteCanCastleLeft = true;
        controller.WhiteCanCastleRight = true;
        controller.BlackCanCastleLeft = true;
        controller.BlackCanCastleRight = true;
        controller.Ongoing = true;
        controller.WhiteWin.SetActive(false);
        controller.BlackWin.SetActive(false);
        controller.buttonCanvas.SetActive(false);

        for (int i = 0; i < controller.Pieces.GetLength(0); i++)
        {
            for (int j = 0; j < controller.Pieces.GetLength(1); j++)
            {
                controller.Pieces[j, i] = string.Empty;
            }
        }

        controller.Pieces[7, 0] = "r";
        controller.Pieces[7, 7] = "r";
        controller.Pieces[7, 1] = "n";
        controller.Pieces[7, 6] = "n";
        controller.Pieces[7, 4] = "k";
        controller.Pieces[7, 2] = "b";
        controller.Pieces[7, 5] = "b";
        controller.Pieces[7, 3] = "q";
        
        controller.Pieces[0, 0] = "R";
        controller.Pieces[0, 7] = "R";
        controller.Pieces[0, 1] = "N";
        controller.Pieces[0, 6] = "N";
        controller.Pieces[0, 2] = "B";
        controller.Pieces[0, 5] = "B";
        controller.Pieces[0, 3] = "Q";
        controller.Pieces[0, 4] = "K";
        for (int i = 0; i < 8; i++)
        {
            controller.Pieces[6, i] = "p";
            controller.Pieces[1, i] = "P";
        }

        DrawBoard();
    }

    public void DrawBoard()
    {
        Destroy(Parent);
        Parent = new GameObject("parent");
        for (int i = 0; i < controller.Pieces.GetLength(0); i++)
        {
            for (int j = 0; j < controller.Pieces.GetLength(1); j++)
            {
                string x = controller.Pieces[j, i];
                if (x != string.Empty)
                {
                    GameObject piece = Instantiate(SpritePrefab, new Vector3(i, 1, j), Quaternion.Euler(90, 0, 0));
                    piece.transform.SetParent(Parent.transform);
                    switch (x)
                    {
                        case "r":
                            piece.GetComponent<SpriteRenderer>().sprite = Dr;
                            break;
                        case "n":
                            piece.GetComponent<SpriteRenderer>().sprite = Dn;
                            break;
                        case "b":
                            piece.GetComponent<SpriteRenderer>().sprite = Db;
                            break;
                        case "q":
                            piece.GetComponent<SpriteRenderer>().sprite = Dq;
                            break;
                        case "k":
                            piece.GetComponent<SpriteRenderer>().sprite = Dk;
                            break;
                        case "p":
                            piece.GetComponent<SpriteRenderer>().sprite = Dp;
                            break;
                        case "R":
                            piece.GetComponent<SpriteRenderer>().sprite = Lr;
                            break;
                        case "N":
                            piece.GetComponent<SpriteRenderer>().sprite = Ln;
                            break;
                        case "B":
                            piece.GetComponent<SpriteRenderer>().sprite = Lb;
                            break;
                        case "Q":
                            piece.GetComponent<SpriteRenderer>().sprite = Lq;
                            break;
                        case "K":
                            piece.GetComponent<SpriteRenderer>().sprite = Lk;
                            break;
                        case "P":
                            piece.GetComponent<SpriteRenderer>().sprite = Lp;
                            break;
                    }
                }
            }
        }

        if (controller.isInCheck("K", controller.Pieces)) //white is in check
        {
            WhiteSide.GetComponent<MeshRenderer>().material = CheckMaterial;
        }
        else
        {
            WhiteSide.GetComponent<MeshRenderer>().material = WhiteMaterial;
        }
        if (controller.isInCheck("k", controller.Pieces)) //black is in check
        {
            BlackSide.GetComponent<MeshRenderer>().material = CheckMaterial;
        }
        else
        {
            BlackSide.GetComponent<MeshRenderer>().material = BlackMaterial;
        }
    }
}
