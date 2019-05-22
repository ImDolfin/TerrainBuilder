using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquareAlgorithm
{

    //The map for the DiamondSquare Algorithm
    private double[,] map;

    //random number generator
    private System.Random random = new System.Random();

    //Average Offeset
    private double offset = 500.0;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapSideLength">has to be a value of 2^n + 1</param>
    /// <param name="seedValue"></param>
    /// <returns></returns>
    public double[,] generateMapArray(int mapSideLength, double seedValue)
    {
        if (!isPowerOfTwo((uint)(mapSideLength - 1)))
        {
            throw new ArgumentException("illegal argument, the provided side length is not 2^n + 1 long");
        }

        map = new double[mapSideLength, mapSideLength];
        setMapCorners(mapSideLength, seedValue);

        for (int sideLength = mapSideLength - 1; sideLength >= 2; sideLength /= 2, offset /= 2.0)
        {
            int halfedSide = sideLength / 2;

            //generate the new square values
            for (int i = 0; i < mapSideLength - 1; i += sideLength)
            {
                for (int j = 0; j < mapSideLength - 1; j += sideLength)
                {
                    squareCalculations(i, j, halfedSide, offset, mapSideLength);
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

        return map;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="halfedSide"></param>
    /// <param name="offset"></param>
    private void diamondCalculations(int i, int j, int halfedSide, double offset, int mapSideLength)
    {
        double average =
                        map[(i - halfedSide + mapSideLength - 1) % (mapSideLength - 1), j] +
                        map[(i + halfedSide) % (mapSideLength - 1), j] +
                        map[i, (j + halfedSide) % (mapSideLength - 1)] +
                        map[i, (j - halfedSide + mapSideLength - 1) % (mapSideLength - 1)];
        average /= 4.0;

        //center is average plus random offset
        average = average + (random.NextDouble() * 2 * offset) - offset;

        //update value at the center of the diamond
        map[i, j] = average;

        if (i == 0) map[mapSideLength - 1, j] = average;
        if (j == 0) map[i, mapSideLength - 1] = average;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="halfedSide"></param>
    /// <param name="offset"></param>
    private void squareCalculations(int i, int j, int halfedSide, double offset, int mapSideLength)
    {
        double average =
                    map[(i - halfedSide + mapSideLength - 1) % (mapSideLength - 1), j] +
                    map[(i + halfedSide) % (mapSideLength - 1), j] +
                    map[i, (j + halfedSide) % (mapSideLength - 1)] +
                    map[i, (j - halfedSide + mapSideLength - 1) % (mapSideLength - 1)];

        //center is average plus random offset
        map[i + halfedSide, j + halfedSide] = average + (random.NextDouble() * 2 * offset) - offset;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapWidth"></param>
    /// <param name="mapLength"></param>
    /// <param name="maxHeight"></param>
    private void setMapCorners(int mapSideLength, double seedValue)
    {
        double randomGeneratedNumber = random.NextDouble() * (seedValue - 0) + 0;
        map[0, mapSideLength - 1] = randomGeneratedNumber;
        map[0, 0] = randomGeneratedNumber;
        map[mapSideLength - 1, 0] = randomGeneratedNumber;
        map[mapSideLength - 1, mapSideLength - 1] = randomGeneratedNumber;
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