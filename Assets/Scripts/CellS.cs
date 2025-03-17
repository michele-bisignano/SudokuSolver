using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CellS : MonoBehaviour // Manages the behavior of a single Sudoku cell
{
    public static int numCellChoises; // Counts how many cells already have a number
    public TMP_InputField obj_text; // UI input field for the cell's number
    public int number; // The definitive number assigned to this cell
    public int[] position = new int[2]; // The cell's position in the Sudoku grid

    public bool[] possibleCellNumber = new bool[9]; // Array to track which numbers are still possible
    public bool isNumDecided; // True if the number is already decided

    // Initialize the cell with default values
    void Start()
    {
        number = 0;
        isNumDecided = false;
        for (int i = 0; i < possibleCellNumber.Length; i++)
        {
            possibleCellNumber[i] = true; // All numbers are initially possible
        }
    }

    // Triggered when the input field value changes
    public void OnValueChanged()
    {
        try
        {
            InsertNum(int.Parse(obj_text.text));
        }
        catch
        {
            if (obj_text.text == "")
            {
                Start(); // Reset the cell if input is cleared
            }
            else
            {
                throw;
            }
        }
    }

    // Assigns a number to the cell and updates its state
    public void InsertNum(int num)
    {
        if (num == 0)
        {
            Start(); // Reset if number is zero
        }
        else
        {
            number = num;
            isNumDecided = true;
            obj_text.text = num.ToString(); // Update the input field

            // Mark all numbers except the chosen one as false
            for (int i = 0; i < possibleCellNumber.Length; i++)
            {
                possibleCellNumber[i] = (i == number - 1);
            }
        }
    }

    // Counts how many numbers are still possible for this cell
    public int CellCeck()
    {
        int count = 0;
        foreach (bool isPossible in possibleCellNumber)
        {
            if (isPossible)
            {
                count++;
            }
        }
        return count;
    }

    // Sets the cell's position in the Sudoku grid
    public void setPosition(int x, int y)
    {
        position[0] = x;
        position[1] = y;
    }

    // Counts the number of valid choices left for the cell
    public int TrueCounter()
    {
        int counter = isNumDecided ? 1 : CellCeck();
        return counter;
    }

    // Resets the count of cells with assigned numbers
    public void NumCellChoisesToZero()
    {
        numCellChoises = 0;
    }

    // Increments the count of cells with assigned numbers
    public void NumCellChoisesIncreaser()
    {
        numCellChoises++;
    }

    // Retrieves the count of cells with assigned numbers
    public int GetnumCellChoises()
    {
        return numCellChoises;
    }
}
