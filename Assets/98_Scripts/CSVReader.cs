using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Text.RegularExpressions;

public class CSVReader : MonoBehaviour
{
    [SerializeField] TextAsset cSV;

    [SerializeField]
    private List<int> oBJECTID;

    private List<string> range_Id;
    private List<string> local_pop;
    private List<string> lABEL;
    private List<string> lABEL2;
    private List<string> pROV_TERR;
    private List<string> risk_Assessment;
    private List<string> range_Type;
    private List<string> population;
    private List<string> pop_Trend;
    private List<float> dH_Fire;
    private List<float> dH_Anthro;
    private List<int> dH_Total;
    private List<float> shape_STArea__;
    private List<float> shape_STLength__;
    private List<float> shape_Length;
    private List<float> shape_Area;

    [SerializeField]
    private int objectId;
    [ShowNonSerializedField]
    private string rangeId;
    [ShowNonSerializedField]
    private string localPop;
    [ShowNonSerializedField]
    private string label;
    [ShowNonSerializedField]
    private string label2;
    [ShowNonSerializedField]
    private string provTerr;
    [ShowNonSerializedField]
    private string riskAssessment;
    [ShowNonSerializedField]
    private string rangeType;
    [ShowNonSerializedField]
    private string population1;
    [ShowNonSerializedField]
    private string popTrend;
    [ShowNonSerializedField]
    private float dHFire;
    [ShowNonSerializedField]
    private float dHAnthro;
    [ShowNonSerializedField]
    private int dHTotal;
    [ShowNonSerializedField]
    private float shapeSTArea;
    [ShowNonSerializedField]
    private float shapeSTLength;
    [ShowNonSerializedField]
    private float shapeLength;
    [ShowNonSerializedField]
    private float shapeArea;


    [Button]
    private void StartReading()
    {
        ListInit();
        Reader();
    }

    [Button]
    private void ShowObjectID()
    {
        if (objectId > 0 && objectId <= oBJECTID.Count)
        {
            TransferValues();
        }
    }

    private void ListInit()
    {
        oBJECTID = new List<int>();
        range_Id = new List<string>();
        local_pop = new List<string>();
        lABEL = new List<string>();
        lABEL2 = new List<string>();
        pROV_TERR = new List<string>();
        risk_Assessment = new List<string>();
        range_Type = new List<string>();
        population = new List<string>();
        pop_Trend = new List<string>();
        dH_Fire = new List<float>();
        dH_Anthro = new List<float>();
        dH_Total = new List<int>();
        shape_STArea__ = new List<float>();
        shape_STLength__ = new List<float>();
        shape_Length = new List<float>();
        shape_Area = new List<float>();
    }

    private void Reader()
    {
        //Save all the lines in a array
        string[] lines = cSV.text.Split("\n"[0]);

        for (int i = 1; i < lines.Length; i++)
        {
            Regex newPart = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            string[] parts = newPart.Split(lines[i]);


            for (int j = 0; j < parts.Length; j++)
            {
                parts[j] = parts[j].Replace("\"", "");
                //Debug.LogError(parts[j]);

            }

            //Debug.LogWarning("objectID = " + parts[0]);
            oBJECTID.Add(int.Parse(parts[0]));
            //Debug.LogWarning("range_Id = " + parts[1]);
            range_Id.Add(parts[1]);
            //Debug.LogWarning("local_pop = " + parts[2]);
            local_pop.Add(parts[2]);
            //Debug.LogWarning("lABEL = " + parts[3]);
            lABEL.Add(parts[3]);
            //Debug.LogWarning("lABEL2 = " + parts[4]);
            lABEL2.Add(parts[4]);
            //Debug.LogWarning("pROV_TERR = " + parts[5]);
            pROV_TERR.Add(parts[5]);
            //Debug.LogWarning("risk_Assessment = " + parts[6]);
            risk_Assessment.Add(parts[6]);
            //Debug.LogWarning("range_Type = " + parts[7]);
            range_Type.Add(parts[7]);
            //Debug.LogWarning(" population = " + parts[8]);
            population.Add(parts[8]);
            //Debug.LogWarning("pop_Trend = " + parts[9]);
            pop_Trend.Add(parts[9]);
            //Debug.LogWarning("dH_Fire = " + parts[10]);
            dH_Fire.Add(float.Parse(parts[10]));
            //Debug.LogWarning("dH_Anthro = " + parts[11]);
            dH_Anthro.Add(float.Parse(parts[11]));
            //Debug.LogWarning("dH_Total = " + parts[12]);
            dH_Total.Add(int.Parse(parts[12]));
            //Debug.LogWarning("shape_STArea__ = " + parts[13]);
            shape_STArea__.Add(float.Parse(parts[13]));
            //Debug.LogWarning("shape_STLength__ = " + parts[14]);
            shape_STLength__.Add(float.Parse(parts[14]));
            //Debug.LogWarning("shape_Length = " + parts[15]);
            shape_Length.Add(float.Parse(parts[15]));
            //Debug.LogWarning("shape_Area = " + parts[16]);
            shape_Area.Add(float.Parse(parts[16]));
        }
    }

    private void TransferValues()
    {
        int index = objectId;
        index -= 1;

        rangeId = range_Id[index];
        localPop = local_pop[index];
        label = lABEL[index];
        label2 = lABEL2[index];
        provTerr = pROV_TERR[index];
        riskAssessment = risk_Assessment[index];
        rangeType = range_Type[index];
        population1 = population[index];
        popTrend = pop_Trend[index];
        dHFire = dH_Fire[index];
        dHAnthro = dH_Anthro[index];
        dHTotal = dH_Total[index];
        shapeSTArea = shape_STArea__[index];
        shapeSTLength = shape_STLength__[index];
        shapeLength = shape_Length[index];
        shapeArea = shape_Area[index];        
    }
}