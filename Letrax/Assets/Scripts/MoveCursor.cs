using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCursor : MonoBehaviour
{
    public int newRow; 
    public int newCol; 

    public void MoveCursorToNewLetter()
    {
        if (GameManager.instance.row == newRow)
        {
            GameManager.instance.ManageCursor(GameManager.instance.row, GameManager.instance.col, false);
            GameManager.instance.row = newRow;
            GameManager.instance.col = newCol;
            GameManager.instance.ManageCursor(GameManager.instance.row, GameManager.instance.col, true);
        }
    }
}
