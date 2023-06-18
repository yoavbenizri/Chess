using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IMovement
{
    void Movement();

    void ChangeNames();
}

public class GameController : MonoBehaviour
{
    public MakeBoard board;
    public bool selected = false;
    public Vector2Int SelectedPos;
    public bool WhiteTurn = true;
    public bool WhiteCanCastleLeft = true;
    public bool WhiteCanCastleRight = true;
    public bool BlackCanCastleLeft = true;
    public bool BlackCanCastleRight = true;
    public GameObject WhiteWin;
    public GameObject BlackWin;
    public bool Ongoing = true;
    public GameObject buttonCanvas;
    public Vector2Int PromotionPos;
    public bool PromoteWhite;
    public ArtificialIntelligence AI;
    public bool moveBlack = false;
    public GameObject AI_thinking;
    public BoardOperations boardOperations;
    public string[,] Pieces;
    public string FENPieces;
    public IMovement movement;
    public string nameW;
    public string nameB;
    public bool nameEntered = false;
    public Text tNameW;
    public Text tNameB;
    public Canvas changeNameCanvas;
    public GameObject hintButton;
    public GameObject menuButton;
    public InputField loadGameField;
    public bool easyMode;

    void Start()
    {
        AI = GetComponent<ArtificialIntelligence>();
        boardOperations = GetComponent<BoardOperations>();
        boardOperations.ResetBoard();
        boardOperations.DrawBoard();
        movement = GetComponent<IMovement>();
    }

    void Update()
    {
        if (!nameEntered)
            return;

        this.movement.Movement();
    }

    public void ExportGame()
    {
        ExportGameData exportedData = ExportGameData.ExportDataFromController(this);
        string export = JsonUtility.ToJson(exportedData);
        print(export);
        GUIUtility.systemCopyBuffer = export;
    }

    public void EnableLoadMenu()
    {
        Camera.main.gameObject.transform.rotation = Quaternion.Euler(270, 0,0);
        loadGameField.gameObject.SetActive(true);
        this.changeNameCanvas.enabled = false;
    }
    
    public void LoadGame()
    {
        ExportGameData importedData = JsonUtility.FromJson<ExportGameData>(loadGameField.text);
        importedData.ImportToController(this);
        boardOperations.DrawBoard();
        this.ChangeNames(this.nameW, this.nameB, this.easyMode);
        loadGameField.gameObject.SetActive(false);
    }

    public string GetFEN(string[,] pieces)
    {
        string fen = "";
        for (int i = 0; i < 8; i++)
        {
            //r_bk___r
            int counter = 0;
            for (int j = 0; j < 8; j++)
            {
                if (pieces[i, j] != "")
                {
                    if (counter > 0)
                    {
                        fen += counter;
                        counter = 0;
                    }

                    fen += pieces[i, j];
                }
                else
                {
                    counter++;
                }
            }

            if (counter > 0)
            {
                fen += counter;
            }

            if (i != 7)
            {
                fen += "/";
            }
        }

        return fen;
    }

    public string[,] FENToPieces(string fen)
    {
        string[] fenRows = fen.Split('/');
        string[,] pieces = new string[8, 8];
        for (int i = 0; i < 8; i++)
        {
            int spaces = 0;
            for (int j = 0; j < fenRows[i].Length; j++)
            {
                if (Char.IsDigit(fenRows[i][j]))
                {
                    int k;
                    for (k = 0; k < fenRows[i][j] - '0'; k++)
                    {
                        pieces[i, j + k + spaces] = "";
                    }

                    spaces += k - 1;
                }
                else
                {
                    pieces[i, j + spaces] = fenRows[i][j].ToString();
                }
            }
        }

        return pieces;
    }

    public void ChangeNames(string nameW, string nameB = "Computer", bool easyMode = false)
    {
        this.nameW = nameW;
        this.nameB = nameB;
        this.easyMode = easyMode;
        nameEntered = true;
        Camera.main.gameObject.transform.rotation = Quaternion.Euler(90, 0,0);
        this.changeNameCanvas.enabled = false;
        this.tNameW.text = nameW;
        this.tNameB.text = nameB;
        this.tNameW.enabled = true;
        this.tNameB.enabled = true;
        if (this.hintButton != null)
        {
            this.hintButton.SetActive(true);
            this.menuButton.transform.localPosition = new Vector3(-55, menuButton.transform.localPosition.y,
                menuButton.transform.localPosition.z);
        }
    }
    
    public bool LegalMove(int x1, int x2, int y1, int y2, string piece, bool check, string[,] array, bool isReal = true)
    {
        int dx = x2 - x1;
        int dy = y2 - y1;

        if (check)
        { 
            string[,] CheckForCheck = new string[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    CheckForCheck[i, j] = array[i, j];
                }
            }
            CheckForCheck[y2,x2] = CheckForCheck[y1,x1];
            CheckForCheck[y1,x1] = string.Empty;
            if (isInCheck(isWhiteHasValue(piece), CheckForCheck))
                return false;
        } // Makes sure player is not walking into check

        if (array[y2, x2] != string.Empty && isWhiteHasValue(array[y2, x2]) == isWhiteHasValue(piece)) // Makes sure player is not stepping over their own piece
            return false;

        switch (piece)
        {
            case "P": //white pawn
                if (y1 == 1)
                {
                    if ((dy == 1 || dy == 2) && dx == 0 && array[y2,x2] == string.Empty)
                        return true;
                }
                else
                {
                    if (dy == 1 && dx == 0 && array[y2, x2] == string.Empty)
                        return true;
                }
                if (dy == 1 && (dx == 1 || dx == -1) && array[y2, x2] != string.Empty && !isWhiteHasValue(array[y2, x2]))
                    return true;

                break;
            case "p": //black pawn
                if (y1 == 6)
                {
                    if ((dy == -1 || dy == -2) && dx == 0 && array[y2, x2] == string.Empty)
                        return true;
                }
                else
                {
                    if (dy == -1 && dx == 0 && array[y2, x2] == string.Empty)
                        return true;
                }
                if (dy == -1 && (dx == 1 || dx == -1) && array[y2, x2] != string.Empty && isWhiteHasValue(array[y2, x2]))
                    return true;
                break;
            case "B": case "b": //bishops
                if (dx == dy)
                {
                    if (dx > 0)
                    {
                        for (int i = 1; i < dx; i++)
                        {
                            if (array[y1 + i, x1 + i] != string.Empty)
                                return false;
                        }
                    }
                    else
                    {
                        for(int i = -1; i > dx; i--)
                        {
                            if (array[y1 + i, x1 + i] != string.Empty)
                                return false;
                        }
                    }
                }
                else if(dx == -dy)
                {
                    if(dx>0)
                    {
                        for (int i = 1; i < dx; i++)
                        {
                            if (array[y1 - i, x1 + i] != string.Empty)
                                return false;
                        }
                    }
                    else
                    {
                        for (int i = 1; i < dy; i++)
                        {
                            if (array[y1 + i, x1 - i] != string.Empty)
                                return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
                return true;
            case "r": case "R": //rook
                if (dx > 0 && dy == 0)
                {
                    for (int i = 1; i < dx; i++)
                    {
                        if (array[y1, x1 + i] != string.Empty)
                            return false;
                    }
                    return true;
                }
                if (dx < 0 && dy == 0)
                {
                    for (int i = -1; i > dx; i--)
                    {
                        if (array[y1, x1 + i] != string.Empty)
                            return false;
                    }
                    return true;
                }
                if (dx == 0 && dy > 0)
                {
                    for (int i = 1; i < dy; i++)
                    {
                        if (array[y1 + i, x1] != string.Empty)
                            return false;
                    }
                    return true;
                }
                if (dx == 0 && dy < 0)
                {
                    for (int i = -1; i > dy; i--)
                    {
                        if (array[y1 + i, x1] != string.Empty)
                            return false;
                    }
                    return true;
                }
                return false;
            case "Q": case "q": //queen
                if (dx == dy)
                {
                    if (dx > 0)
                    {
                        for (int i = 1; i < dx; i++)
                        {
                            if (array[y1 + i, x1 + i] != string.Empty)
                                return false;
                        }
                        return true;
                    }
                    else
                    {
                        for (int i = -1; i > dx; i--)
                        {
                            if (array[y1 + i, x1 + i] != string.Empty)
                                return false;
                        }
                        return true;
                    }
                }
                if (dx == -dy)
                {
                    if (dx > 0)
                    {
                        for (int i = 1; i < dx; i++)
                        {
                            if (array[y1 - i, x1 + i] != string.Empty)
                                return false;
                        }
                        return true;
                    }
                    else
                    {
                        for (int i = 1; i < dy; i++)
                        {
                            if (array[y1 + i, x1 - i] != string.Empty)
                                return false;
                        }
                        return true;
                    }
                }
                if (dx > 0 && dy == 0)
                {
                    for (int i = 1; i < dx; i++)
                    {
                        if (array[y1, x1 + i] != string.Empty)
                            return false;
                    }
                    return true;
                }
                if (dx < 0 && dy == 0)
                {
                    for (int i = -1; i > dx; i--)
                    {
                        if (array[y1, x1 + i] != string.Empty)
                            return false;
                    }
                    return true;
                }
                if (dx == 0 && dy > 0)
                {
                    for (int i = 1; i < dy; i++)
                    {
                        if (array[y1 + i, x1] != string.Empty)
                            return false;
                    }
                    return true;
                }
                if (dx == 0 && dy < 0)
                {
                    for (int i = -1; i > dy; i--)
                    {
                        if (array[y1 + i, x1] != string.Empty)
                            return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            case "n": case "N": //knight
                if (((dx == 2 || dx == -2) && (dy == 1 || dy == -1)) || ((dx == 1 || dx == -1) && (dy == 2 || dy == -2)))
                    return true;
                break;
            case "K": case "k": //king
                if (dx >= -1 && dx <= 1 && dy >= -1 && dy <= 1)
                    return true;
                if (dx == 2 && dy == 0 && piece == "K" && y1 == 0 && x1 == 4 && array[0, 5] == string.Empty && array[0, 6] == string.Empty && array[0, 7] == "R" && WhiteCanCastleRight && !isInCheck("K",Pieces))
                {
                    if (isReal)
                    {
                        array[0, 7] = string.Empty;
                        array[0, 5] = "R";
                    }
                    return true;
                }
                if (dx == -2 && dy == 0 && piece == "K" && y1 == 0 && x1 == 4 && array[0, 3] == string.Empty && array[0, 2] == string.Empty && array[0, 1] == string.Empty && array[0, 0] == "R" && WhiteCanCastleLeft && !isInCheck("K", Pieces))
                {
                    if (isReal)
                    {
                        array[0, 0] = string.Empty;
                        array[0, 3] = "R";
                    }
                    return true;
                }
                if (dx == 2 && dy == 0 && piece == "k" && y1 == 7 && x1 == 4 && array[7, 5] == string.Empty && array[7, 6] == string.Empty && array[7, 7] == "r" && BlackCanCastleRight && !isInCheck("k", Pieces))
                {
                    if (isReal)
                    {
                        array[7, 7] = string.Empty;
                        array[7, 5] = "r";
                    }
                    return true;
                }
                if (dx == -2 && dy == 0 && piece == "k" && y1 == 7 && x1 == 4 && array[7, 3] == string.Empty && array[7, 2] == string.Empty && array[7, 1] == string.Empty && array[7, 0] == "r" && BlackCanCastleLeft && !isInCheck("k", Pieces))
                {
                    if (isReal)
                    {
                        array[7, 0] = string.Empty;
                        array[7, 3] = "r";
                    }
                    return true;
                }
                break;
        }
        return false;
    }

    public void EnableButtonToPromote()
    {
        buttonCanvas.SetActive(true);
    }

    public void Promote(string piece)
    {
        if(PromoteWhite)
        {
            Pieces[PromotionPos.y, PromotionPos.x] = piece.ToUpper();
        }
        else
        {
            Pieces[PromotionPos.y, PromotionPos.x] = piece;
        }
        boardOperations.DrawBoard();
        buttonCanvas.SetActive(false);
        Ongoing = true;
    }
    public List<string[,]> AllMovesOfAColor(string[,] array, bool white)
    {
        List<string[,]> list = new List<string[,]>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (isWhite(array[j, i]) == white)
                {
                    list.AddRange(AllMovesOfAPiece(array, i, j));
                }
            }
        }
        return list;
    }

    public List<string[,]> AllMovesOfAPiece(string[,] array, int x, int y)
    {
        List<string[,]> listOfPositions = new List<string[,]>();
        for (int k = 0; k < 8; k++)
        {
            for (int l = 0; l < 8; l++)
            {
                if (LegalMove(x, k, y, l, array[y, x], true, array, false))
                {

                    string[,] alteredPosition = new string[8, 8];
                    alteredPosition = (string[,])array.Clone();
                    alteredPosition[l, k] = alteredPosition[y, x];
                    alteredPosition[y, x] = string.Empty;
                    if (alteredPosition[l, k] == "P" && l == 7)
                        alteredPosition[l, k] = "Q";
                    if (alteredPosition[l, k] == "p" && l == 0)
                        alteredPosition[l, k] = "q";
                    int dx = k - x;
                    if (alteredPosition[l, k] == "K" && dx == 2)
                    {
                        alteredPosition[0, 7] = string.Empty;
                        alteredPosition[0, 5] = "R";
                    }
                    if (alteredPosition[l, k] == "K" && dx == -2)
                    {
                        alteredPosition[0, 0] = string.Empty;
                        alteredPosition[0, 3] = "R";
                    }
                    if (alteredPosition[l, k] == "k" && dx == 2)
                    {
                        alteredPosition[7, 7] = string.Empty;
                        alteredPosition[7, 5] = "r";
                    }
                    if (alteredPosition[l, k] == "k" && dx == -2)
                    {
                        alteredPosition[7, 0] = string.Empty;
                        alteredPosition[7, 3] = "r";
                    }
                    listOfPositions.Add(alteredPosition);
                }
            }
        }
        return listOfPositions;
    } // returns board after move

    public List<Vector2Int> AllMovesOfAPiece (int x, int y, string piece, string[,] array) // returns position of piece
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if(LegalMove(x,i,y,j,piece,true,array,false))
                {
                    list.Add(new Vector2Int(i, j));
                }
            }
        }
        return list;
    }

    public int MateStatus(bool White, string[,] array)
    {
        bool check = isInCheck(White, array);
        bool canMove = false;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (isWhite(array[j,i]) == White)
                {
                    foreach (Vector2Int Position in AllMovesOfAPiece(i, j, array[j, i], array))
                    {
                        string[,] CheckForCheckmate = new string[8, 8];
                        for (int x = 0; x < 8; x++)
                        {
                            for (int y = 0; y < 8; y++)
                            {
                                CheckForCheckmate[y, x] = array[y, x];
                            }
                        }
                        CheckForCheckmate[Position.y, Position.x] = CheckForCheckmate[j, i];
                        CheckForCheckmate[j, i] = string.Empty;
                        if (!isInCheck(White, CheckForCheckmate))
                            return 0;
                    }
                }
            }
        }
        if (!canMove && check)
            return 1;
        if (!canMove && !check)
            return 2;
        return -999;
    }

    public bool isInCheck(string king, string[,] array)
    {
        Vector2Int KingPos = new Vector2Int();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if(array[i,j] == king)
                {
                    KingPos = new Vector2Int(j, i);
                }
            }
        }
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if(array[j,i] != string.Empty && isWhiteHasValue(array[j,i]) != isWhiteHasValue(array[KingPos.y,KingPos.x]))
                {
                    if (LegalMove(i, KingPos.x, j, KingPos.y, array[j, i], false, array, false))
                        return true;
                }
            }
        }
        return false;
    }

    public bool isInCheck(bool white, string[,] array)
    {
        string king;
        if (white)
            king = "K";
        else
            king = "k";

        Vector2Int KingPos = new Vector2Int();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (array[i,j] == king)
                {
                    KingPos = new Vector2Int(j, i);
                }
            }
        }
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (array[j, i] != string.Empty && isWhiteHasValue(array[j, i]) != white)
                {
                    if (LegalMove(i, KingPos.x, j, KingPos.y, array[j, i], false, array,false))
                        return true;
                }
            }
        }
        return false;
    }

    public bool? isWhite(string s)
    {
        if (s != string.Empty)
            return (s.ToUpper().Equals(s));
        return null;
    }
    public bool isWhiteHasValue(string s)
    {
        return (s.ToUpper().Equals(s));
    }

    public int NumberPieces(string[,] board)
    {
        int sum = 0;
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if (board[i, j] != string.Empty)
                    sum++;
            }
        }
        return sum;
    }
}