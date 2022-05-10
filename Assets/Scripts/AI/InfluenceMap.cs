using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMap : MonoBehaviour
{

    [Header("Final Texture")]
    [SerializeField]
    private Texture2D influenceTex;
    [SerializeField]
    private int sizeFinalTex = 64;

    [Header("Terrain Settings")]
    [SerializeField]
    private Texture2D limitMap;
    [SerializeField]
    private Material mat;

    private Vector2 sizeField;
    private Terrain terrain;

    [Header("Influence Settings")]
    [SerializeField]
    private float powerInfluence = 1.0f;
    [SerializeField]
    private float radiusInfluence = 5.0f;

    [Space]
    [SerializeField]
    private float timerUpdate = 1.0f;
    private float timerSave = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        influenceTex = new Texture2D(sizeFinalTex, sizeFinalTex);

        terrain = GetComponent<Terrain>();
        sizeField.x = terrain.terrainData.size.x;
        sizeField.y = terrain.terrainData.size.z;
        
        GenerateLimitInfluenceMap();
        GenerateInfluenceMap();
    }

    // Update is called once per frame
    void Update()
    {
        timerSave += Time.deltaTime;
        if(timerSave > timerUpdate)
        {
            GenerateInfluenceMap();
            timerSave = 0.0f;
        }
        
    }
    void GenerateLimitInfluenceMap()
    {
        mat.mainTexture = influenceTex;

        for (int i = 0; i < influenceTex.width; i++)
        {
            for (int j = 0; j < influenceTex.height; j++)
            {
                float ratioMapTexture = limitMap.width / influenceTex.width;
                float minColor = 0.1f;

                Color posPixel = limitMap.GetPixel((int)(i * ratioMapTexture), (int)(j * ratioMapTexture));

                influenceTex.SetPixel(i, j, Color.black);

                // Area empty you can't do nothing on it 
                if (posPixel.r > minColor)
                    influenceTex.SetPixel(i, j, Color.white);
            }
        }

        influenceTex.Apply();

    }

    void GenerateInfluenceMap()
    {
        mat.mainTexture = influenceTex;

        for (int i = 0; i < influenceTex.width; i++)
            for (int j = 0; j < influenceTex.height; j++)
            {
                if (influenceTex.GetPixel(i, j) == Color.white)
                    continue;

                influenceTex.SetPixel(i, j, Color.black);
            }

        float ratioTexMap = influenceTex.width / sizeField.x;

        CheckUnit(ETeam.Blue, ratioTexMap);
        CheckUnit(ETeam.Red, ratioTexMap);

        CheckFactory(ETeam.Blue, ratioTexMap);
        CheckFactory(ETeam.Red, ratioTexMap);

        CheckBuilding(ratioTexMap);

        influenceTex.Apply();
    }

    void CheckBuilding(float ratio)
    {
        TargetBuilding[] savebuilding = GameServices.GetTargetBuildings();

        for (int i = 0; i < savebuilding.Length; i++)
        {
            Vector2 flatPos = new Vector2(savebuilding[i].gameObject.transform.position.x * ratio, savebuilding[i].gameObject.transform.position.z * ratio);
            influenceTex.SetPixel((int)flatPos.x, (int)flatPos.y, Color.green);
        }
    }

    void CheckFactory(ETeam team, float ratio)
    {
        List<Factory> factorys = GameServices.GetControllerByTeam(team).GetAllFactorys();

        Color color = team == ETeam.Blue ? Color.blue : Color.red;

        for (int i = 0; i < factorys.Count; i++)
        {
            Vector2 flatPos = new Vector2(factorys[i].gameObject.transform.position.x * ratio, factorys[i].gameObject.transform.position.z * ratio);
            influenceTex.SetPixel((int)flatPos.x, (int)flatPos.y, color);
        }
    }

    void CheckUnit(ETeam team, float ratio)
    {
        List<Unit> units = GameServices.GetControllerByTeam(team).GetAllUnits();

        Color color = team == ETeam.Blue ? Color.blue : Color.red;

        for (int i = 0; i < units.Count; i++)
        {
            Vector2 flatPos = new Vector2(units[i].gameObject.transform.position.x * ratio, units[i].gameObject.transform.position.z * ratio);
            influenceTex.SetPixel((int)flatPos.x, (int)flatPos.y, color);

            SetPixelAroundPositionTexture((int)flatPos.x, (int)flatPos.y, radiusInfluence, color);
        }
    }

    void SetPixelAroundPositionTexture(int posX, int posY, float radius, Color color)
    {
        Vector2Int centerPixel = new Vector2Int(posX, posY);

        for (int i = posX - (int)radius; i < posX + (int)radius; i++)
        {
            for (int j = posY - (int)radius; j < posY + (int)radius; j++)
            {
                Color currentColorPixel = influenceTex.GetPixel(i, j);

                if (currentColorPixel == Color.white)
                    continue;

                Vector2Int currentPixel = new Vector2Int(i, j);
                float magnitudePixel = (currentPixel - centerPixel).magnitude;

                if (magnitudePixel < radius)
                {
                    float multiColor = -(magnitudePixel /radius) + 1.0f;
                    Color newColor = color * (multiColor * powerInfluence);

                    Color finalColor = (newColor + currentColorPixel) / 2.0f;
                    finalColor.a = 1.0f;

                    influenceTex.SetPixel(i, j, finalColor);
                }
            }
        }
    }
}

