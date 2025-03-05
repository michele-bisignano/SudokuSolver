using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    Exception unsolvable;
    int side = 9; //sudoku side
    bool valueChanged;//true when at least a value is changed

    public float square_offset = 0.0f;
    public float square3X3_offset = 0.0f;
    public GameObject grid_square, SudokuSolved, SudokuUnsolved, SolveButton;
    private Vector2 start_position;
    public float square_scale = 1.0f;
    private int[] lastCellCheck = new int[2];
    private int[,] randomGrid = new int[9, 9];//contiene la griglia randomica

    public GameObject[,] Cella = new GameObject[9, 9];


    void Start()
    {
        start_position = new Vector2(-(square_scale+ square_offset )* 2f+ square3X3_offset, -(square_scale + square_offset) * 2.15f + square3X3_offset);
        SudokuSolved.SetActive(false);
        SudokuUnsolved.SetActive(false);
        SolveButton.SetActive(true);
        lastCellCheck[0] = -1;
        lastCellCheck[1] = 0;
        SpawnGridSquare();
    }
    private void SpawnGridSquare()
    {
        float Xsquare3x3_offset = 0;//permette di calcolare le colonne con più spazio
        float Ysquare3x3_offset;//permette di calcolare le righe con più spazio
        for (int i = 0; i < side; i++)
        {
            Ysquare3x3_offset = 0;
            for (int j = 0; j < side; j++) //generate squares
            {

                Cella[i, j] = Instantiate(grid_square, new Vector2(start_position.x + i * square_offset + Xsquare3x3_offset, start_position.y + j * square_offset + Ysquare3x3_offset), new Quaternion(0, 0, 0, 0));
                Cella[i, j].transform.SetParent(this.transform, true);
                Cella[i, j].transform.localScale = new Vector3(square_scale, square_scale, square_scale);
                Cella[i, j].GetComponent<CellS>().setPosition(i, j);

                if ((j + 1) % 3 == 0)// DISTANZIA LE CELLE IN CUBI 3x3
                {
                    Ysquare3x3_offset += square3X3_offset;
                }
            }
            if ((i + 1) % 3 == 0)// DISTANZIA LE CELLE IN CUBI 3x3
            {
                Xsquare3x3_offset += square3X3_offset;
            }
        }
    }
    //tentativo
    public void Play()//define the functions order
    {

        SolveButton.SetActive(false);

        try
        {
        FirstCheck();
        NumChooseFounder();
        RecursiveFunction();
        }
        catch
        {
            //SUDOKU IRRISOLVIBILE
            SudokuUnsolved.SetActive(true);
        }
    }

    public void FirstCheck()//Check if there are 2 identical numbers in the same rows, col, 3x3cell
    {
        int temp;//numero del quale stiamo cercando un eventuale coppione
        //row
        for (int y = 0; y < side; y++)
        {
            for (int x = 0; x < side - 1; x++)
            {
                if (Cella[x, y].GetComponent<CellS>().isNumDecided)//true if the cell ISN'T void
                {
                    temp = Cella[x, y].GetComponent<CellS>().number;
                    for (int i = x + 1; i < side; i++)
                    {
                        if (Cella[i, y].GetComponent<CellS>().isNumDecided)
                            if (temp == Cella[i, y].GetComponent<CellS>().number)//If there are 2 identical number it is usolvable
                            {
                                throw unsolvable;
                            }
                    }
                }
            }
        }

        //col
        for (int x = 0; x < side; x++)
            for (int y = 0; y < side - 1; y++)
            {
                if (Cella[x, y].GetComponent<CellS>().isNumDecided)//true if the cell ISN'T void
                {
                    temp = Cella[x, y].GetComponent<CellS>().number;
                    for (int i = y + 1; i < side; i++)
                    {
                        if (Cella[x, i].GetComponent<CellS>().isNumDecided)
                            if (temp == Cella[x, i].GetComponent<CellS>().number)//If there are 2 identical number it is usolvable
                            {
                                throw unsolvable;
                            }
                    }
                }
            }

        //3x3box
        //salvo tutti i numeri del box in una lista, così poi posso controllare eventuali ripetizioni con più facilità
        for (int yBox = 0; yBox < 9; yBox += 3)//sposta alla casella 3X3 giù
            for (int xBox = 0; xBox < 9; xBox += 3)//sposta alla casella 3X3 a dx
            {
                List<int> ListNums = new List<int>();//contains the num in the 3x3box
                for (int y = yBox; y < yBox + 3; y++)//sposta alla riga successiva
                {
                    for (int x = xBox; x < xBox + 3; x++)//seleziona le 3 caselle di ciascuna cella 3X3
                    {
                        if (Cella[x, y].GetComponent<CellS>().isNumDecided)//true if the cell ISN'T void
                        {
                            ListNums.Add(Cella[x, y].GetComponent<CellS>().number);
                        }
                    }
                }

                int[] ArrayNum = ListNums.ToArray();//I need an array
                for (int i = 0; i < ArrayNum.Length - 1; i++)
                {
                    for (int j = i + 1; j < ArrayNum.Length; j++)
                    {
                        if (ArrayNum[i] == ArrayNum[j])//If there are 2 identical number it is usolvable
                        {
                            throw unsolvable;
                        }
                    }
                }

            }
    }

    //Make the row, col and 3x3cell falses
    public void FalseMaker(int Xpos, int Ypos, int num)//requires the cell position and the number
    {
        //row
        for (int x = 0; x < side; x++)
        {
            Cella[x, Ypos].GetComponent<CellS>().possibleCellNumber[num - 1] = false;
        }

        //col
        for (int y = 0; y < side; y++)
        {
            Cella[Xpos, y].GetComponent<CellS>().possibleCellNumber[num - 1] = false;
        }
        //calcolo la posizione della cella con coordinate minori appartenente al 3x3Box 
        int yBox = Ypos - (Ypos % 3);
        int xBox = Xpos - (Xpos % 3);
        for (int y = yBox; y < yBox + 3; y++)//sposta alla riga successiva
            for (int x = xBox; x < xBox + 3; x++)//seleziona le 3 caselle di ciascuna cella 3X3
            {
                Cella[x, y].GetComponent<CellS>().possibleCellNumber[num - 1] = false;
            }

        Cella[Xpos, Ypos].GetComponent<CellS>().possibleCellNumber[num - 1] = true;
        valueChanged = true;
    }

    private void NumChooseFounder()//Found the cell which have the number already chosen
    {
        for (int y = 0; y < side; y++)
            for (int x = 0; x < side; x++)
            {
                if (Cella[x, y].GetComponent<CellS>().isNumDecided && Cella[x, y].GetComponent<CellS>().number != 0 && Cella[x, y].GetComponent<CellS>().number != 12)
                {
                    FalseMaker(x, y, Cella[x, y].GetComponent<CellS>().number);
                }
            }
    }

    //It calls al the function with the correct order, every time that a number is found the RecursiveFunction is called
    private void RecursiveFunction()
    {
        valueChanged = false;

        CellCheck();
        if (valueChanged) RecursiveFunction();

        RowCheck();
        if (valueChanged) RecursiveFunction();

        ColCheck();
        if (valueChanged) RecursiveFunction();

        Box3x3Check();
        if (valueChanged) RecursiveFunction();

        EqualPairTrue(lastCellCheck[0], lastCellCheck[1]);
        if (valueChanged) RecursiveFunction();

        RandomGridInitializer();
        if (SudokuAttender(randomGrid, 0, 0))
        {
            RandomGridEnd();

            SolveButton.SetActive(false);
            SudokuSolved.SetActive(true);
        }
        else
            throw unsolvable;
    }
    private void CellCheck()//Check if the cell can store max a num
    {
        int nAllowedNum;//number of values that the cell can store
        for (int y = 0; y < side; y++)
            for (int x = 0; x < side; x++)
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
                        for (int i = 0; i < side; i++)//check which number is allowed
                        {
                            if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[i])
                            {
                                allowedNum = i;
                            }
                        }
                        if (allowedNum < 10)
                        {
                            //Cella[x, y].GetComponent<Text>().color = Color.green;
                            Cella[x, y].GetComponent<CellS>().InsertNum(allowedNum + 1);
                            FalseMaker(x, y, allowedNum + 1);
                        }
                        else
                            throw unsolvable;
                    }
                }
            }
    }

    private void RowCheck()//Check if inside a row the number can be store only in a cell
    {
        bool assegnaValore;//vero se nella riga num è ammesso in una sola cella
        int XcellTemp;//la cella contiene temporanemente la x della cella che ammette il numero
        for (int num = 0; num < side; num++)//select the number to check
            for (int y = 0; y < side; y++)//sposta alla riga successiva 
            {

                assegnaValore = true;
                XcellTemp = 11;
                for (int x = 0; x < side; x++)
                {
                    if (Cella[x, y].GetComponent<CellS>().isNumDecided)
                    {
                        if (Cella[x, y].GetComponent<CellS>().number == (num + 1))//se il numero è già stato scritto non faccio il controllo
                        {
                            assegnaValore = false;
                            break;
                        }
                        else continue;
                    }
                    else if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[num])
                    {
                        if (XcellTemp == 11)
                        {
                            XcellTemp = x;
                        }
                        else
                        {
                            assegnaValore = false;
                            break;//il numero è ammesso in almeno un'altra cella
                        }
                    }
                }
                if (assegnaValore)
                {
                    Cella[XcellTemp, y].GetComponent<CellS>().InsertNum(num + 1);
                    FalseMaker(XcellTemp, y, num + 1);
                    return;
                }

            }
    }

    private void ColCheck()//Check if inside a column the number can be store only in a cell
    {
        bool assegnaValore;//vero se nella riga num è ammesso in una sola cella
        int ycellTemp;//la cella contiene temporanemente la x della cella che ammette il numero
        for (int num = 0; num < side; num++)//select the number to check
            for (int x = 0; x < side; x++)//sposta alla riga successiva 
            {

                assegnaValore = true;
                ycellTemp = 11;
                for (int y = 0; y < side; y++)
                {
                    if (Cella[x, y].GetComponent<CellS>().isNumDecided)
                    {
                        if (Cella[x, y].GetComponent<CellS>().number == (num + 1))//se il numero è già stato scritto non faccio il controllo
                        {
                            assegnaValore = false;
                            break;
                        }
                        else continue;//se un numero è stato scritto controllo la cella successiva
                    }
                    else if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[num])
                    {
                        if (ycellTemp == 11)
                        {
                            ycellTemp = y;
                        }
                        else
                        {
                            assegnaValore = false;
                            break;//il numero è ammesso in almeno un'altra cella
                        }
                    }
                }
                if (assegnaValore)
                {
                    Cella[x, ycellTemp].GetComponent<CellS>().InsertNum(num + 1);
                    FalseMaker(x, ycellTemp, num + 1);
                    return;
                }

            }
    }

    private void Box3x3Check()//Check if inside 3x3box the number can be store only in a cell
    {
        bool assegnaValore;//vero se nella riga num è ammesso in una sola cella
        int xcellTemp, ycellTemp;//la cella contiene temporanemente le coordinate della cella che ammette il numero
        for (int num = 0; num < side; num++)//select the number to check
            for (int yBox = 0; yBox < side; yBox += 3)//sposta alla casella 3X3 giù
                for (int xBox = 0; xBox < side; xBox += 3)//sposta alla casella 3X3 a dx
                {
                    assegnaValore = true;
                    ycellTemp = xcellTemp = 11;
                    for (int y = yBox; y < yBox + 3; y++)//sposta alla riga successiva
                    {
                        for (int x = xBox; x < xBox + 3; x++)//seleziona le 3 caselle di ciascuna cella 3X3
                        {
                            if (Cella[x, y].GetComponent<CellS>().isNumDecided)
                            {
                                if (Cella[x, y].GetComponent<CellS>().number == (num + 1))//se il numero è già stato scritto non faccio il controllo
                                {
                                    assegnaValore = false;
                                    break;
                                }
                                else continue;
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
                                    assegnaValore = false;
                                    break;//il numero è ammesso in almeno un'altra cella
                                }

                            }
                        }
                        if (!assegnaValore) break;
                    }

                    if (assegnaValore)
                    {
                        Cella[xcellTemp, ycellTemp].GetComponent<CellS>().InsertNum(num + 1);
                        FalseMaker(xcellTemp, ycellTemp, num + 1);
                        return;
                    }
                }

    }

    //cerco le coppie di celle che possono avere 2 numeri uguali
    private void EqualPairTrue(int Xstart, int Ystart)//Le coodrinate della prima tra le due celle in cui ha trovato la coppia
    {
        //se 2 celle hanno possono avere gli stessi 2 numeri, tali valori vanno esclusi da tutte le altre celle della rigam colonna e box3x3
        bool found1 = false;//true when the first number with 2 possible values is found
        int[,] coordinate = new int[2, 2];//coordinates of the 2 cells
        int[,] possibleValue = new int[2, 2];//possible values of the 2 cells
        possibleValue[0, 0] = 12;
        for (int y = Ystart; y < side; y++)
        {
            int x;
            if (y == Ystart) { x = Xstart + 1; }
            else { x = 0; }
            while (x < side)
            {
                if (Cella[x, y].GetComponent<CellS>().TrueCounter() == 2)//se la cella ammette solo 2 valori salvo tali valori
                {
                    coordinate[0, 0] = x;
                    coordinate[0, 1] = y;
                    for (int i = 0; i < side; i++)
                    {
                        if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[i])
                        {
                            if (possibleValue[0, 0] == 12) possibleValue[0, 0] = i;
                            else
                            {
                                possibleValue[0, 1] = i;
                                found1 = true;
                                break;
                            }
                        }
                    }
                }
                if (found1) { break; }
                x++;
            }
            if (found1) { break; }
        }
        if (!found1)//Nel sudoku non c'è nessuna cella con 2 valori uguali
        {
            return;
            //CAMBIARE: controllare le celle a 3 a 3
        }

        //una volta individuata la cella e i 2 numeri che può ammettere cerco nella riga,
        //della colonna e della cella3x3 se c'è un altra cella che ammette i 2 stessi numeri

        bool found2 = false;//true when the second number with 2 possible values equals to the firs one's number
        possibleValue[1, 0] = 12;

        //poichè l'algoritmo controlla le celle in ordine crescente,
        //so che ha già controllato le celle prima a quella con i 2 valori veri,
        //dunque parto da quella dopo per cercare celle con 2 valori veri
        //controllo lungo la riga
        for (int x = coordinate[0, 0] + 1; x < side; x++)
        {
            if (Cella[x, coordinate[0, 1]].GetComponent<CellS>().TrueCounter() == 2)
            {
                coordinate[1, 0] = x;
                coordinate[1, 1] = coordinate[0, 1];
                for (int i = 0; i < side; i++)
                {
                    if (Cella[x, coordinate[0, 1]].GetComponent<CellS>().possibleCellNumber[i])
                    {
                        if (possibleValue[1, 0] == 12)
                        {
                            if (possibleValue[0, 0] == i)
                            {
                                possibleValue[1, 0] = i;
                            }
                            else { break; }
                        }
                        else if (possibleValue[0, 1] == i)
                        {
                            possibleValue[1, 1] = i;
                            found2 = true;
                            break;
                        }
                    }
                }
                if (found2) { break; }
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

    private void EqualPairTrue2()//se 2 valori sono presenti solo in 2 celle, sicuramente quelle 2 celle non possono ospitare altri numeri e le altre celle della riga non possono ospitare i 2 valori in questione
    {
        
        for(int y=0; y<side; y++)
        {
            for(int num=0;num<side; num++)
            {
                for(int x=0; x<side; x++)
                {
                    if (Cella[x, y].GetComponent<CellS>().possibleCellNumber[num]) { }
                }
            } 
        }

    }
    private void PrecisedRowFalsificator(int[,] coordinate, int[,] values)//rende falsi alcuni specifici valori della riga o colonna o box3x3 a cui appartengono le 2 celle
    {
        for(int x=0; x<side; x++)
        {
            if (x != coordinate[0, 0] && x != coordinate[1, 0])
            {
                Cella[x, coordinate[0, 1]].GetComponent<CellS>().possibleCellNumber[values[0,0]] = false;
                Cella[x, coordinate[0, 1]].GetComponent<CellS>().possibleCellNumber[values[0, 1]] = false;
            }
        }
    }

    private void RandomGridInitializer()
    {
        for (int x = 0; x < side; x++)
            for (int y = 0; y < side; y++)
            {
                if (Cella[x, y].GetComponent<CellS>().number == 12)
                    randomGrid[x, y] = 0;
                else
                    randomGrid[x,y]=Cella[x, y].GetComponent<CellS>().number;
            }
    }
    private void RandomGridEnd()
    {
        for (int x = 0; x < side; x++)
            for (int y = 0; y < side; y++)
            {
                Cella[x, y].GetComponent<CellS>().InsertNum( randomGrid[x, y]);
            }

        //CAMBIARE sudoku risolto
    }

    bool isSafe(int[,] randomGrid, int row, int col, int num)
    {
        // if we find the same num in the similar row,
        // it returns false
        for (int x = 0; x <= 8; x++)
            if (randomGrid[row,x] == num)
                return false;

        // if we find the same num in the similar col,
        // it returns false
        for (int x = 0; x <= 8; x++)
            if (randomGrid[x,col] == num)
                return false;

        // Check if we find the same num in the particular 3*3 matrix,
        // it returns false
        int startRow = row - row % 3,
                startCol = col - col % 3;

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (randomGrid[i + startRow,j + startCol] == num)
                    return false;

        return true;
    }

    private bool SudokuAttender(int[,] randomGrid, int row, int col)//tries all the possibilities
    {
        // if we have reached the 8th row and 9th column (0 indexed matrix),
        // it is returning true to avoid further backtracking
        if (row == side - 1 && col == side)
            return true;

        // if column value becomes 9, we move to next row and column start from 0
        if (col == side)
        {
            row++;
            col = 0;
        }

        // if the current position of the grid already contains value >0,
        // it iterates for next column
        if (randomGrid[row,col] > 0)
            return SudokuAttender(randomGrid, row, col + 1);

        for (int num = 1; num <= side; num++)
        {

            // if it is safe to place the num (1-9)  in the
            // given row ,col  ->it moves to next column
            if (isSafe(randomGrid, row, col, num))
            {

                /* Assigning the num in the current (row,col) position of the grid
                   and assuming our assigned num in the position is correct     */
                randomGrid[row,col] = num;

                //  Checking for next possibility with next column
                if (SudokuAttender(randomGrid, row, col + 1))
                    return true;
            }

            // Removing the assigned num, since our assumption was wrong , and we go for 
            // next assumption with diff num value
            randomGrid[row,col] = 0;
        }
        return false;
    }

}
