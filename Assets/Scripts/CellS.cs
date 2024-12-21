using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CellS : MonoBehaviour//codice di ogni singola cella
{
    public static int numCellChoises;//conta quante celle hanno già un numero
    public TMP_InputField obj_text;
    public int number;//the DEFINITIVE number
    public int[] position = new int[2];//the cell's position in the sudoku
    //public GameObject grid_square;

    public bool[] possibleCellNumber= new bool[9];//true if the num can be the right one
    public bool isNumDecided;//true if the num is already decided

    // Start is called before the first frame update
    void Start()
    {
        number = 12;
        isNumDecided =false;
        for (int i=0; i<possibleCellNumber.Length; i++)
        {
            possibleCellNumber[i] = true;
        }
    }


    public void OnValueChanged()
    {        
        try { InsertNum(int.Parse(obj_text.text)); }
        catch {
            if (obj_text.text == "")
            {
                Start();
            }
            else { throw; }
        }//eccezione
    }

    public void InsertNum(int num)//istanzia numero e rende false le altre celle RICORDA di chiamarlo insieme al FalseMaker
    {
        if (num == 0)
        {
            Start();
        }
        else
        {
            number=num;
            isNumDecided= true;
            if(num!=0)
            {
                obj_text.text = num.ToString();
            }
            for (int i = 0; i < possibleCellNumber.Length; i++)
            {
                if (i != number)
                    possibleCellNumber[i] = false;
                else
                    possibleCellNumber[i] = true;
            }
        }
    }

    public int CellCeck()//count how many numbers the cell can store
    {
        int temp = 0;
        for (int i=0;i<possibleCellNumber.Length; i++)
        {
            if (possibleCellNumber[i])
            {
                temp++;
            }
        }
        return temp;
    }

    public void setPosition(int x, int y)
    {
        position[0]=x; position[1]=y;
    }

    public int TrueCounter()//count the true
    {
        int counter = 0;
        if (isNumDecided) counter++;
        else
        {
            for(int i=0;i< possibleCellNumber.Length; i++)
            {
                if (possibleCellNumber[i])
                {
                    counter++;
                }
            }
        }
        return counter;
    }


    public void NumCellChoisesToZero()
    {
        numCellChoises=0;
    }

    public void NumCellChoisesIncreaser()
    {
        numCellChoises++;
    }

    public int GetnumCellChoises()
    {
        return numCellChoises;
    }
    /*public void trueCheck()
    {


        for (int i=0;i<9; i++)CANCELLARE
        {
            if (possibleCellNumber[i])
            {

                Debug.Log("\nCella: " + position[0] + "  " + position[1] + "\n"+i +" true\t");
            }
            else
            {
                Debug.Log("\nCella: " + position[0] + "  " + position[1] + "\n"+i + " false\t");

            }
        }
    }
    */

}