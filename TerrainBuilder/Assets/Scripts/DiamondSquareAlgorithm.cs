using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class containing methods that perform the diamond square algorithm
/// </summary>
public class DiamondSquareAlgorithm
{

    //The map for the DiamondSquare Algorithm
    private float[,] map;

    //random number generator
    private System.Random random = new System.Random();

    //Average Offset which sets the hight to be in a specific Interval
    //The offset will also be halfed at each step like the side length in order to smoothen the surface 
    private float offset = 100.0f;

    /// <summary>
    /// generates a 2D array of heights which are calculated based on the diamond square algorithm
    /// </summary>
    /// <param name="mapSideLength">length of the map which has to be a value of 2^n + 1</param>
    /// <param name="seedValue">seed value which is used for the initialization</param>
    /// <returns>2D float array of the the generated heights</returns>
    public float[,] generateMapArray(int mapSideLength, float seedValue, float offset)
    {
        this.offset = offset;

        //check if the provided number of the maps dimension is a power of two + 1
        if (!isPowerOfTwo((uint)(mapSideLength - 1)))
        {
            throw new ArgumentException("illegal argument, the provided side length is not 2^n + 1 long");
        }

        //initialize the map 
        map = new float[mapSideLength, mapSideLength];
        setMapCorners(mapSideLength, seedValue);

        //perform the recursive approach in a for loop because recursive will cause stack overflow for bigger maps
        for (int sideLength = mapSideLength - 1; sideLength >= 2; sideLength /= 2, offset /= 2.0f)
        {
            int halfedSide = sideLength / 2;

            //perform the Diamond Step which takes the average of all the square corners
            //generate the new square values
            for (int i = 0; i < mapSideLength - 1; i += sideLength)
            {
                for (int j = 0; j < mapSideLength - 1; j += sideLength)
                {
                    squareCalculations(i, j, halfedSide, offset, sideLength);
                }
            }

            //perform the Square Step which takes the average of all the diamond corners
            //generate the diamond values
            for (int i = 0; i < mapSideLength - 1; i += halfedSide)
            {
                for (int j = (i + halfedSide) % sideLength; j < mapSideLength - 1; j += sideLength)
                {
                    diamondCalculations(i, j, halfedSide, offset, mapSideLength);
                }
            }
        }
        //set all negative values in the array to a 0
        setNegativeToZero(mapSideLength);
        return map;
    }

    /// <summary>
    /// performs the average calculations for the square
    /// </summary>
    /// <param name="i">i coordinate of the top left corner</param>
    /// <param name="j">j coordinate of the top left corner</param>
    /// <param name="halfedSide"> the halfed length of the sidelength</param>
    /// <param name="offset">offset value</param>
    private void squareCalculations(int i, int j, int halfedSide, float offset, int sideLength)
    {
        //simple average of all the squares corners
        float average =
                    map[i + sideLength, j] +//top right
                    map[i, j] + //top left
                    map[i, j + sideLength] + //lower left
                    map[i + sideLength, j + sideLength];//lower right
        average /= 4.0f;

        //center of the square is the average plus a random value in the range of the offset
        map[i + halfedSide, j + halfedSide] = 
            average + ((float)random.NextDouble() * 2 * offset) - offset;
    }

    /// <summary>
    /// Applies the diamond calculation to the i/j coordinates in the map
    /// </summary>
    /// <param name="i">i coordinate</param>
    /// <param name="j">j coordinate</param>
    /// <param name="halfedSide">the halfed side of the side lengths</param>
    /// <param name="offset"> ffset value</param>
    private void diamondCalculations(int i, int j, int halfedSide, float offset, int mapSideLength)
    {

        //calculations for the diamond which calculate the corners of the diamond and average their value
        float average =
                        map[i, (j - halfedSide + mapSideLength - 1) % (mapSideLength - 1)] + //top
                        map[(i - halfedSide + mapSideLength - 1) % (mapSideLength - 1), j] + //left
                        map[i, (j + halfedSide) % (mapSideLength - 1)] + // bottom
                        map[(i + halfedSide) % (mapSideLength - 1), j]; // right
                        
        average /= 4.0f;

        //center of the diamond is the average plus a random value in the range of the offset
        average = average + ((float)random.NextDouble() * 2 * offset) - offset;

        //assign value at the center of the diamond
        map[i, j] = average;

        //Wrap edges of the map
        //Points that are located on the edges of the array will have only three points to use
        //Therefor, the diamond will have only 3 values set.
        //This is being worked around by applying the average to the other side of the map. so basically "wrap around to the other map side"
        if (i == 0) map[mapSideLength - 1, j] = average;
        if (j == 0) map[i, mapSideLength - 1] = average;
    }

    /// <summary>
    /// sets the initial corners to a randm value based on the seed
    /// </summary>
    /// <param name="mapSideLength"> length of the map side</param>
    /// <param name="seedValue"> seed of the random number</param>
    private void setMapCorners(int mapSideLength, float seedValue)
    {
        //use a random value of the seed to initialize the corners
        float randValue = (float)random.Next((int)seedValue);
        map[0, mapSideLength - 1] = randValue;
        map[0, 0] = randValue;
        map[mapSideLength - 1, 0] = randValue;
        map[mapSideLength - 1, mapSideLength - 1] = randValue;
    }

    /// <summary>
    /// Checks if a number is a power of two
    /// </summary>
    /// <param name="number">uint of the number which has to be checked</param>
    /// <returns>bool if number is power of two then TRUE, else FALSE</returns>
    public bool isPowerOfTwo(uint number)
    {
        //tricky trick which tests for the number
        //  1000 - 1
        // =0111 apply logic AND &
        // =0000
        //logic AND(&) will then result in 0
        //technically this will result in 0 being a power of 2 too which is wrong,
        //Therefore, if the number is not 0 and a power of 2 then this will result in TRUE
        return (number != 0) && ((number & (number - 1)) == 0);
    }

    /// <summary>
    /// Sets all negative numbers in the class parameter array to 0
    /// </summary>
    /// <param name="mapSideLength"> length of the arrays sides</param>
    private void setNegativeToZero(int mapSideLength)
    {
        for (int i = 0; i < mapSideLength - 1; i++)
        {
            for (int j = 0; j < mapSideLength - 1; j++)
            {
                if (map[i, j] < 0f)
                    map[i, j] = 0;
            }
        }
    }
}