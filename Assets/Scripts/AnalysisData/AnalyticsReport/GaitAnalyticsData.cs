using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GaitAnalyticsData : MonoBehaviour
{
  public GaitMetrics CalculateGaitMetrics(List<BodyAngles> bapData)
  {
    var gaitMetrics = new GaitMetrics();

    // Left Hip
    gaitMetrics.LeftHipRangeOfMotion = CalculateRangeOfMotion(bapData.Select(b => b.leftHipFlexion).ToList());
    gaitMetrics.LeftHipMean = CalculateMean(bapData.Select(b => b.leftHipFlexion).ToList());
    gaitMetrics.LeftHipStdDev = CalculateStandardDeviation(bapData.Select(b => b.leftHipFlexion).ToList());

    // Left Knee
    gaitMetrics.LeftKneeRangeOfMotion = CalculateRangeOfMotion(bapData.Select(b => b.leftKneeFlexion_Extension).ToList());
    gaitMetrics.LeftKneeMean = CalculateMean(bapData.Select(b => b.leftKneeFlexion_Extension).ToList());
    gaitMetrics.LeftKneeStdDev = CalculateStandardDeviation(bapData.Select(b => b.leftKneeFlexion_Extension).ToList());

    // Left Ankle
    gaitMetrics.LeftAnkleRangeOfMotion = CalculateRangeOfMotion(bapData.Select(b => b.leftAnkleDorsiflexion).ToList(), bapData.Select(b => b.leftAnklePlantarflexion).ToList());
    gaitMetrics.LeftAnkleMean = CalculateMean(bapData.Select(b => b.leftAnkleDorsiflexion).Concat(bapData.Select(b => b.leftAnklePlantarflexion)).ToList());
    gaitMetrics.LeftAnkleStdDev = CalculateStandardDeviation(bapData.Select(b => b.leftAnkleDorsiflexion).Concat(bapData.Select(b => b.leftAnklePlantarflexion)).ToList());

    // Right Hip
    gaitMetrics.RightHipRangeOfMotion = CalculateRangeOfMotion(bapData.Select(b => b.rightHipFlexion).ToList());
    gaitMetrics.RightHipMean = CalculateMean(bapData.Select(b => b.rightHipFlexion).ToList());
    gaitMetrics.RightHipStdDev = CalculateStandardDeviation(bapData.Select(b => b.rightHipFlexion).ToList());

    // Right Knee
    gaitMetrics.RightKneeRangeOfMotion = CalculateRangeOfMotion(bapData.Select(b => b.rightKneeFlexion_Extension).ToList());
    gaitMetrics.RightKneeMean = CalculateMean(bapData.Select(b => b.rightKneeFlexion_Extension).ToList());
    gaitMetrics.RightKneeStdDev = CalculateStandardDeviation(bapData.Select(b => b.rightKneeFlexion_Extension).ToList());

    // Right Ankle
    gaitMetrics.RightAnkleRangeOfMotion = CalculateRangeOfMotion(bapData.Select(b => b.rightAnkleDorsiflexion).ToList(), bapData.Select(b => b.rightAnklePlantarflexion).ToList());
    gaitMetrics.RightAnkleMean = CalculateMean(bapData.Select(b => b.rightAnkleDorsiflexion).Concat(bapData.Select(b => b.rightAnklePlantarflexion)).ToList());
    gaitMetrics.RightAnkleStdDev = CalculateStandardDeviation(bapData.Select(b => b.rightAnkleDorsiflexion).Concat(bapData.Select(b => b.rightAnklePlantarflexion)).ToList());

    // Left Shoulder Abduction
    gaitMetrics.LeftShoulderAbductionRangeOfMotion = CalculateRangeOfMotion(bapData.Select(b => b.leftShoulderSideAbduction_Adduction).ToList());
    gaitMetrics.LeftShoulderAbductionMean = CalculateMean(bapData.Select(b => b.leftShoulderSideAbduction_Adduction).ToList());
    gaitMetrics.LeftShoulderAbductionStdDev = CalculateStandardDeviation(bapData.Select(b => b.leftShoulderSideAbduction_Adduction).ToList());

    // Right Shoulder Abduction
    gaitMetrics.RightShoulderAbductionRangeOfMotion = CalculateRangeOfMotion(bapData.Select(b => b.rightShoulderSideAbduction_Adduction).ToList());
    gaitMetrics.RightShoulderAbductionMean = CalculateMean(bapData.Select(b => b.rightShoulderSideAbduction_Adduction).ToList());
    gaitMetrics.RightShoulderAbductionStdDev = CalculateStandardDeviation(bapData.Select(b => b.rightShoulderSideAbduction_Adduction).ToList());

    return gaitMetrics;
  }

  private float CalculateRangeOfMotion(List<float> flexionValues, List<float> extensionValues = null)
  {
    float minFlexion = flexionValues.Min();
    float maxFlexion = flexionValues.Max();
    float rangeOfMotion = maxFlexion - minFlexion;

    if (extensionValues != null)
    {
      float minExtension = extensionValues.Min();
      float maxExtension = extensionValues.Max();
      rangeOfMotion += maxExtension - minExtension;
    }

    return rangeOfMotion;
  }

  private float CalculateMean(List<float> values)
  {
    if (values.Count == 0) return 0;
    return values.Sum() / values.Count;
  }

  private float CalculateStandardDeviation(List<float> values)
  {
    if (values.Count == 0) return 0;

    float mean = CalculateMean(values);
    float sumOfSquaresOfDifferences = values.Select(val => (val - mean) * (val - mean)).Sum();
    return Mathf.Sqrt(sumOfSquaresOfDifferences / values.Count);
  }
}
