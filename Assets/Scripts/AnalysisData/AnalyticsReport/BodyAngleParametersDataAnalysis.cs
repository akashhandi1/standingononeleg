using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class BodyAngleParametersDataAnalysis : MonoBehaviour
{
  public DataReader dataReader;
  public GaitAnalyticsData gaitAnalyticsData; // Reference to GaitAnalyticsData
  public FootPlacementMetrics footPlacementMetrics; // Reference to FootPlacementMetrics

  private HandToMouthMetrics handToMouthMetrics2D; // Store the hand-to-mouth metrics for 2D
  private HandToMouthMetrics handToMouthMetrics3D; // Store the hand-to-mouth metrics for 3D
  private PositionStatistics positionMetrics2D; // Store the position metrics for 2D
  private PositionStatistics positionMetrics3D; // Store the position metrics for 3D

  public IEnumerator ProcessBAPFile()
  {
    if (dataReader == null || string.IsNullOrEmpty(dataReader.selectedSessionFolderPath))
    {
      Debug.LogWarning("DataReader or selected session folder path is not set.");
      yield break;
    }

    var bapFilePath = Path.Combine(dataReader.selectedSessionFolderPath, "bapresults.json");
    var bapData = dataReader.bodyAngleDataList;
    if (bapData == null)
    {
      Debug.LogWarning("Body angle data list is not set.");
      yield break;
    }

    var statistics = CalculateStatistics(bapData);

    // Calculate gait metrics and add to statistics
    if (gaitAnalyticsData != null)
    {
      var gaitMetrics = gaitAnalyticsData.CalculateGaitMetrics(bapData);
      statistics.GaitMetrics = gaitMetrics;
    }
    else
    {
      Debug.LogWarning("GaitAnalyticsData is not set.");
    }

    // Calculate foot placement metrics and add to statistics
    if (footPlacementMetrics != null)
    {
      var footMetrics = footPlacementMetrics.CalculateFootMetrics(dataReader.balanceMatDataList);
      statistics.FootMetrics = footMetrics;
    }
    else
    {
      Debug.LogWarning("FootPlacementMetrics is not set.");
    }

    // Add hand-to-mouth metrics and hip center metrics to statistics
    if (handToMouthMetrics2D != null)
    {
      statistics.PositionMetrics.HandToMouthMetrics2D = handToMouthMetrics2D;
      Debug.Log($"Added HandToMouthMetrics2D: {JsonUtility.ToJson(handToMouthMetrics2D)}");
    }
    else
    {
      Debug.LogWarning("HandToMouthMetrics2D is null.");
    }

    if (handToMouthMetrics3D != null)
    {
      statistics.PositionMetrics.HandToMouthMetrics3D = handToMouthMetrics3D;
      Debug.Log($"Added HandToMouthMetrics3D: {JsonUtility.ToJson(handToMouthMetrics3D)}");
    }
    else
    {
      Debug.LogWarning("HandToMouthMetrics3D is null.");
    }

    if (positionMetrics2D != null)
    {
      statistics.PositionMetrics.LeftHandToHipCenter2D = positionMetrics2D.LeftHandToHipCenter2D;
      statistics.PositionMetrics.RightHandToHipCenter2D = positionMetrics2D.RightHandToHipCenter2D;
      statistics.PositionMetrics.LeftAnkleToHipCenter2D = positionMetrics2D.LeftAnkleToHipCenter2D;
      statistics.PositionMetrics.RightAnkleToHipCenter2D = positionMetrics2D.RightAnkleToHipCenter2D;
      Debug.Log($"Added PositionMetrics2D: {JsonUtility.ToJson(positionMetrics2D)}");
    }
    else
    {
      Debug.LogWarning("PositionMetrics2D is null.");
    }

    if (positionMetrics3D != null)
    {
      statistics.PositionMetrics.LeftHandToHipCenter3D = positionMetrics3D.LeftHandToHipCenter3D;
      statistics.PositionMetrics.RightHandToHipCenter3D = positionMetrics3D.RightHandToHipCenter3D;
      statistics.PositionMetrics.LeftAnkleToHipCenter3D = positionMetrics3D.LeftAnkleToHipCenter3D;
      statistics.PositionMetrics.RightAnkleToHipCenter3D = positionMetrics3D.RightAnkleToHipCenter3D;
      Debug.Log($"Added PositionMetrics3D: {JsonUtility.ToJson(positionMetrics3D)}");
    }
    else
    {
      Debug.LogWarning("PositionMetrics3D is null.");
    }

    SaveStatistics(statistics, bapFilePath);
  }

  public BAPStatistics CalculateStatistics(List<BodyAngles> bapData)
  {
    var stats = new BAPStatistics();

    var properties = typeof(BodyAngles).GetFields().Where(f => f.FieldType == typeof(float)); // Only numeric fields
    foreach (var prop in properties)
    {
      var values = bapData.Select(b => Mathf.Abs((float)prop.GetValue(b))).ToList(); // Convert to absolute values

      if (!values.Any()) // If there are no values, skip the property
      {
        continue;
      }

      float min = values.Min();
      float max = values.Max();
      float rangeOfMotion = max - min;

      stats.Stats[prop.Name] = new BAPStatistic
      {
        Min = min,
        Max = max,
        RangeOfMotion = rangeOfMotion
      };
    }

    return stats;
  }

  public void SetHandToMouthMetrics2D(HandToMouthMetrics metrics)
  {
    Debug.Log($"Updating HandToMouthMetrics2D: {JsonUtility.ToJson(metrics)}");
    handToMouthMetrics2D = metrics;
  }

  public void SetHandToMouthMetrics3D(HandToMouthMetrics metrics)
  {
    Debug.Log($"Updating HandToMouthMetrics3D: {JsonUtility.ToJson(metrics)}");
    handToMouthMetrics3D = metrics;
  }

  public void SetHipCenterMetrics2D(PositionStatistics metrics)
  {
    Debug.Log($"Updating PositionMetrics2D: {JsonUtility.ToJson(metrics)}");
    positionMetrics2D = metrics;
  }

  public void SetHipCenterMetrics3D(PositionStatistics metrics)
  {
    Debug.Log($"Updating PositionMetrics3D: {JsonUtility.ToJson(metrics)}");
    positionMetrics3D = metrics;
  }

  private void SaveStatistics(BAPStatistics statistics, string filePath)
  {
    string json = JsonConvert.SerializeObject(statistics, Formatting.Indented);
    File.WriteAllText(filePath, json);
    Debug.Log("BAP statistics saved to " + filePath);
  }
}
