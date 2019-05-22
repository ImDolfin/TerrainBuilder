using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquareAlgorithm
{

    //The map for the DiamondSquare Algorithm
    private float[,] map;

    //random number generator
    private System.Random random = new System.Random();

    //Average Offset
    private float offset = 100.0f;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapSideLength">has to be a value of 2^n + 1</param>
    /// <param name="seedValue"></param>
    /// <returns></returns>
    public float[,] generateMapArray(int mapSideLength, float seedValue, float offset)
    {
        this.offset = offset;

        if (!isPowerOfTwo((uint)(mapSideLength - 1)))
        {
            throw new ArgumentException("illegal argument, the provided side length is not 2^n + 1 long");
        }

        map = new float[mapSideLength, mapSideLength];
        setMapCorners(mapSideLength, seedValue);

        for (int sideLength = mapSideLength - 1; sideLength >= 2; sideLength /= 2, offset /= 2.0f)
        {
            int halfedSide = sideLength / 2;

            //generate the new square values
            for (int i = 0; i < mapSideLength - 1; i += sideLength)
            {
                for (int j = 0; j < mapSideLength - 1; j += sideLength)
                {
                    squareCalculations(i, j, halfedSide, offset, sideLength);
                }
            }

            //generate the diamond values
            for (int i = 0; i < mapSideLength - 1; i += halfedSide)
            {
                for (int j = (i + halfedSide) % sideLength; j < mapSideLength - 1; j += sideLength)
                {
                    diamondCalculations(i, j, halfedSide, offset, mapSideLength);
                }
            }
        }
        setNegativeToZero(mapSideLength);
        return map;
    }

    private void setNegativeToZero(int mapSideLength)
    {
        for(int i = 0; i < mapSideLength - 1; i++)
            {
            for (int j = 0; j < mapSideLength - 1; j++)
            {
                if (map[i, j] < 0f)
                    map[i, j] = 0;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="halfedSide"></param>
    /// <param name="offset"></param>
    private void squareCalculations(int i, int j, int halfedSide, float offset, int sideLength)
    {
        float average =
                    map[i, j] + //top left
                    map[i + sideLength, j] +//top right
                    map[i, j + sideLength] + //lower left
                    map[i + sideLength, j + sideLength];
        average /= 4.0f;

        //center is average plus random offset
        map[i + halfedSide, j + halfedSide] = 
            average + ((float)random.NextDouble() * 2 * offset) - offset;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="halfedSide"></param>
    /// <param name="offset"></param>
    private void diamondCalculations(int i, int j, int halfedSide, float offset, int mapSideLength)
    {
        float average =
                        map[(i - halfedSide + mapSideLength - 1) % (mapSideLength - 1), j] +
                        map[(i + halfedSide) % (mapSideLength - 1), j] +
                        map[i, (j + halfedSide) % (mapSideLength - 1)] +
                        map[i, (j - halfedSide + mapSideLength - 1) % (mapSideLength - 1)];
        average /= 4.0f;

        //center is average plus random offset
        average = average + ((float)random.NextDouble() * 2 * offset) - offset;

        //update value at the center of the diamond
        map[i, j] = average;

        if (i == 0) map[mapSideLength - 1, j] = average;
        if (j == 0) map[i, mapSideLength - 1] = average;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapWidth"></param>
    /// <param name="mapLength"></param>
    /// <param name="maxHeight"></param>
    private void setMapCorners(int mapSideLength, float seedValue)
    {
        map[0, mapSideLength - 1] = seedValue;
        map[0, 0] = seedValue;
        map[mapSideLength - 1, 0] = seedValue;
        map[mapSideLength - 1, mapSideLength - 1] = seedValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    private bool isPowerOfTwo(uint number)
    {
        return (number != 0) && ((number & (number - 1)) == 0);
    }
}