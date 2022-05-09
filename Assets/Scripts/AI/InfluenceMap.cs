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

    [Space]
    [SerializeField]
    private float timerUpdate = 1.0f;
    private float timerSave = 0.0f;

    [Header("Debug")]
    [SerializeField]
    private bool isDebug = false;
    [SerializeField]
    private float divTerrain = 30.0f;

    // Start is called before the first frame update
    void Start()
    {
        influenceTex = new Texture2D(sizeFinalTex, sizeFinalTex);

        terrain = GetComponent<Terrain>();
        sizeField.x = terrain.terrainData.size.x;
        sizeField.y = terrain.terrainData.size.z;
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

    private void OnDrawGizmos()
    {
        if (!isDebug)
            return;

        float height = 10.0f;

        for (float i = 0; i < sizeField.x; i += sizeField.x / divTerrain)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(new Vector3(i, height, 0), new Vector3(i, height, sizeField.x));

            for (float j = 0; j < sizeField.y; j += sizeField.y / divTerrain)
            {
                float ratioMapTexture = limitMap.width / sizeField.x;

                Color posPixel = limitMap.GetPixel((int)(i * ratioMapTexture), (int)(j * ratioMapTexture));

                float minColor = 0.1f;

                if (posPixel.r > minColor || posPixel.g > minColor || posPixel.b > minColor)
                    Gizmos.color = Color.yellow;
                else if (CheckBuilding(new Vector2(i, j), 5))
                    Gizmos.color = Color.green;
                else
                    Gizmos.color = Color.white;

                Gizmos.DrawSphere(new Vector3(i, height, j), 0.5f);
                
                if (i == 0)
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(new Vector3(0, height, j), new Vector3(sizeField.y, height, j));
                }
            }
        }   
    }

    bool CheckBuilding(Vector2 pos, float maxDistance)
    {

        TargetBuilding[] savebuilding = GameServices.GetTargetBuildings();

        for (int i = 0; i < savebuilding.Length; i++)
        {
            Vector2 flatPos = new Vector2(savebuilding[i].gameObject.transform.position.x, savebuilding[i].gameObject.transform.position.z);
            if ((flatPos - pos).magnitude < maxDistance)
                return true;
        }

        return false;
    }

    bool CheckUnit(Vector2 pos , ETeam team , float maxdDistance)
    {
        List<Unit> units = GameServices.GetControllerByTeam(team).GetAllUnits();

        for(int i = 0 ; i < units.Count ; i++)
        {
            Vector2 flatPos = new Vector2(units[i].gameObject.transform.position.x, units[i].gameObject.transform.position.z);
            if ((flatPos - pos).magnitude < maxdDistance)
                return true;
        }

        return false;
    }

    bool CheckFactory(Vector2 pos, ETeam team, float maxdDistance)
    {
        List<Factory> factorys = GameServices.GetControllerByTeam(team).GetAllFactorys();

        for (int i = 0; i < factorys.Count; i++)
        {
            Vector2 flatPos = new Vector2(factorys[i].gameObject.transform.position.x, factorys[i].gameObject.transform.position.z);
            if ((flatPos - pos).magnitude < maxdDistance)
                return true;
        }

        return false;
    }

    void GenerateInfluenceMap()
    {

        mat.mainTexture = influenceTex;

        for (int i = 0; i < influenceTex.width; i++)
        {
            for (int j = 0; j < influenceTex.height; j++)
            {
                float ratioMapTexture = limitMap.width / influenceTex.width;
                float ratioTexMap = influenceTex.width / sizeField.x;
                float minColor = 0.1f;

                Color posPixel = limitMap.GetPixel((int)(i * ratioMapTexture), (int)(j * ratioMapTexture));

                influenceTex.SetPixel(i, j, Color.white);

                // Area empty you can't do nothing on it 
                if (posPixel.r > minColor || posPixel.g > minColor || posPixel.b > minColor)
                    influenceTex.SetPixel(i,j,Color.black);

                float ratioI = i / ratioTexMap;
                float ratioJ = j / ratioTexMap;

                // Check if Building
                if (CheckBuilding(new Vector2(ratioI, ratioJ), 3))
                    influenceTex.SetPixel(i,j,Color.green);
               
                // Units on field 
                if (CheckUnit(new Vector2(ratioI, ratioJ),ETeam.Blue,2))
                    influenceTex.SetPixel(i, j, Color.blue);

                if (CheckUnit(new Vector2(ratioI, ratioJ), ETeam.Red, 2))
                    influenceTex.SetPixel(i, j, Color.red);

                // Factory Make
                if (CheckFactory(new Vector2(ratioI, ratioJ), ETeam.Blue, 5))
                    influenceTex.SetPixel(i, j, Color.blue);

                if (CheckFactory(new Vector2(ratioI, ratioJ), ETeam.Red, 5))
                    influenceTex.SetPixel(i, j, Color.red);

            }
        }

        influenceTex.Apply();
    }
}
