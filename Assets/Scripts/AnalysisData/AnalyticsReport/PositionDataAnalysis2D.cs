using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionDataAnalysis2D : MonoBehaviour
{
  public DataReader dataReader;
  public BodyAngleParametersDataAnalysis bodyAngleParametersDataAnalysis;

  void Start()
  {
    if (dataReader == null)
    {
      Debug.LogError("DataReader is not assigned!");
      return;
    }

    if (bodyAngleParametersDataAnalysis == null)
    {
      Debug.LogError("BodyAngleParametersDataAnalysis is not assigned!");
      return;
    }

    StartCoroutine(WaitForDataAndAnalyze());
  }

  private IEnumerator WaitForDataAndAnalyze()
  {
    while (!IsDataLoaded())
    {
      yield return null;
    }

    var (handToMouthMetrics2D, positionMetrics2D) = AnalyzePositionData2D(dataReader.positionData2DList);

    Debug.Log("Setting 2D hand-to-mouth metrics...");
    Debug.Log($"HandToMouthMetrics2D: {JsonUtility.ToJson(handToMouthMetrics2D)}");
    bodyAngleParametersDataAnalysis.SetHandToMouthMetrics2D(handToMouthMetrics2D);

    Debug.Log("Setting 2D hip center metrics...");
    Debug.Log($"PositionMetrics2D: {JsonUtility.ToJson(positionMetrics2D)}");
    bodyAngleParametersDataAnalysis.SetHipCenterMetrics2D(positionMetrics2D);

    yield return StartCoroutine(bodyAngleParametersDataAnalysis.ProcessBAPFile());
  }

  private bool IsDataLoaded()
  {
    return dataReader.positionData2DList.Count > 0;
  }

  private (HandToMouthMetrics, PositionStatistics) AnalyzePositionData2D(List<PositionData2D> positionData2DList)
  {
    List<float> distancesLeftHandToMouth = new List<float>();
    List<float> distancesRightHandToMouth = new List<float>();
    List<float> distancesLeftHandToHipCenter = new List<float>();
    List<float> distancesRightHandToHipCenter = new List<float>();
    List<float> distancesLeftAnkleToHipCenter = new List<float>();
    List<float> distancesRightAnkleToHipCenter = new List<float>();

    foreach (var positionData in positionData2DList)
    {
      float distanceLeftHandToMouth = Vector2.Distance(positionData.LeftWrist, positionData.MouthLeft);
      float distanceRightHandToMouth = Vector2.Distance(positionData.RightWrist, positionData.MouthRight);

      distancesLeftHandToMouth.Add(distanceLeftHandToMouth);
      distancesRightHandToMouth.Add(distanceRightHandToMouth);

  
    }

    Debug.Log($"DistancesLeftHandToMouth: {string.Join(", ", distancesLeftHandToMouth)}");
    Debug.Log($"DistancesRightHandToMouth: {string.Join(", ", distancesRightHandToMouth)}");


    var handToMouthMetrics2D = new HandToMouthMetrics
    {
      MaxDistanceLeftHandToMouth = distancesLeftHandToMouth.Max(),
      MinDistanceLeftHandToMouth = distancesLeftHandToMouth.Min(),
      AvgDistanceLeftHandToMouth = distancesLeftHandToMouth.Average(),
      MaxDistanceRightHandToMouth = distancesRightHandToMouth.Max(),
      MinDistanceRightHandToMouth = distancesRightHandToMouth.Min(),
      AvgDistanceRightHandToMouth = distancesRightHandToMouth.Average()
    };

    var positionMetrics2D = new PositionStatistics
    {
      LeftHandToHipCenter2D = new BodyPartDistanceMetrics
      {
        MaxDistance = distancesLeftHandToHipCenter.Max(),
        MinDistance = distancesLeftHandToHipCenter.Min(),
        AvgDistance = distancesLeftHandToHipCenter.Average()
      },
      RightHandToHipCenter2D = new BodyPartDistanceMetrics
      {
        MaxDistance = distancesRightHandToHipCenter.Max(),
        MinDistance = distancesRightHandToHipCenter.Min(),
        AvgDistance = distancesRightHandToHipCenter.Average()
      },
      LeftAnkleToHipCenter2D = new BodyPartDistanceMetrics
      {
        MaxDistance = distancesLeftAnkleToHipCenter.Max(),
        MinDistance = distancesLeftAnkleToHipCenter.Min(),
        AvgDistance = distancesLeftAnkleToHipCenter.Average()
      },
      RightAnkleToHipCenter2D = new BodyPartDistanceMetrics
      {
        MaxDistance = distancesRightAnkleToHipCenter.Max(),
        MinDistance = distancesRightAnkleToHipCenter.Min(),
        AvgDistance = distancesRightAnkleToHipCenter.Average()
      }
    };

    return (handToMouthMetrics2D, positionMetrics2D);
  }
}
