using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FootPlacementMetrics : MonoBehaviour
{
  public FootMetrics CalculateFootMetrics(List<BalanceMatData> balanceMatDataList)
  {
    var footMetrics = new FootMetrics();

    // Calculate averages and standard deviations
    CalculateSensorAverages(balanceMatDataList, footMetrics);

    // Determine movement patterns
    footMetrics.LeftHeelToFrontCount = CalculatePatternCount(balanceMatDataList, data => IsHeelToFrontPattern(data.GetLeftSensorData()));
    footMetrics.LeftFrontToHeelCount = CalculatePatternCount(balanceMatDataList, data => IsFrontToHeelPattern(data.GetLeftSensorData()));
    footMetrics.RightHeelToFrontCount = CalculatePatternCount(balanceMatDataList, data => IsHeelToFrontPattern(data.GetRightSensorData()));
    footMetrics.RightFrontToHeelCount = CalculatePatternCount(balanceMatDataList, data => IsFrontToHeelPattern(data.GetRightSensorData()));

    // Determine common order
    footMetrics.LeftFootPlacementOrder = DetermineCommonOrder(balanceMatDataList.Select(data => GetFootPlacementOrder(data.GetLeftSensorData())).ToList());
    footMetrics.RightFootPlacementOrder = DetermineCommonOrder(balanceMatDataList.Select(data => GetFootPlacementOrder(data.GetRightSensorData())).ToList());

    return footMetrics;
  }

  private void CalculateSensorAverages(List<BalanceMatData> balanceMatDataList, FootMetrics footMetrics)
  {
    // Left Foot Front
    footMetrics.LeftFrontAvg = CalculateAverage(balanceMatDataList, data => CalculateOverlappingAverage(data.GetLeftSensorData(), 0, 5));
    footMetrics.LeftFrontStdDev = CalculateStandardDeviation(balanceMatDataList.Select(data => CalculateOverlappingAverage(data.GetLeftSensorData(), 0, 5)).ToList());

    // Left Foot Mid
    footMetrics.LeftMidAvg = CalculateAverage(balanceMatDataList, data => CalculateOverlappingAverage(data.GetLeftSensorData(), 3, 7));
    footMetrics.LeftMidStdDev = CalculateStandardDeviation(balanceMatDataList.Select(data => CalculateOverlappingAverage(data.GetLeftSensorData(), 3, 7)).ToList());

    // Left Foot Heel
    footMetrics.LeftHeelAvg = CalculateAverage(balanceMatDataList, data => CalculateOverlappingAverage(data.GetLeftSensorData(), 9, 4));
    footMetrics.LeftHeelStdDev = CalculateStandardDeviation(balanceMatDataList.Select(data => CalculateOverlappingAverage(data.GetLeftSensorData(), 9, 4)).ToList());

    // Right Foot Front
    footMetrics.RightFrontAvg = CalculateAverage(balanceMatDataList, data => CalculateOverlappingAverage(data.GetRightSensorData(), 0, 5));
    footMetrics.RightFrontStdDev = CalculateStandardDeviation(balanceMatDataList.Select(data => CalculateOverlappingAverage(data.GetRightSensorData(), 0, 5)).ToList());

    // Right Foot Mid
    footMetrics.RightMidAvg = CalculateAverage(balanceMatDataList, data => CalculateOverlappingAverage(data.GetRightSensorData(), 3, 7));
    footMetrics.RightMidStdDev = CalculateStandardDeviation(balanceMatDataList.Select(data => CalculateOverlappingAverage(data.GetRightSensorData(), 3, 7)).ToList());

    // Right Foot Heel
    footMetrics.RightHeelAvg = CalculateAverage(balanceMatDataList, data => CalculateOverlappingAverage(data.GetRightSensorData(), 9, 4));
    footMetrics.RightHeelStdDev = CalculateStandardDeviation(balanceMatDataList.Select(data => CalculateOverlappingAverage(data.GetRightSensorData(), 9, 4)).ToList());
  }


  private float CalculateAverage(List<BalanceMatData> balanceMatDataList, System.Func<BalanceMatData, float> selector)
  {
    if (balanceMatDataList.Count == 0) return 0;
    return balanceMatDataList.Average(selector);
  }

  private int CalculatePatternCount(List<BalanceMatData> balanceMatDataList, System.Func<BalanceMatData, bool> patternFunc)
  {
    int count = 0;
    for (int i = 1; i < balanceMatDataList.Count; i++)
    {
      if (patternFunc(balanceMatDataList[i - 1]) && patternFunc(balanceMatDataList[i]))
      {
        count++;
      }
    }
    return count;
  }

  private float CalculateOverlappingAverage(List<ushort> sensorData, int startIndex, int windowSize)
  {
    float sum = 0;
    int count = 0;

    for (int i = startIndex; i < startIndex + windowSize && i < sensorData.Count; i++)
    {
      sum += sensorData[i];
      count++;
    }

    return sum / count;
  }

  private bool IsHeelToFrontPattern(List<ushort> sensorData)
  {
    // Define logic to detect heel to front pattern
    return IsActive(sensorData, 9, 12) && IsActive(sensorData, 0, 4);
  }

  private bool IsFrontToHeelPattern(List<ushort> sensorData)
  {
    // Define logic to detect front to heel pattern
    return IsActive(sensorData, 0, 4) && IsActive(sensorData, 9, 12);
  }

  private bool IsActive(List<ushort> sensorData, int startIndex, int endIndex)
  {
    for (int i = startIndex; i <= endIndex; i++)
    {
      if (sensorData[i] > 0)
      {
        return true;
      }
    }
    return false;
  }

  private string DetermineCommonOrder(List<string> orders)
  {
    return orders.GroupBy(x => x).OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? "Unknown";
  }

  private string GetFootPlacementOrder(List<ushort> sensorData)
  {
    string order = "";

    if (IsActive(sensorData, 0, 4))
    {
      order += "Front-";
    }
    if (IsActive(sensorData, 3, 7))
    {
      order += "Mid-";
    }
    if (IsActive(sensorData, 9, 12))
    {
      order += "Heel";
    }

    return order.TrimEnd('-');
  }

  private float CalculateStandardDeviation(List<float> values)
  {
    if (values.Count == 0) return 0;

    float mean = values.Average();
    float sumOfSquaresOfDifferences = values.Select(val => (val - mean) * (val - mean)).Sum();
    return Mathf.Sqrt(sumOfSquaresOfDifferences / values.Count);
  }
}
