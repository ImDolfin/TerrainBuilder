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

        map = new double[mapSideLength, mapSideLength];
        setMapCorners(mapSideLength, seedValue);

        for (int sideLength = mapSideLength - 1; sideLength >= 2; sideLength /= 2, offset /= 2.0)
        {
            int halfedSide = sideLength / 2;

            //generate the new square values
            for (int x = 0; x < mapSideLength - 1; x += sideLength)
            {
                for (int y = 0; y < mapSideLength - 1; y += sideLength)
                {
                    squareCalculations(x, y, halfedSide, offset, mapSideLength);
                }
            }

            //generate the diamond values
            for (int x = 0; x < mapSideLength - 1; x += halfedSide)
            {
                for (int y = (x + halfedSide) % sideLength; y < mapSideLength - 1; y += sideLength)
                {
                    diamondCalculations(x, y, halfedSide, offset, mapSideLength);
                }
            }
        }

        return map;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="halfedSide"></param>
    /// <param name="offset"></param>
    private void diamondCalculations(int x, int y, int halfedSide, double offset, int mapSideLength)
    {
        double average =
                        map[(x - halfedSide + mapSideLength - 1) % (mapSideLength - 1), y] +
                        map[(x + halfedSide) % (mapSideLength - 1), y] +
                        map[x, (y + halfedSide) % (mapSideLength - 1)] +
                        map[x, (y - halfedSide + mapSideLength - 1) % (mapSideLength - 1)];
        average /= 4.0;

        //center is average plus random offset
        average = average + (random.NextDouble() * 2 * offset) - offset;

        //update value at the center of the diamond
        map[x, y] = average;

        if (x == 0) map[mapSideLength - 1, y] = average;
        if (y == 0) map[x, mapSideLength - 1] = average;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="halfedSide"></param>
    /// <param name="offset"></param>
    private void squareCalculations(int x, int y, int halfedSide, double offset, int mapSideLength)
    {
        double average =
                    map[(x - halfedSide + mapSideLength - 1) % (mapSideLength - 1), y] +
                    map[(x + halfedSide) % (mapSideLength - 1), y] +
                    map[x, (y + halfedSide) % (mapSideLength - 1)] +
                    map[x, (y - halfedSide + mapSideLength - 1) % (mapSideLength - 1)];

        //center is average plus random offset
        map[x + halfedSide, y + halfedSide] = average + (random.NextDouble() * 2 * offset) - offset;
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
}