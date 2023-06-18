using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvpMovement : MonoBehaviour, IMovement
{
    public GameController controller;
    public InputField nameW;
    public InputField nameB;


    void Start()
    {
        controller = GetComponentInParent<GameController>();
    }

    public void ChangeNames()
    {
        string name1 = nameW.text;
        string name2 = nameB.text;
        controller.ChangeNames(name1, name2);
    }
    
    public void Movement()
    {
        if (!controller.Ongoing)
            return;

        if (controller.MateStatus(true, controller.Pieces) == 1) //black won
        {
            controller.Ongoing = false;
            controller.BlackWin.SetActive(true);
        }

        if (controller.MateStatus(false, controller.Pieces) == 1) //white won
        {
            controller.Ongoing = false;
            controller.WhiteWin.SetActive(true);
        }

        if (!Input.GetMouseButtonDown(0))
            return;

        Vector3 MousePos = Input.mousePosition;
        Vector3 WorldPos = Camera.main.ScreenToWorldPoint(MousePos);
        int x = Mathf.FloorToInt(WorldPos.x);
        int z = Mathf.FloorToInt(WorldPos.z);

        if (!controller.selected)
        {
            if (x >= 0 && x <= 7 && z >= 0 && z <= 7 && controller.Pieces[z, x] != string.Empty &&
                controller.WhiteTurn == controller.isWhite(controller.Pieces[z, x]))
            {
                controller.board.ChangeSelection(x, z);
                controller.selected = true;
                controller.SelectedPos = new Vector2Int(x, z);
            }

            return;
        }

        if (x >= 0 && x <= 7 && z >= 0 && z <= 7 &&
            !(controller.SelectedPos.x == x && controller.SelectedPos.y == z)
            && controller.isWhite(controller.Pieces[z, x]) !=
            controller.isWhite(controller.Pieces[controller.SelectedPos.y, controller.SelectedPos.x]) &&
            controller.LegalMove(controller.SelectedPos.x, x, controller.SelectedPos.y, z,
                controller.Pieces[controller.SelectedPos.y, controller.SelectedPos.x], true, controller.Pieces))
        {
            //move
            controller.Pieces[z, x] = controller.Pieces[controller.SelectedPos.y, controller.SelectedPos.x];
            controller.Pieces[controller.SelectedPos.y, controller.SelectedPos.x] = string.Empty;
            controller.boardOperations.DrawBoard();
            controller.board.ResetSelection();
            controller.selected = false;
            controller.WhiteTurn = !controller.WhiteTurn;

            if (z == 7 && controller.Pieces[z, x] == "P")
            {
                controller.PromotionPos = new Vector2Int(x, z);
                controller.PromoteWhite = true;
                controller.EnableButtonToPromote();
                controller.Ongoing = false;
            }

            if (z == 0 && controller.Pieces[z, x] == "p")
            {
                controller.PromotionPos = new Vector2Int(x, z);
                controller.PromoteWhite = false;
                controller.EnableButtonToPromote();
                controller.Ongoing = false;
            }



            if (z == 0 && x == 7)
                controller.WhiteCanCastleRight = false;
            if (z == 0 && x == 0)
                controller.WhiteCanCastleLeft = false;
            if (z == 7 && x == 7)
                controller.BlackCanCastleRight = false;
            if (z == 7 && x == 0)
                controller.BlackCanCastleLeft = false;
            if (z == 0 && x == 4)
            {
                controller.WhiteCanCastleRight = false;
                controller.WhiteCanCastleLeft = false;
            }

            if (z == 7 && x == 4)
            {
                controller.BlackCanCastleLeft = false;
                controller.BlackCanCastleRight = false;
            }
        }

        else
        {
            controller.board.ResetSelection();
            controller.selected = false;
        }
    }
}
