using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataReader : MonoBehaviour
{
    public string selectedSessionFolderPath;
    public string selectedSessionFolderName;
    public LoadingScreenController loadingScreenController; // Reference to the loading screen controller
    public BodyAngleParametersDataAnalysis bapGaitProcessor; // Reference to BAPGaitProcessor

    [Header("File Lists")]
    [SerializeField]
    private List<string> bjaFiles = new List<string>();
    [SerializeField]
    private List<string> btrFiles = new List<string>();
    [SerializeField]
    private List<string> bodyBtr2dFiles = new List<string>();
    [SerializeField]
    private List<string> bapFiles = new List<string>();
    [SerializeField]
    private List<string> leftHandFiles = new List<string>();
    [SerializeField]
    private List<string> rightHandFiles = new List<string>();
  [SerializeField]
  private List<string> bmrFiles = new List<string>();


  [Header("Balance Mat Data")]
  [SerializeField]
  public List<BalanceMatData> balanceMatDataList = new List<BalanceMatData>();



  [Header("Joint Angle Data")]
    [SerializeField]
    public List<JointAngleData> jointAngleDataList = new List<JointAngleData>();

    [Header("Position Data")]
    [SerializeField]
    public List<PositionData> positionDataList = new List<PositionData>();

    [Header("Position Data 2D")]
    [SerializeField]
    public List<PositionData2D> positionData2DList = new List<PositionData2D>();

  [Header("Left Hand 2D Position Data")]
  [SerializeField]
  public List<PositionLeftHand2D> leftHandPositionData2DList = new List<PositionLeftHand2D>();

  [Header("Right Hand 2D Position Data")]
  [SerializeField]
  public List<PositionRightHand2D> rightHandPositionData2DList = new List<PositionRightHand2D>();

  [Header("Body Angle Parameters")]
    [SerializeField]
    public List<BodyAngles> bodyAngleDataList = new List<BodyAngles>();

  public void LoadData()
  {
    if (string.IsNullOrEmpty(selectedSessionFolderPath))
    {
      Debug.LogError("Selected session folder path is not set or is empty.");
      return;
    }

    StartCoroutine(LoadDataCoroutine());
  }

  private IEnumerator LoadDataCoroutine()
  {
    loadingScreenController.ShowLoadingScreen("Please wait until data is loaded...");

    // Clear previous data
    jointAngleDataList.Clear();
    positionDataList.Clear();
    positionData2DList.Clear();
    bodyAngleDataList.Clear();
    leftHandPositionData2DList.Clear();
    rightHandPositionData2DList.Clear();
    balanceMatDataList.Clear();

    // Load .bja files
    bjaFiles = Directory.GetFiles(selectedSessionFolderPath, "*.bja").ToList();
    foreach (var file in bjaFiles)
    {
      ReadJointAngleData(file);
      yield return null; // Yield to keep UI responsive
    }

    // Load .btr files
    btrFiles = Directory.GetFiles(selectedSessionFolderPath, "*.btr").ToList();
    foreach (var file in btrFiles)
    {
      ReadPositionData(file);
      yield return null; // Yield to keep UI responsive
    }

    // Load _body.btr2d files
    bodyBtr2dFiles = Directory.GetFiles(selectedSessionFolderPath, "*_body.btr2d").ToList();
    foreach (var file in bodyBtr2dFiles)
    {
      ReadPositionData2D(file);
      yield return null; // Yield to keep UI responsive
    }

    // Load .bap files
    bapFiles = Directory.GetFiles(selectedSessionFolderPath, "*.bap").ToList();
    foreach (var file in bapFiles)
    {
      ReadBodyAngleParameters(file);
      yield return null; // Yield to keep UI responsive
    }

    // Load _leftHand.btr2d files
    leftHandFiles = Directory.GetFiles(selectedSessionFolderPath, "*_leftHand.btr2d").ToList();
    foreach (var file in leftHandFiles)
    {
      ReadLeftHandPositionData2D(file);
      yield return null; // Yield to keep UI responsive
    }

    // Load _rightHand.btr2d files
    rightHandFiles = Directory.GetFiles(selectedSessionFolderPath, "*_rightHand.btr2d").ToList();
    foreach (var file in rightHandFiles)
    {
      ReadRightHandPositionData2D(file);
      yield return null; // Yield to keep UI responsive
    }

    // Load .bmr files
    bmrFiles = Directory.GetFiles(selectedSessionFolderPath, "*.bmr").ToList();
    foreach (var file in bmrFiles)
    {
      ReadBalanceMatData(file);
      yield return null; // Yield to keep UI responsive
    }

    Debug.Log("Data loaded successfully.");

    if (bapGaitProcessor != null)
    {
      yield return StartCoroutine(bapGaitProcessor.ProcessBAPFile());
    }

    loadingScreenController.HideLoadingScreen();
  }





  void ReadJointAngleData(string filePath)
  {
    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines)
    {
      try
      {
        // Split the line by commas and remove empty entries
        var data = line.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

        if (data.Length != 12)
        {
          Debug.LogError($"Invalid number of elements in line: {line}. Expected 12, got {data.Length}");
          continue;
        }

        float leftShoulderAngle, leftHandElbowAngle, leftLegHipAngle, leftLegKneeAngle;
        float rightShoulderAngle, rightHandElbowAngle, rightLegHipAngle, rightLegKneeAngle;
        float neckAngle, pelvisAngle, leftLegAnkleAngle, rightLegAnkleAngle;

        if (float.TryParse(data[0], out leftShoulderAngle) &&
            float.TryParse(data[1], out leftHandElbowAngle) &&
            float.TryParse(data[2], out leftLegHipAngle) &&
            float.TryParse(data[3], out leftLegKneeAngle) &&
            float.TryParse(data[4], out rightShoulderAngle) &&
            float.TryParse(data[5], out rightHandElbowAngle) &&
            float.TryParse(data[6], out rightLegHipAngle) &&
            float.TryParse(data[7], out rightLegKneeAngle) &&
            float.TryParse(data[8], out neckAngle) &&
            float.TryParse(data[9], out pelvisAngle) &&
            float.TryParse(data[10], out leftLegAnkleAngle) &&
            float.TryParse(data[11], out rightLegAnkleAngle))
        {
          // Create a new JointAngleData instance and assign the parsed angles
          JointAngleData jointAngleData = new JointAngleData
          {
            LeftShoulderAngle = leftShoulderAngle,
            LeftHandElbowAngle = leftHandElbowAngle,
            LeftLegHipAngle = leftLegHipAngle,
            LeftLegKneeAngle = leftLegKneeAngle,
            RightShoulderAngle = rightShoulderAngle,
            RightHandElbowAngle = rightHandElbowAngle,
            RightLegHipAngle = rightLegHipAngle,
            RightLegKneeAngle = rightLegKneeAngle,
            NeckAngle = neckAngle,
            PelvisAngle = pelvisAngle,
            LeftLegAnkleAngle = leftLegAnkleAngle,
            RightLegAnkleAngle = rightLegAnkleAngle
          };
          jointAngleDataList.Add(jointAngleData);
        }
        else
        {
          Debug.LogError($"Failed to parse angles in line: {line}");
        }
      }
      catch (System.Exception ex)
      {
        Debug.LogError($"Error reading line: {line}. Exception: {ex.Message}");
      }
    }
  }
  void ReadPositionData(string filePath)
  {
    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(line))
        {
          continue;
        }

        // Split the data into coordinates and timestamp
        var data = line.Split('/');
        if (data.Length != 2)
        {
         // Debug.LogError($"Invalid line format: {line}");
          continue;
        }

        // Trim whitespace from data
        var coordinatesPart = data[0].Trim();
        var coordinates = coordinatesPart.Split(';');

        if (coordinates.Length == 0) // Ensure there are positions
        {
         // Debug.LogError($"Unexpected number of coordinates: {coordinates.Length} in line: {line}");
          continue;
        }

        var positions = new List<Vector3>();
        foreach (var coordinate in coordinates)
        {
          var trimmedCoordinate = coordinate.Trim();
          if (string.IsNullOrEmpty(trimmedCoordinate))
          {
           // Debug.LogError($"Empty coordinate detected in line: {line}");
            continue; // Skip empty coordinate
          }

          var coords = trimmedCoordinate.Split(',');
          if (coords.Length != 3)
          {
            Debug.LogError($"Invalid coordinate format: {trimmedCoordinate}. Coordinates length: {coords.Length}");
            continue; // Skip invalid coordinate
          }

        //  Debug.Log($"Coordinate split result: {coords[0]}, {coords[1]}, {coords[2]}");

          float x, y, z;
          if (float.TryParse(coords[0], out x) && float.TryParse(coords[1], out y) && float.TryParse(coords[2], out z))
          {
            positions.Add(new Vector3(x, y, z));
          }
          else
          {
            Debug.LogError($"Failed to parse coordinates: {trimmedCoordinate}");
          }
        }

        // Parse the timestamp
        if (!long.TryParse(data[1].Trim(), out long timestamp))
        {
          Debug.LogError($"Invalid timestamp format: {data[1]}");
          continue;
        }

        if (positions.Count != 33) // Ensure there are 33 valid positions
        {
          Debug.LogError($"Unexpected number of valid coordinates: {positions.Count} in line: {line}");
          continue;
        }

        // Create a new PositionData instance and assign the parsed vectors
        PositionData positionData = new PositionData
        {
          Nose = positions[0],
          LeftEyeInner = positions[1],
          LeftEye = positions[2],
          LeftEyeOuter = positions[3],
          RightEyeInner = positions[4],
          RightEye = positions[5],
          RightEyeOuter = positions[6],
          LeftEar = positions[7],
          RightEar = positions[8],
          MouthLeft = positions[9],
          MouthRight = positions[10],
          LeftShoulder = positions[11],
          RightShoulder = positions[12],
          LeftElbow = positions[13],
          RightElbow = positions[14],
          LeftWrist = positions[15],
          RightWrist = positions[16],
          LeftPinky = positions[17],
          RightPinky = positions[18],
          LeftIndex = positions[19],
          RightIndex = positions[20],
          LeftThumb = positions[21],
          RightThumb = positions[22],
          LeftHip = positions[23],
          RightHip = positions[24],
          LeftKnee = positions[25],
          RightKnee = positions[26],
          LeftAnkle = positions[27],
          RightAnkle = positions[28],
          LeftHeel = positions[29],
          RightHeel = positions[30],
          LeftFootIndex = positions[31],
          RightFootIndex = positions[32],
          TimeStamp = timestamp
        };

        positionDataList.Add(positionData);
      }
      catch (System.Exception ex)
      {
        Debug.LogError($"Error reading line: {line}. Exception: {ex.Message}");
      }
    }
  }
  void ReadPositionData2D(string filePath)
  {
    Debug.Log("Reading _body.btr2d file: " + filePath);

    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(line))
        {
         // Debug.LogWarning("Skipping empty line.");
          continue;
        }

        var data = line.Split(';');
        if (data.Length < 2 && !data[0].Contains('/'))
        {
         // Debug.LogWarning($"Invalid line format: {line}");
          continue;
        }

        List<Vector2> positions = new List<Vector2>();
        int expectedPositionsCount = 33;

        for (int i = 0; i < data.Length - 1; i++)
        {
          var coords = data[i].Split(',');
          if (coords.Length != 2)
          {
          //  Debug.LogWarning($"Invalid coordinate data at index {i}: {data[i]}");
            positions.Add(Vector2.zero); // Add zero vector for invalid data
            continue;
          }
          positions.Add(new Vector2(float.Parse(coords[0]), float.Parse(coords[1])));
        }

        var timestampData = data[data.Length - 1].Split('/');
        if (timestampData.Length != 2)
        {
         // Debug.LogWarning($"Invalid timestamp data: {data[data.Length - 1]}");
          positions.Add(Vector2.zero); // Add zero vector for invalid data
          timestampData = new string[] { "0,0", "0" }; // Add default timestamp
        }

        var lastCoords = timestampData[0].Split(',');
        if (lastCoords.Length != 2)
        {
         // Debug.LogWarning($"Invalid last coordinate data: {timestampData[0]}");
          positions.Add(Vector2.zero); // Add zero vector for invalid data
        }
        else
        {
          positions.Add(new Vector2(float.Parse(lastCoords[0]), float.Parse(lastCoords[1])));
        }

        // Add zeros if the number of positions is less than expected
        while (positions.Count < expectedPositionsCount)
        {
          positions.Add(Vector2.zero);
        }

        long timestamp = long.Parse(timestampData[1]);

        PositionData2D positionData2D = new PositionData2D
        {
          Nose = positions[0],
          LeftEyeInner = positions[1],
          LeftEye = positions[2],
          LeftEyeOuter = positions[3],
          RightEyeInner = positions[4],
          RightEye = positions[5],
          RightEyeOuter = positions[6],
          LeftEar = positions[7],
          RightEar = positions[8],
          MouthLeft = positions[9],
          MouthRight = positions[10],
          LeftShoulder = positions[11],
          RightShoulder = positions[12],
          LeftElbow = positions[13],
          RightElbow = positions[14],
          LeftWrist = positions[15],
          RightWrist = positions[16],
          LeftPinky = positions[17],
          RightPinky = positions[18],
          LeftIndex = positions[19],
          RightIndex = positions[20],
          LeftThumb = positions[21],
          RightThumb = positions[22],
          LeftHip = positions[23],
          RightHip = positions[24],
          LeftKnee = positions[25],
          RightKnee = positions[26],
          LeftAnkle = positions[27],
          RightAnkle = positions[28],
          LeftHeel = positions[29],
          RightHeel = positions[30],
          LeftFootIndex = positions[31],
          RightFootIndex = positions[32],
          TimeStamp = timestamp
        };

        positionData2DList.Add(positionData2D);
      }
      catch (System.Exception ex)
      {
        Debug.LogError($"Error reading line: {line}. Exception: {ex.Message}");
      }
    }
  }
  void ReadBodyAngleParameters(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            var data = line.Split(',').Select(float.Parse).ToArray();
            var angles = new BodyAngles
            {
                leftShoulderFlexion = data[0],
                leftShoulderExtension = data[1],
                leftElbowFlexion_Extension = data[2],
                leftHipFlexion = data[3],
                leftHipExtension = data[4],
                leftKneeFlexion_Extension = data[5],
                leftAnkleDorsiflexion = data[6],
                leftAnklePlantarflexion = data[7],
                rightShoulderFlexion = data[8],
                rightShoulderExtension = data[9],
                rightElbowFlexion_Extension = data[10],
                rightHipFlexion = data[11],
                rightHipExtension = data[12],
                rightKneeFlexion_Extension = data[13],
                rightAnkleDorsiflexion = data[14],
                rightAnklePlantarflexion = data[15],
                lefttrunkFlexion = data[16],
                lefttrunkExtension = data[17],
                righttrunkFlexion = data[18],
                righttrunkExtension = data[19],
                lefttrunkRotation = data[20],
                righttrunkRotation = data[21],
                lefttrunkLateralFlexion = data[22],
                righttrunkLateralFlexion = data[23],
                leftShoulderSideAbduction_Adduction = data[24],
                rightShoulderSideAbduction_Adduction = data[25],
                leftHipAbduction = data[26],
                leftHipAdduction = data[27],
                rightHipAbduction = data[28],
                rightHipAdduction = data[29],
                Timestamp = (long)data[30]
            };
            bodyAngleDataList.Add(angles);
        }
    }
  void ReadLeftHandPositionData2D(string filePath)
  {
    Debug.Log("Reading _leftHand.btr2d file: " + filePath);

    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(line))
        {
        //  Debug.LogWarning("Skipping empty line.");
          continue;
        }

        var data = line.Split(';');
        if (data.Length < 2 && !data[0].Contains('/'))
        {
        //  Debug.LogWarning($"Invalid line format: {line}");
          continue;
        }

        List<Vector2> positions = new List<Vector2>();
        int expectedPositionsCount = 21;

        for (int i = 0; i < data.Length - 1; i++)
        {
          var coords = data[i].Split(',');
          if (coords.Length != 2)
          {
           // Debug.LogWarning($"Invalid coordinate data at index {i}: {data[i]}");
            positions.Add(Vector2.zero); // Add zero vector for invalid data
            continue;
          }
          positions.Add(new Vector2(float.Parse(coords[0]), float.Parse(coords[1])));
        }

        var timestampData = data[data.Length - 1].Split('/');
        if (timestampData.Length != 2)
        {
        //  Debug.LogWarning($"Invalid timestamp data: {data[data.Length - 1]}");
          positions.Add(Vector2.zero); // Add zero vector for invalid data
          timestampData = new string[] { "0,0", "0" }; // Add default timestamp
        }

        var lastCoords = timestampData[0].Split(',');
        if (lastCoords.Length != 2)
        {
        //  Debug.LogWarning($"Invalid last coordinate data: {timestampData[0]}");
          positions.Add(Vector2.zero); // Add zero vector for invalid data
        }
        else
        {
          positions.Add(new Vector2(float.Parse(lastCoords[0]), float.Parse(lastCoords[1])));
        }

        // Add zeros if the number of positions is less than expected
        while (positions.Count < expectedPositionsCount)
        {
          positions.Add(Vector2.zero);
        }

        long timestamp = long.Parse(timestampData[1]);

        PositionLeftHand2D positionData2D = new PositionLeftHand2D
        {
          Wrist = positions[0],
          Thumb_CMC = positions[1],
          Thumb_MCP = positions[2],
          Thumb_IP = positions[3],
          Thumb_Tip = positions[4],
          Index_MCP = positions[5],
          Index_PIP = positions[6],
          Index_DIP = positions[7],
          Index_Tip = positions[8],
          Middle_MCP = positions[9],
          Middle_PIP = positions[10],
          Middle_DIP = positions[11],
          Middle_Tip = positions[12],
          Ring_MCP = positions[13],
          Ring_PIP = positions[14],
          Ring_DIP = positions[15],
          Ring_Tip = positions[16],
          Pinky_MCP = positions[17],
          Pinky_PIP = positions[18],
          Pinky_DIP = positions[19],
          Pinky_Tip = positions[20],
          TimeStamp = timestamp
        };

        leftHandPositionData2DList.Add(positionData2D);
      }
      catch (System.Exception ex)
      {
        Debug.LogError($"Error reading line: {line}. Exception: {ex.Message}");
      }
    }
  }
  void ReadRightHandPositionData2D(string filePath)
  {
    Debug.Log("Reading _rightHand.btr2d file: " + filePath);

    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(line))
        {
         // Debug.LogWarning("Skipping empty line.");
          continue;
        }

        var data = line.Split(';');
        if (data.Length < 2 && !data[0].Contains('/'))
        {
         // Debug.LogWarning($"Invalid line format: {line}");
          continue;
        }

        List<Vector2> positions = new List<Vector2>();
        int expectedPositionsCount = 21;

        for (int i = 0; i < data.Length - 1; i++)
        {
          var coords = data[i].Split(',');
          if (coords.Length != 2)
          {
           // Debug.LogWarning($"Invalid coordinate data at index {i}: {data[i]}");
            positions.Add(Vector2.zero); // Add zero vector for invalid data
            continue;
          }
          positions.Add(new Vector2(float.Parse(coords[0]), float.Parse(coords[1])));
        }

        var timestampData = data[data.Length - 1].Split('/');
        if (timestampData.Length != 2)
        {
         // Debug.LogWarning($"Invalid timestamp data: {data[data.Length - 1]}");
          positions.Add(Vector2.zero); // Add zero vector for invalid data
          timestampData = new string[] { "0,0", "0" }; // Add default timestamp
        }

        var lastCoords = timestampData[0].Split(',');
        if (lastCoords.Length != 2)
        {
         // Debug.LogWarning($"Invalid last coordinate data: {timestampData[0]}");
          positions.Add(Vector2.zero); // Add zero vector for invalid data
        }
        else
        {
          positions.Add(new Vector2(float.Parse(lastCoords[0]), float.Parse(lastCoords[1])));
        }

        // Add zeros if the number of positions is less than expected
        while (positions.Count < expectedPositionsCount)
        {
          positions.Add(Vector2.zero);
        }

        long timestamp = long.Parse(timestampData[1]);

        PositionRightHand2D positionData2D = new PositionRightHand2D
        {
          Wrist = positions[0],
          Thumb_CMC = positions[1],
          Thumb_MCP = positions[2],
          Thumb_IP = positions[3],
          Thumb_Tip = positions[4],
          Index_MCP = positions[5],
          Index_PIP = positions[6],
          Index_DIP = positions[7],
          Index_Tip = positions[8],
          Middle_MCP = positions[9],
          Middle_PIP = positions[10],
          Middle_DIP = positions[11],
          Middle_Tip = positions[12],
          Ring_MCP = positions[13],
          Ring_PIP = positions[14],
          Ring_DIP = positions[15],
          Ring_Tip = positions[16],
          Pinky_MCP = positions[17],
          Pinky_PIP = positions[18],
          Pinky_DIP = positions[19],
          Pinky_Tip = positions[20],
          TimeStamp = timestamp
        };

        rightHandPositionData2DList.Add(positionData2D);
      }
      catch (System.Exception ex)
      {
        Debug.LogError($"Error reading line: {line}. Exception: {ex.Message}");
      }
    }
  }

  void ReadBalanceMatData(string filePath)
  {
    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines)
    {
      try
      {
        // Split the line by the '-' delimiter to separate data from the timestamp
        var dataStringsTemp = line.Split('-');

        // Split the data part by commas
        var dataStrings = dataStringsTemp[0].Split(',');

        if (dataStrings.Length == 58)
        {
          byte[] data = new byte[58];
          for (int i = 0; i < 58; i++)
          {
            if (byte.TryParse(dataStrings[i], out byte byteValue))
            {
              data[i] = byteValue;
            }
            else
            {
              Debug.LogError("Failed to parse byte from line: " + dataStrings[i]);
              break; // Exit the loop on error
            }
          }

          // Decode the data
          ushort[] decodedValues = DecodeData(data);

          if (decodedValues.Length != 28)
          {
            Debug.LogError("Decoded values length is not 28");
            continue;
          }

          // Create a new BalanceMatData instance and assign the parsed values, skipping the first two values
          BalanceMatData balanceMatData = new BalanceMatData
          {
            leftSensor0 = decodedValues[9],
            leftSensor1 = decodedValues[4],
            leftSensor2 = decodedValues[12],
            leftSensor3 = decodedValues[6],
            leftSensor4 = decodedValues[14],
            leftSensor5 = decodedValues[8],
            leftSensor6 = decodedValues[3],
            leftSensor7 = decodedValues[11],
            leftSensor8 = decodedValues[5],
            leftSensor9 = decodedValues[13],
            leftSensor10 = decodedValues[7],
            leftSensor11 = decodedValues[2],
            leftSensor12 = decodedValues[10],

            rightSensor0 = decodedValues[27],
            rightSensor1 = decodedValues[22],
            rightSensor2 = decodedValues[17],
            rightSensor3 = decodedValues[24],
            rightSensor4 = decodedValues[19],
            rightSensor5 = decodedValues[26],
            rightSensor6 = decodedValues[21],
            rightSensor7 = decodedValues[16],
            rightSensor8 = decodedValues[23],
            rightSensor9 = decodedValues[18],
            rightSensor10 = decodedValues[25],
            rightSensor11 = decodedValues[20],
            rightSensor12 = decodedValues[15],

            TimeStamp = long.Parse(dataStringsTemp[1])
          };

          balanceMatDataList.Add(balanceMatData);
        }
        else
        {
          Debug.LogError("Invalid data format in line: " + line);
        }
      }
      catch (System.Exception ex)
      {
        Debug.LogError("Error reading line: " + line + ". Exception: " + ex.Message);
      }
    }
  }


  private ushort[] DecodeData(byte[] data)
  {
    const int numIntegers = 28; // 56 bytes / 2 bytes per ushort
    ushort[] decodedValues = new ushort[numIntegers];

    for (int i = 0; i < numIntegers; i++)
    {
      decodedValues[i] = BitConverter.ToUInt16(data, i * 2);
    }

    return decodedValues;
  }









}
// format for .bja
[System.Serializable]
public class JointAngleData
{
    public float LeftShoulderAngle;
    public float LeftHandElbowAngle;
    public float LeftLegHipAngle;
    public float LeftLegKneeAngle;
    public float RightShoulderAngle;
    public float RightHandElbowAngle;
    public float RightLegHipAngle;
    public float RightLegKneeAngle;
    public float NeckAngle;
    public float PelvisAngle;
    public float LeftLegAnkleAngle;
    public float RightLegAnkleAngle;
    public long TimeStamp;  // Add TimeStamp
}

// .btr 3d position data
[System.Serializable]
public class PositionData
{
  public Vector3 Nose;
  public Vector3 LeftEyeInner;
  public Vector3 LeftEye;
  public Vector3 LeftEyeOuter;
  public Vector3 RightEyeInner;
  public Vector3 RightEye;
  public Vector3 RightEyeOuter;
  public Vector3 LeftEar;
  public Vector3 RightEar;
  public Vector3 MouthLeft;
  public Vector3 MouthRight;
  public Vector3 LeftShoulder;
  public Vector3 RightShoulder;
  public Vector3 LeftElbow;
  public Vector3 RightElbow;
  public Vector3 LeftWrist;
  public Vector3 RightWrist;
  public Vector3 LeftPinky;
  public Vector3 RightPinky;
  public Vector3 LeftIndex;
  public Vector3 RightIndex;
  public Vector3 LeftThumb;
  public Vector3 RightThumb;
  public Vector3 LeftHip;
  public Vector3 RightHip;
  public Vector3 LeftKnee;
  public Vector3 RightKnee;
  public Vector3 LeftAnkle;
  public Vector3 RightAnkle;
  public Vector3 LeftHeel;
  public Vector3 RightHeel;
  public Vector3 LeftFootIndex;
  public Vector3 RightFootIndex;
  public long TimeStamp;
}

// _body.btr2d position data
[System.Serializable]
public class PositionData2D
{
    public Vector2 Nose;
    public Vector2 LeftEyeInner;
    public Vector2 LeftEye;
    public Vector2 LeftEyeOuter;
    public Vector2 RightEyeInner;
    public Vector2 RightEye;
    public Vector2 RightEyeOuter;
    public Vector2 LeftEar;
    public Vector2 RightEar;
    public Vector2 MouthLeft;
    public Vector2 MouthRight;
    public Vector2 LeftShoulder;
    public Vector2 RightShoulder;
    public Vector2 LeftElbow;
    public Vector2 RightElbow;
    public Vector2 LeftWrist;
    public Vector2 RightWrist;
    public Vector2 LeftPinky;
    public Vector2 RightPinky;
    public Vector2 LeftIndex;
    public Vector2 RightIndex;
    public Vector2 LeftThumb;
    public Vector2 RightThumb;
    public Vector2 LeftHip;
    public Vector2 RightHip;
    public Vector2 LeftKnee;
    public Vector2 RightKnee;
    public Vector2 LeftAnkle;
    public Vector2 RightAnkle;
    public Vector2 LeftHeel;
    public Vector2 RightHeel;
    public Vector2 LeftFootIndex;
    public Vector2 RightFootIndex;
    public long TimeStamp;
}
//_leftHand.btr2dPosition data of 2d lefthand
[System.Serializable]
public class PositionLeftHand2D
{
    public Vector2 Wrist;
    public Vector2 Thumb_CMC;
    public Vector2 Thumb_MCP;
    public Vector2 Thumb_IP;
    public Vector2 Thumb_Tip;
    public Vector2 Index_MCP;
    public Vector2 Index_PIP;
    public Vector2 Index_DIP;
    public Vector2 Index_Tip;
    public Vector2 Middle_MCP;
    public Vector2 Middle_PIP;
    public Vector2 Middle_DIP;
    public Vector2 Middle_Tip;
    public Vector2 Ring_MCP;
    public Vector2 Ring_PIP;
    public Vector2 Ring_DIP;
    public Vector2 Ring_Tip;
    public Vector2 Pinky_MCP;
    public Vector2 Pinky_PIP;
    public Vector2 Pinky_DIP;
    public Vector2 Pinky_Tip;
  public long TimeStamp;
}
//_rightHand.btr2d position data
[System.Serializable]
public class PositionRightHand2D
{
    public Vector2 Wrist;
    public Vector2 Thumb_CMC;
    public Vector2 Thumb_MCP;
    public Vector2 Thumb_IP;
    public Vector2 Thumb_Tip;
    public Vector2 Index_MCP;
    public Vector2 Index_PIP;
    public Vector2 Index_DIP;
    public Vector2 Index_Tip;
    public Vector2 Middle_MCP;
    public Vector2 Middle_PIP;
    public Vector2 Middle_DIP;
    public Vector2 Middle_Tip;
    public Vector2 Ring_MCP;
    public Vector2 Ring_PIP;
    public Vector2 Ring_DIP;
    public Vector2 Ring_Tip;
    public Vector2 Pinky_MCP;
    public Vector2 Pinky_PIP;
    public Vector2 Pinky_DIP;
    public Vector2 Pinky_Tip;
  public long TimeStamp;
}
// .bmr preasure value of the each sensor
[System.Serializable]
public class BalanceMatData
{
  public ushort leftSensor0;
  public ushort leftSensor1;
  public ushort leftSensor2;
  public ushort leftSensor3;
  public ushort leftSensor4;
  public ushort leftSensor5;
  public ushort leftSensor6;
  public ushort leftSensor7;
  public ushort leftSensor8;
  public ushort leftSensor9;
  public ushort leftSensor10;
  public ushort leftSensor11;
  public ushort leftSensor12;

  public ushort rightSensor0;
  public ushort rightSensor1;
  public ushort rightSensor2;
  public ushort rightSensor3;
  public ushort rightSensor4;
  public ushort rightSensor5;
  public ushort rightSensor6;
  public ushort rightSensor7;
  public ushort rightSensor8;
  public ushort rightSensor9;
  public ushort rightSensor10;
  public ushort rightSensor11;
  public ushort rightSensor12;

  public long TimeStamp; // Add TimeStamp

  public List<ushort> GetLeftSensorData()
  {
    return new List<ushort>
        {
            leftSensor0, leftSensor1, leftSensor2, leftSensor3, leftSensor4,
            leftSensor5, leftSensor6, leftSensor7, leftSensor8, leftSensor9,
            leftSensor10, leftSensor11, leftSensor12
        };
  }

  public List<ushort> GetRightSensorData()
  {
    return new List<ushort>
        {
            rightSensor0, rightSensor1, rightSensor2, rightSensor3, rightSensor4,
            rightSensor5, rightSensor6, rightSensor7, rightSensor8, rightSensor9,
            rightSensor10, rightSensor11, rightSensor12
        };
  }
}

