using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ArtificialIntelligence : MonoBehaviour
{
    string[,] Pieces = new string[8, 8];
    GameController controller;
    string[,] bestPosition;
    public int maxDepth;

    void Start()
    {
        controller = GetComponentInParent<GameController>();
    }

    public int CalculateDepth(string[,] Pieces)
    {
        decimal numPieces = Convert.ToDecimal(controller.NumberPieces(Pieces));
        int easyMode = 0;
        if (controller.easyMode)
            easyMode = -1;
        if (numPieces == 3)
            return 6 + easyMode;
        if (numPieces <= 5)
            return 5 + easyMode;
        return 3 + easyMode;
    }

    public string[,] AIBestMove(bool color=false)
    {
        Pieces = controller.Pieces;
        maxDepth = CalculateDepth(Pieces);
        minimax(Pieces, maxDepth, -int.MaxValue, int.MaxValue, color);
        return bestPosition;
    }

    public void DoHint()
    {
        controller.Pieces = AIBestMove(true);
        controller.boardOperations.DrawBoard();
        controller.WhiteTurn = false;
    }
    
    public void MoveBlack()
    {
        controller.Pieces = AIBestMove();
        controller.boardOperations.DrawBoard();
        controller.WhiteTurn = true;
    }

    public int minimax(string[,] position, int depth, int alpha, int beta, bool isWhite)
    {
        if (depth == 0)
            return EvaluatePosition(position);

        if (isWhite)
        {
            int maxEvaluation = -int.MaxValue; // Use -int.MaxValue because cant negate int.MinValue
            List<string[,]> AllMovesOfAColor = controller.AllMovesOfAColor(position, true);
            for (int i = 0; i < AllMovesOfAColor.Count; i++)
            {
                int evaluation = minimax(AllMovesOfAColor[i], depth - 1, alpha, beta, false);
                if (Math.Abs(evaluation) > 200) // Some Mate found in future
                    evaluation /= 2;
                
                alpha = Mathf.Max(alpha, evaluation);
                if (evaluation > maxEvaluation)
                {
                    maxEvaluation = evaluation;
                    if (maxDepth == depth)
                    {
                        bestPosition = AllMovesOfAColor[i];
                    }
                }
                if (beta <= alpha)
                    break;
            }
            return maxEvaluation;
        }

        else
        {
            int minEvaluation = int.MaxValue;
            List<string[,]> AllMovesOfAColor = controller.AllMovesOfAColor(position, false);
            for (int i = 0; i < AllMovesOfAColor.Count; i++)
            {
                int evaluation = minimax(AllMovesOfAColor[i], depth - 1, alpha, beta, true);
                if (Math.Abs(evaluation) > 200) // Some Mate found in future
                    evaluation /= 2;
                beta = Mathf.Min(beta, evaluation);
                if(evaluation < minEvaluation)
                {
                    minEvaluation = evaluation;
                    if (maxDepth == depth)
                    {
                        bestPosition = AllMovesOfAColor[i];
                    }
                }
                if (beta <= alpha)
                    break;
            }
            return minEvaluation;
        }

    }
    
    public int EvaluatePosition(string[,] array)
    {
        if (controller.MateStatus(true, array) == 2 || controller.MateStatus(false, array) == 2) //stalemate
        {
            return 0;
        }
        int sum = 0;
        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                if(array[j,i] != string.Empty)
                {
                    int White;
                    if (controller.isWhiteHasValue(array[j, i]))
                        White = 1;
                    else
                        White = -1;
                    int piece = 0;
                    switch (array[j,i])
                    {
                        case "p": case "P":
                            piece = 1;
                            break;

                        case "r": case "R":
                            piece = 5;
                            break;

                        case "n": case "N":
                            piece = 3;
                            break;

                        case "b": case "B":
                            piece = 3;
                            break;

                        case "q": case "Q":
                            piece = 9;
                            break;

                        case "k": case "K":
                            piece = 100000;
                            break;
                    }
                    sum += White * piece;
                }
            }
        }
        return sum;
    }

}
