using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHeightMap : MonoBehaviour
{
    public Renderer renderer;

    private Color[] colorHeightMap;

    public void createNewColorHeightMap(double[,] heightArray)
    {
        int sideLength = heightArray.GetLength(0);

        Texture2D texture = new Texture2D(sideLength, sideLength);

        retrieveColorHeightMap(heightArray);

        texture.SetPixels(colorHeightMap);
        texture.Apply();

        renderColorHeightMap(texture, sideLength);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="heightArray"></param>
    private void retrieveColorHeightMap(double[,] heightArray)
    {
        int sideLength = heightArray.GetLength(0);

        colorHeightMap = new Color[sideLength * sideLength];

        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                colorHeightMap[i * sideLength + j] = Color.Lerp(Color.black, Color.white, (float)heightArray[i, j]);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="sideLength"></param>
    public void renderColorHeightMap(Texture2D texture, int sideLength)
    {
        renderer.sharedMaterial.mainTexture = texture;
        renderer.transform.localScale = new Vector3(sideLength, 1, sideLength);
    }
}
