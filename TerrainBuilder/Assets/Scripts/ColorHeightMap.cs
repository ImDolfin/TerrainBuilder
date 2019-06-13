using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorHeightMap : MonoBehaviour
{

    /// <summary>
    /// The Idea was to simply map the heights array onto a colormap and apply it to the Mesh
    /// Using that method, one can easily do changes to the terrain without using a meshcollider.
    /// avoiding a raycast and update of the meshcollider is much more efficient and precise than 
    /// the use of the collider and distances. Also, using this way, looping through all the vertices is avoided,
    /// which reduces the cpu overhead.
    /// </summary>
    public Renderer renderer;

    private Color[] colorHeightMap;

    /// <summary>
    /// Create a new colormap
    /// </summary>
    /// <param name="heightArray"></param>
    public void createNewColorHeightMap(float[,] heightArray)
    {
        int sideLength = heightArray.GetLength(0);

        Texture2D texture = new Texture2D(sideLength, sideLength);

        retrieveColorHeightMap(heightArray);

        texture.SetPixels(colorHeightMap);
        texture.Apply();

        renderColorHeightMap(texture, sideLength);
    }

    /// <summary>
    /// return the color map based on the array of heights
    /// </summary>
    /// <param name="heightArray"></param>
    private void retrieveColorHeightMap(float[,] heightArray)
    {
        int sideLength = heightArray.GetLength(0);

        colorHeightMap = new Color[sideLength * sideLength];

        for (int i = 0; i < sideLength; i++)
        {
            for (int j = 0; j < sideLength; j++)
            {
                colorHeightMap[i * sideLength + j] = Color.Lerp(Color.black, Color.white, heightArray[i, j]);
            }
        }
    }

    /// <summary>
    /// render the color map
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="sideLength"></param>
    public void renderColorHeightMap(Texture2D texture, int sideLength)
    {
        renderer.sharedMaterial.mainTexture = texture;
        renderer.transform.localScale = new Vector3(sideLength, 1, sideLength);
    }
}
