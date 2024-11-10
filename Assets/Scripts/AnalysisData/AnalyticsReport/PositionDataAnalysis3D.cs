using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PositionDataAnalysis3D : MonoBehaviour
{
  public DataReader dataReader;

  void Start()
  {
    if (dataReader == null)
    {
      Debug.LogError("DataReader is not assigned!");
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

    CalculateHandToMouthDistances(dataReader.positionDataList);
  }

  private bool IsDataLoaded()
  {
    return dataReader.positionDataList.Count > 0;
  }

  private void CalculateHandToMouthDistances(List<PositionData> positionDataList)
  {
    List<float> leftHandToMouthDistances = new List<float>();
    List<float> rightHandToMouthDistances = new List<float>();

    foreach (var positionData in positionDataList)
    {
      float distanceLeftHandToMouth = MathUtilities.CalculateDistance(positionData.LeftWrist, positionData.MouthLeft);
      float distanceRightHandToMouth = MathUtilities.CalculateDistance(positionData.RightWrist, positionData.MouthRight);

      leftHandToMouthDistances.Add(distanceLeftHandToMouth);
      rightHandToMouthDistances.Add(distanceRightHandToMouth);
    }

    float maxLeftHandToMouth = leftHandToMouthDistances.Max();
    float minLeftHandToMouth = leftHandToMouthDistances.Min();

    float maxRightHandToMouth = rightHandToMouthDistances.Max();
    float minRightHandToMouth = rightHandToMouthDistances.Min();

    Debug.Log($"Left Hand to Mouth - Max Distance: {maxLeftHandToMouth}, Min Distance: {minLeftHandToMouth}");
    Debug.Log($"Right Hand to Mouth - Max Distance: {maxRightHandToMouth}, Min Distance: {minRightHandToMouth}");
  }
}
