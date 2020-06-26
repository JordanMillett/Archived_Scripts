using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]
public class TexturePaintTarget : MonoBehaviour
{
    public enum Shapes  //auto load names of shapes
	{
		Circle,
        Square
	};

    public List<BrushShape> shapeData;

    public float brushSize;
    public float brushStrength;
    public Color32 brushColor;
    public Shapes currentBrushShape;

    public Vector2Int textureSize;
    public Color32 backgroundColor;

    public Vector3 mousePosition = new Vector3(0f, 0f, 0f);
    public bool isHovered = false;
    public bool editing = false;
    public List<Texture2D> Layers;
    Renderer _renderer;

    //Layers, don't recalculate mip maps

    [ExecuteInEditMode]
    public void PaintPixels(Vector3 Position, int Layer)
    {

        Vector2Int pixelIndex;
        
        Vector2 localPosition = 
        new Vector2
        (
            Position.x - this.transform.position.x,
            Position.z - this.transform.position.z
        );

        Vector2 NormalizedPosition = localPosition/this.transform.localScale.x; //make it work for different xy

        int xPixelIndex = (int)Mathf.Floor(Layers[Layer].width * NormalizedPosition.x);
        int yPixelIndex = (int)Mathf.Floor(Layers[Layer].height * NormalizedPosition.y);

        xPixelIndex -= Layers[Layer].width/-2;
        yPixelIndex -= Layers[Layer].height/-2;

        if(xPixelIndex > Layers[Layer].width)
            xPixelIndex = Layers[Layer].width;
        if(xPixelIndex < 0)
            xPixelIndex = 0;

        if(yPixelIndex > Layers[Layer].height)
            yPixelIndex = Layers[Layer].height;
        if(yPixelIndex < 0)
            yPixelIndex = 0;
        
        pixelIndex = new Vector2Int(xPixelIndex, yPixelIndex);

        //use alpha of brush to apply lighter colors
        //this is an override function, setpixels coveres all pixels
        //paint different types of brushes
        //link together multiple quads?
        //Save Textures
        //User friendly UI
        //erase tool
        //blur texture, mipmap levels
        //lerp between positions to do long streaks when it skips frames
        //account for different rotations and scales
        //support full pbr materials, drop in public materials as colors
        //save textures so they don't disappear when scene is saved

        /*
        Color32[] pixelColors = Layers[Layer].GetPixels32(0);   

        for (int i = 0; i < pixelColors.Length; i++)
        {
            if(PixelAffected(PixelXY(i, Layers[Layer].width), pixelIndex))
                pixelColors[i] = brushColor;
        }
        Layers[Layer].SetPixels32(pixelColors, 0);
        */
        /*
        for(int x = 0; x < Layers[Layer].width; x++)
        {
            for(int y = 0; y < Layers[Layer].height; y++)
            {

                //if(PixelAffected(new Vector2Int(x,y), pixelIndex))
                if(new Vector2Int(x,y) == pixelIndex)
                    Layers[Layer].SetPixel(x, y, brushColor);

            }
        }*/


        Paint(pixelIndex, brushSize, Layer);

        //Debug.Log(pixelIndex);

        //Layers[Layer].SetPixel(pixelIndex.x, pixelIndex.y, brushColor);  single pixel brush 

        //set each color to a boolean by passing their position
        //first check if close enough to use the algorithm
        //use the algorithm

        
        //Layers[Layer].SetPixel(pixelIndex.x + 1, pixelIndex.y, brushColor);
        //Layers[Layer].SetPixel(pixelIndex.x + 1, pixelIndex.y + 1, brushColor); 
        //Layers[Layer].SetPixel(pixelIndex.x, pixelIndex.y + 1, brushColor); 
        //Layers[Layer].SetPixel(pixelIndex.x - 1, pixelIndex.y + 1, brushColor); 
        //Layers[Layer].SetPixel(pixelIndex.x - 1, pixelIndex.y, brushColor); 
        //Layers[Layer].SetPixel(pixelIndex.x - 1, pixelIndex.y - 1, brushColor); 
        //Layers[Layer].SetPixel(pixelIndex.x , pixelIndex.y - 1, brushColor);
        //Layers[Layer].SetPixel(pixelIndex.x + 1 , pixelIndex.y - 1, brushColor);  
        


        Layers[Layer].Apply(false);
        _renderer = GetComponent<Renderer>();
        _renderer.sharedMaterial.SetTexture("_BaseMap", Layers[Layer]);

    }

    void Paint(Vector2Int Origin, float Size, int Layer)
    {

        //Layers[Layer].SetPixel(Origin.x, Origin.y, brushColor);

        int radius = (int) Mathf.Ceil(Size/2f);     //map more accuratly
        //int radius = (int) Mathf.Round(Size/2f);

        int scaleFactor = Layers[Layer].width/128;
        radius *= scaleFactor;

        /*
        if(radius == 0)
            Layers[Layer].SetPixel(Origin.x, Origin.y, brushColor);
        else
        */

            for(int x = -radius; x < radius; x++)
            {
                for(int y = -radius; y < radius; y++)
                {
                    if(!OffGrid(new Vector2Int(Origin.x + x, Origin.y + y), Layers[Layer].width))
                        Layers[Layer].SetPixel(Origin.x + x, Origin.y + y, brushColor);

                }
            }
        

        //Debug.Log(radius);

    }

    bool OffGrid(Vector2Int Position, int Size)
    {

        if(Position.x > Size || Position.x < 0)
            return true;

        if(Position.y > Size || Position.y < 0)
            return true;

        return false;

    }

    /*Vector2Int PixelXY(int Index, int Size)
    {
        int value = Index;

        int XLoc = 0;
        int YLoc = 0;

        while(value > Size)
        {
            value -= Size;
            YLoc++;
        }

        XLoc = value;

        //Debug.Log(new Vector2Int(XLoc, YLoc));

        return new Vector2Int(XLoc, YLoc);

    }*/
    /*
    bool PixelAffected(Vector2Int Position, Vector2Int MouseOrigin)
    {

        if(Position == MouseOrigin)
            return true;
        else
            return false;

    }*/

    [ExecuteInEditMode]
    public void NewTexture()
    {

        Texture2D newTexture = new Texture2D(textureSize.x, textureSize.y, TextureFormat.RGBA32, false);

        Color32 bgColor = backgroundColor;
        Color32[] currentPixels = newTexture.GetPixels32(0);

        for(int i = 0; i < currentPixels.Length; i++)
        {
            currentPixels[i] = bgColor;
        }

        newTexture.SetPixels32(currentPixels, 0);

        newTexture.Apply(false);

        _renderer = GetComponent<Renderer>();
        _renderer.sharedMaterial.SetTexture("_BaseMap", newTexture);

        //Layers.Add(newTexture);
        Layers[0] = newTexture;
        //SetToNoise(Layers[0]);
    }

    [ExecuteInEditMode]
    public void UpdateMousePosition(SceneView SV)
    {
        RaycastHit mouseHit;
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if(Physics.Raycast(mouseRay, out mouseHit))
        {
            if(mouseHit.transform == this.transform)
            {

                mousePosition = mouseHit.point;
                isHovered = true;

                

                //DrawCursor(shapeData[(int) currentBrushShape]);

            }else
            {

                isHovered = false;

            }

        }else
        {
            isHovered = false;
        }
    }

    void DrawCursor(BrushShape S)
    {

        Gizmos.color = Color.green;

        List<Vector3> Positions = new List<Vector3>();

        foreach(Vector3 V in S.vertices)
        {

            Positions.Add((new Vector3(V.x, 0f, V.y) * brushSize) + mousePosition);

        }

        for(int i = 0; i < Positions.Count; i++)
        {
            Positions[i] += new Vector3(0f, .1f, 0f);
        }

        for(int i = 0; i < Positions.Count - 1; i++)
        {
            
            Gizmos.DrawLine(Positions[i], Positions[i + 1]);
        }

        Gizmos.DrawLine(Positions[Positions.Count - 1], Positions[0]);

        /*foreach(Vector3 V in Positions)
        {

            Gizmos.DrawCube(V, new Vector3(.25f,.25f,.25f));

        }*/
    

        //Gizmos.DrawLine(mousePosition + (Vector3) S.vertices[0], mousePosition + (Vector3) S.vertices[1]);
        //Gizmos.DrawWireSphere(this.transform.position, 5f); 
        
    }

    void OnDrawGizmosSelected()
    {
 
        #if UNITY_EDITOR

            if(isHovered && editing)
            {
                //Gizmos.color = brushColor;
                //Gizmos.DrawWireSphere(mousePosition, brushSize);

                //Debug.Log((int)currentBrushShape);

                //E e = E.C;
                //int index = Array.IndexOf(Enum.GetValues(e.GetType()), e);

                //int index = (int) Shapes.Circle;
                int index = (int)currentBrushShape;

                //Debug.Log(index);

                DrawCursor(shapeData[index]);
                /*
                    ((int)i).ToString();

                */

            }else
            {
                //Gizmos.color = Color.green;
                //Gizmos.DrawWireSphere(this.transform.position, 10f); 
            }

        #endif
    }

    void SetToNoise(Texture2D Layer)
    {

        Color32[] currentPixels = Layer.GetPixels32(0);

        for(int i = 0; i < currentPixels.Length; i++)
        {
            currentPixels[i] = new Color32((byte)Random.Range(0, 255), (byte) Random.Range(0, 255), (byte) Random.Range(0, 255), (byte) 255);
        }

        Layer.SetPixels32(currentPixels, 0);

        Layer.Apply(false);

        _renderer = GetComponent<Renderer>();
        _renderer.sharedMaterial.SetTexture("_BaseMap", Layer);

    }
}
