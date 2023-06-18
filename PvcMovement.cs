using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PvcMovement : MonoBehaviour, IMovement
{
    public GameController controller;
    public InputField nameW;
    public Toggle easyMode;

    void Start()
    {
        controller = GetComponentInParent<GameController>();
    }

    public void ChangeNames()
    {
        string name = nameW.text;
        bool easyMode = this.easyMode.isOn;
        controller.ChangeNames(name, easyMode: easyMode);
    }

    public void TakeHint()
    {
        if (!controller.WhiteTurn || !controller.Ongoing)
            return;
        controller.AI.DoHint();
    }
    
    public void Movement()
    {
        if(controller.moveBlack)
        {
            controller.moveBlack = false;
            controller.AI.MoveBlack();
            controller.AI_thinking.SetActive(false);
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
        }

        if (controller.Ongoing && !controller.WhiteTurn)
        {
            controller.moveBlack = true;
            controller.AI_thinking.SetActive(true);
        }


        if (Input.GetMouseButtonDown(0) && controller.Ongoing)
        {
            Vector3 MousePos = Input.mousePosition;
            Vector3 WorldPos = Camera.main.ScreenToWorldPoint(MousePos);
            int x = Mathf.FloorToInt(WorldPos.x);
            int z = Mathf.FloorToInt(WorldPos.z);

            if (!controller.selected)
            {
                if (x >= 0 && x <= 7 && z >= 0 && z <= 7 && controller.Pieces[z, x] != string.Empty && controller.WhiteTurn == controller.isWhite(controller.Pieces[z,x]))
                {
                    controller.board.ChangeSelection(x, z);
                    controller.selected = true;
                    controller.SelectedPos = new Vector2Int(x, z);
                }
            }
            else
            {
                if (x >= 0 && x <= 7 && z >= 0 && z <= 7 && !(controller.SelectedPos.x == x && controller.SelectedPos.y == z)
                    && controller.isWhite(controller.Pieces[z, x]) != controller.isWhite(controller.Pieces[controller.SelectedPos.y, controller.SelectedPos.x]) && controller.LegalMove(controller.SelectedPos.x, x, controller.SelectedPos.y, z, controller.Pieces[controller.SelectedPos.y, controller.SelectedPos.x],true,controller.Pieces))
                {
                    //move
                    controller.Pieces[z, x] = controller.Pieces[controller.SelectedPos.y, controller.SelectedPos.x];
                    controller.Pieces[controller.SelectedPos.y, controller.SelectedPos.x] = string.Empty;
                    controller.boardOperations.DrawBoard();
                    controller.board.ResetSelection();
                    controller.selected = false;
                    controller.WhiteTurn = !controller.WhiteTurn;

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

                    if (z == 7 && controller.Pieces[z,x] == "P")
                    {
                        controller.PromotionPos = new Vector2Int(x, z);
                        controller.PromoteWhite = true;
                        controller.EnableButtonToPromote();
                        controller.Ongoing = false;
                    }

                    if(z == 0 && controller.Pieces[z,x] == "p")
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
    }
}
