using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private Exception unsolvable;
    private static readonly int SIDE = 9; // Sudoku grid side length
    private bool valueChanged; // True when at least one value is changed

    public float square_offset = 0.0f;
    public float square3X3_offset = 0.0f;
    public GameObject grid_square, SudokuSolved, SudokuUnsolved, SolveButton, BruteForceError;
    private Vector2 start_position;
    public float square_scale = 1.0f;
    private int[] lastCellCheck = new int[2];
    private int[,] randomGrid = new int[SIDE, SIDE]; // Contains the random grid
    private bool isSudokuSolved;

    public GameObject[,] Cella = new GameObject[SIDE, SIDE];

    void Start()
    {
        start_position = new Vector2(-(square_scale + square_offset) * 2f + square3X3_offset, -(square_scale + square_offset) * 2f + square3X3_offset);
        SudokuSolved.SetActive(false);
        SudokuUnsolved.SetActive(false);
        SolveButton.SetActive(true);
        BruteForceError.SetActive(false);
        lastCellCheck[0] = -1;
        lastCellCheck[1] = 0;
        isSudokuSolved = false;
        SpawnGridSquare();
    }

    private void SpawnGridSquare()
    {
        float Xsquare3x3_offset = 0; // Allows spacing columns
        float Ysquare3x3_offset; // Allows spacing rows
        for (int i = 0; i < SIDE; i++)
        {
            Ysquare3x3_offset = 0;
            for (int j = 0; j < SIDE; j++) // Generate squares
            {
                Cella[i, j] = Instantiate(grid_square, new Vector2(start_position.x + i * square_offset + Xsquare3x3_offset, start_position.y + j * square_offset + Ysquare3x3_offset), new Quaternion(0, 0, 0, 0));
                Cella[i, j].transform.SetParent(this.transform, true);
                Cella[i, j].transform.localScale = new Vector3(square_scale, square_scale, square_scale);
                Cella[i, j].GetComponent<CellS>().setPosition(i, j);

                if ((j + 1) % 3 == 0) // Space cells in 3x3 cubes
                {
                    Ysquare3x3_offset += square3X3_offset;
                }
            }
            if ((i + 1) % 3 == 0) // Space cells in 3x3 cubes
            {
                Xsquare3x3_offset += square3X3_offset;
            }
        }
    }

    // Attempt to solve the Sudoku
    public void Play()
    {
        SolveButton.SetActive(false);
        try
        {
            FirstCheck();
            NumChooseFounder();
            SolveSudoku();
        }
        catch
        {
            // UNSOLVABLE SUDOKU
            SudokuUnsolved.SetActive(true);
        }
    }

    // Check if there are duplicate numbers in the same rows, columns, or 3x3 cells
    public void FirstCheck()
    {
        int temp;
        // Check rows
        for (int y = 0; y < SIDE; y++)
        {
            for (int x = 0; x < SIDE - 1; x++)
            {
                if (Cella[x, y].GetComponent<CellS>().isNumDecided)
                {
                    temp = Cella[x, y].GetComponent<CellS>().number;
                    for (int i = x + 1; i < SIDE; i++)
                    {
                        if (Cella[i, y].GetComponent<CellS>().isNumDecided && temp == Cella[i, y].GetComponent<CellS>().number)
                        {
                            throw unsolvable;
                        }
                    }
                }
            }
        }

        // Check columns
        for (int x = 0; x < SIDE; x++)
        {
            for (int y = 0; y < SIDE - 1; y++)
            {
                if (Cella[x, y].GetComponent<CellS>().isNumDecided)
                {
                    temp = Cella[x, y].GetComponent<CellS>().number;
                    for (int i = y + 1; i < SIDE; i++)
                    {
                        if (Cella[x, i].GetComponent<CellS>().isNumDecided && temp == Cella[x, i].GetComponent<CellS>().number)
                        {
                            throw unsolvable;
                        }
                    }
                }
            }
        }

        // Check 3x3 boxes
        for (int yBox = 0; yBox < SIDE; yBox += 3)
        {
            for (int xBox = 0; xBox < SIDE; xBox += 3)
            {
                List<int> ListNums = new List<int>();
                for (int y = yBox; y < yBox + 3; y++)
                {
                    for (int x = xBox; x < xBox + 3; x++)
                    {
                        if (Cella[x, y].GetComponent<CellS>().isNumDecided)
                        {
                            ListNums.Add(Cella[x, y].GetComponent<CellS>().number);
                        }
                    }
                }

                int[] ArrayNum = ListNums.ToArray();
                for (int i = 0; i < ArrayNum.Length - 1; i++)
                {
                    for (int j = i + 1; j < ArrayNum.Length; j++)
                    {
                        if (ArrayNum[i] == ArrayNum[j])
                        {
                            throw unsolvable;
                        }
                    }
                }
            }
        }
    }

    // Make the row, column, and 3x3 cell values false
    public void FalseMaker(int Xpos, int Ypos, int num)
    {
        // Make row values false
        for (int x = 0; x < SIDE; x++)
        {
            Cella[x, Ypos].GetComponent<CellS>().possibleCellNumber[num - 1] = false;
        }

        // Make column values false
        for (int y = 0; y < SIDE; y++)
        {
            Cella[Xpos, y].GetComponent<CellS>().possibleCellNumber[num - 1] = false;
        }

        // Calculate the position of the cell belonging to the 3x3 box
        int yBox = Ypos - (Ypos % 3);
        int xBox = Xpos - (Xpos % 3);
        for (int y = yBox; y < yBox + 3; y++)
        {
            for (int x = xBox; x < xBox + 3; x++)
            {
                Cella[x, y].GetComponent<CellS>().possibleCellNumber[num - 1] = false;
            }
        }

        Cella[Xpos, Ypos].GetComponent<CellS>().possibleCellNumber[num - 1] = true;
        valueChanged = true;
    }

    // Find the cells that already have chosen numbers
    private void NumChooseFounder()
    {
        for (int y = 0; y < SIDE; y++)
        {
            for (int x = 0; x < SIDE; x++)
            {
                if (Cella[x, y].GetComponent<CellS>().isNumDecided && Cella[x, y].GetComponent<CellS>().number != 0 && Cella[x, y].GetComponent<CellS>().number != 12)
                {
                    FalseMaker(x, y, Cella[x, y].GetComponent<CellS>().number);
                }
            }
        }
    }

    // Solve the Sudoku by calling functions in the correct order
    private void SolveSudoku()
    {
        while (!CheckIfSudokuIsSolved())
        {
            valueChanged = false;

            CellCheck();
            if (valueChanged) continue;

            RowCheck();
            if (valueChanged) continue;

            ColCheck();
            if (valueChanged) continue;

            Box3x3Check();
            if (valueChanged) continue;

            EqualPairTrue(lastCellCheck[0], lastCellCheck[1]);
            if (valueChanged) continue;

            RandomGridInitializer();
            if (SudokuAttender(randomGrid, 0, 0))
            {
                RandomGridEnd();
                BruteForceError.SetActive(true);
                break;
            }
            else
            {
                throw unsolvable;
            }
        }

        SolveButton.SetActive(false);
        SudokuSolved.SetActive(true);
        isSudokuSolved = true;
    }

    // Check if the cell can store at most one number
    private void CellCheck()
    {
        int nAllowedNum;
        for (int y = 0; y < SIDE; y++)
        {
            for (int x = 0; x < SIDE; x++)
            {
                if (!Cella[x, y].GetComponent<CellS>().isNumDecided)
                {
                    nAllowedNum = Cella[x, y].GetComponent<CellS>().CellCeck();

                    if (nAllowedNum == 0)
                    {
                        throw unsolvable;
                    }
                    else if (nAllowedNum == 1)
                    {
                        int allowedNum = 11;
                        for (int i = 0; i < SIDE; i++)
                        {
                            if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[i])
                            {
                                allowedNum = i;
                            }
                        }
                        if (allowedNum < 10)
                        {
                            Cella[x, y].GetComponent<CellS>().InsertNum(allowedNum + 1);
                            FalseMaker(x, y, allowedNum + 1);
                        }
                        else
                        {
                            throw unsolvable;
                        }
                    }
                }
            }
        }
    }

    // Check if a number can be stored in only one cell in a row
    private void RowCheck()
    {
        bool assignValue;
        int XcellTemp;
        for (int num = 0; num < SIDE; num++)
        {
            for (int y = 0; y < SIDE; y++)
            {
                assignValue = true;
                XcellTemp = 11;
                for (int x = 0; x < SIDE; x++)
                {
                    if (Cella[x, y].GetComponent<CellS>().isNumDecided)
                    {
                        if (Cella[x, y].GetComponent<CellS>().number == (num + 1))
                        {
                            assignValue = false;
                            break;
                        }
                    }
                    else if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[num])
                    {
                        if (XcellTemp == 11)
                        {
                            XcellTemp = x;
                        }
                        else
                        {
                            assignValue = false;
                            break;
                        }
                    }
                }
                if (assignValue)
                {
                    Cella[XcellTemp, y].GetComponent<CellS>().InsertNum(num + 1);
                    FalseMaker(XcellTemp, y, num + 1);
                    return;
                }
            }
        }
    }

    // Check if a number can be stored in only one cell in a column
    private void ColCheck()
    {
        bool assignValue;
        int ycellTemp;
        for (int num = 0; num < SIDE; num++)
        {
            for (int x = 0; x < SIDE; x++)
            {
                assignValue = true;
                ycellTemp = 11;
                for (int y = 0; y < SIDE; y++)
                {
                    if (Cella[x, y].GetComponent<CellS>().isNumDecided)
                    {
                        if (Cella[x, y].GetComponent<CellS>().number == (num + 1))
                        {
                            assignValue = false;
                            break;
                        }
                    }
                    else if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[num])
                    {
                        if (ycellTemp == 11)
                        {
                            ycellTemp = y;
                        }
                        else
                        {
                            assignValue = false;
                            break;
                        }
                    }
                }
                if (assignValue)
                {
                    Cella[x, ycellTemp].GetComponent<CellS>().InsertNum(num + 1);
                    FalseMaker(x, ycellTemp, num + 1);
                    return;
                }
            }
        }
    }

    // Check if a number can be stored in only one cell in a 3x3 box
    private void Box3x3Check()
    {
        bool assignValue;
        int xcellTemp, ycellTemp;
        for (int num = 0; num < SIDE; num++)
        {
            for (int yBox = 0; yBox < SIDE; yBox += 3)
            {
                for (int xBox = 0; xBox < SIDE; xBox += 3)
                {
                    assignValue = true;
                    ycellTemp = xcellTemp = 11;
                    for (int y = yBox; y < yBox + 3; y++)
                    {
                        for (int x = xBox; x < xBox + 3; x++)
                        {
                            if (Cella[x, y].GetComponent<CellS>().isNumDecided)
                            {
                                if (Cella[x, y].GetComponent<CellS>().number == (num + 1))
                                {
                                    assignValue = false;
                                    break;
                                }
                            }
                            else if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[num])
                            {
                                if (ycellTemp == 11 && xcellTemp == 11)
                                {
                                    xcellTemp = x;
                                    ycellTemp = y;
                                }
                                else
                                {
                                    assignValue = false;
                                    break;
                                }
                            }
                        }
                        if (!assignValue) break;
                    }

                    if (assignValue)
                    {
                        Cella[xcellTemp, ycellTemp].GetComponent<CellS>().InsertNum(num + 1);
                        FalseMaker(xcellTemp, ycellTemp, num + 1);
                        return;
                    }
                }
            }
        }
    }

    // Check for pairs of cells that can only have the same two numbers
    private void EqualPairTrue(int Xstart, int Ystart)
    {
        bool found1 = false;
        int[,] coordinate = new int[2, 2];
        int[,] possibleValue = new int[2, 2];
        possibleValue[0, 0] = 12;

        for (int y = Ystart; y < SIDE; y++)
        {
            int x = (y == Ystart) ? Xstart + 1 : 0;
            while (x < SIDE)
            {
                if (Cella[x, y].GetComponent<CellS>().TrueCounter() == 2)
                {
                    coordinate[0, 0] = x;
                    coordinate[0, 1] = y;
                    for (int i = 0; i < SIDE; i++)
                    {
                        if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[i])
                        {
                            if (possibleValue[0, 0] == 12)
                            {
                                possibleValue[0, 0] = i;
                            }
                            else
                            {
                                possibleValue[0, 1] = i;
                                found1 = true;
                                break;
                            }
                        }
                    }
                }
                if (found1) break;
                x++;
            }
            if (found1) break;
        }
        if (!found1) return;

        bool found2 = false;
        possibleValue[1, 0] = 12;

        for (int x = coordinate[0, 0] + 1; x < SIDE; x++)
        {
            if (Cella[x, coordinate[0, 1]].GetComponent<CellS>().TrueCounter() == 2)
            {
                coordinate[1, 0] = x;
                coordinate[1, 1] = coordinate[0, 1];
                for (int i = 0; i < SIDE; i++)
                {
                    if (Cella[x, coordinate[0, 1]].GetComponent<CellS>().possibleCellNumber[i])
                    {
                        if (possibleValue[1, 0] == 12)
                        {
                            if (possibleValue[0, 0] == i)
                            {
                                possibleValue[1, 0] = i;
                            }
                            else break;
                        }
                        else if (possibleValue[0, 1] == i)
                        {
                            possibleValue[1, 1] = i;
                            found2 = true;
                            break;
                        }
                    }
                }
                if (found2) break;
            }
        }

        lastCellCheck[0] = coordinate[0, 0];
        lastCellCheck[1] = coordinate[0, 1];
        if (found2)
        {
            PrecisedRowFalsificator(coordinate, possibleValue);
            valueChanged = true;
            return;
        }
        else
        {
            EqualPairTrue(lastCellCheck[0], lastCellCheck[1]);
            possibleValue[1, 0] = 12;
        }
    }

    // If two values are present only in two cells, those cells cannot host other numbers and other cells in the row cannot host the two values
    private void EqualPairTrue2()
    {
        for (int y = 0; y < SIDE; y++)
        {
            for (int num = 0; num < SIDE; num++)
            {
                for (int x = 0; x < SIDE; x++)
                {
                    if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[num]) { }
                }
            }
        }
    }

    // Make specific values false in the row, column, or 3x3 box containing the two cells
    private void PrecisedRowFalsificator(int[,] coordinate, int[,] values)
    {
        for (int x = 0; x < SIDE; x++)
        {
            if (x != coordinate[0, 0] && x != coordinate[1, 0])
            {
                Cella[x, coordinate[0, 1]].GetComponent<CellS>().possibleCellNumber[values[0, 0]] = false;
                Cella[x, coordinate[0, 1]].GetComponent<CellS>().possibleCellNumber[values[0, 1]] = false;
            }
        }
    }

    // Initialize the random grid
    private void RandomGridInitializer()
    {
        for (int x = 0; x < SIDE; x++)
        {
            for (int y = 0; y < SIDE; y++)
            {
                if (Cella[x, y].GetComponent<CellS>().number == 12)
                {
                    randomGrid[x, y] = 0;
                }
                else
                {
                    randomGrid[x, y] = Cella[x, y].GetComponent<CellS>().number;
                }
            }
        }
    }

    // Finalize the random grid with the solved values
    private void RandomGridEnd()
    {
        for (int x = 0; x < SIDE; x++)
        {
            for (int y = 0; y < SIDE; y++)
            {
                Cella[x, y].GetComponent<CellS>().InsertNum(randomGrid[x, y]);
            }
        }
    }

    // Check if it is safe to place a number in the given cell
    bool isSafe(int[,] randomGrid, int row, int col, int num)
    {
        // Check the row for the same number
        for (int x = 0; x < SIDE; x++)
        {
            if (randomGrid[row, x] == num)
            {
                return false;
            }
        }

        // Check the column for the same number
        for (int x = 0; x < SIDE; x++)
        {
            if (randomGrid[x, col] == num)
            {
                return false;
            }
        }

        // Check the 3x3 subgrid for the same number
        int startRow = row - row % 3;
        int startCol = col - col % 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (randomGrid[i + startRow, j + startCol] == num)
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Recursive function to solve the Sudoku using backtracking
    private bool SudokuAttender(int[,] randomGrid, int row, int col)
    {
        // If we have reached the last cell, the Sudoku is solved
        if (row == SIDE - 1 && col == SIDE)
        {
            return true;
        }

        // Move to the next row if we have reached the end of the current row
        if (col == SIDE)
        {
            row++;
            col = 0;
        }

        // Skip cells that are already filled
        if (randomGrid[row, col] > 0)
        {
            return SudokuAttender(randomGrid, row, col + 1);
        }

        // Try placing numbers from 1 to 9 in the current cell
        for (int num = 1; num <= SIDE; num++)
        {
            if (isSafe(randomGrid, row, col, num))
            {
                randomGrid[row, col] = num;

                // Recursively solve the rest of the Sudoku
                if (SudokuAttender(randomGrid, row, col + 1))
                {
                    return true;
                }

                // If the number does not lead to a solution, reset the cell
                randomGrid[row, col] = 0;
            }
        }

        return false;
    }

    // Check if the Sudoku is completely solved
    bool CheckIfSudokuIsSolved()
    {
        for (int x = 0; x < SIDE; x++)
        {
            for (int y = 0; y < SIDE; y++)
            {
                if (!Cella[x, y].GetComponent<CellS>().isNumDecided)
                {
                    return false;
                }
            }
        }
        return true;
    }
}