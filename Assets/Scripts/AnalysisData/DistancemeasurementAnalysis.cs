using System.Collections.Generic;

[System.Serializable]
public class HandToMouthMetrics
{
  public float MaxDistanceLeftHandToMouth;
  public float MinDistanceLeftHandToMouth;
  public float AvgDistanceLeftHandToMouth;
  public float MaxDistanceRightHandToMouth;
  public float MinDistanceRightHandToMouth;
  public float AvgDistanceRightHandToMouth;
}

[System.Serializable]
public class BodyPartDistanceMetrics
{
  public float MaxDistance;
  public float MinDistance;
  public float AvgDistance;
}

[System.Serializable]
public class PositionStatistics
{
  public HandToMouthMetrics HandToMouthMetrics2D;
  public HandToMouthMetrics HandToMouthMetrics3D;
  public BodyPartDistanceMetrics LeftHandToHipCenter2D;
  public BodyPartDistanceMetrics RightHandToHipCenter2D;
  public BodyPartDistanceMetrics LeftAnkleToHipCenter2D;
  public BodyPartDistanceMetrics RightAnkleToHipCenter2D;
  public BodyPartDistanceMetrics LeftHandToHipCenter3D;
  public BodyPartDistanceMetrics RightHandToHipCenter3D;
  public BodyPartDistanceMetrics LeftAnkleToHipCenter3D;
  public BodyPartDistanceMetrics RightAnkleToHipCenter3D;
}

[System.Serializable]
public class BAPStatistics
{
  public Dictionary<string, BAPStatistic> Stats = new Dictionary<string, BAPStatistic>(); // Body angle parameter metrics
  public GaitMetrics GaitMetrics; // Gait metrics
  public PositionStatistics PositionMetrics; // Position metrics
  public FootMetrics FootMetrics; // Foot placement metrics
}


[System.Serializable]
public class BAPStatistic
{
  public float Min;
  public float Max;
  public float RangeOfMotion;
}



[System.Serializable]
public class GaitMetrics
{
  public float LeftHipRangeOfMotion;
  public float LeftHipMean;
  public float LeftHipStdDev;

  public float LeftKneeRangeOfMotion;
  public float LeftKneeMean;
  public float LeftKneeStdDev;

  public float LeftAnkleRangeOfMotion;
  public float LeftAnkleMean;
  public float LeftAnkleStdDev;

  public float RightHipRangeOfMotion;
  public float RightHipMean;
  public float RightHipStdDev;

  public float RightKneeRangeOfMotion;
  public float RightKneeMean;
  public float RightKneeStdDev;

  public float RightAnkleRangeOfMotion;
  public float RightAnkleMean;
  public float RightAnkleStdDev;

  public float LeftShoulderAbductionRangeOfMotion;
  public float LeftShoulderAbductionMean;
  public float LeftShoulderAbductionStdDev;

  public float RightShoulderAbductionRangeOfMotion;
  public float RightShoulderAbductionMean;
  public float RightShoulderAbductionStdDev;
}

[System.Serializable]
public class FootMetrics
{
  public float LeftFrontAvg;
  public float LeftFrontStdDev;

  public float LeftMidAvg;
  public float LeftMidStdDev;

  public float LeftHeelAvg;
  public float LeftHeelStdDev;

  public float RightFrontAvg;
  public float RightFrontStdDev;

  public float RightMidAvg;
  public float RightMidStdDev;

  public float RightHeelAvg;
  public float RightHeelStdDev;

  public int LeftHeelToFrontCount;
  public int LeftFrontToHeelCount;
  public int RightHeelToFrontCount;
  public int RightFrontToHeelCount;

  public string LeftFootPlacementOrder;
  public string RightFootPlacementOrder;
}

