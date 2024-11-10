using UnityEngine;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class FileReadDropDown : MonoBehaviour
{
  public TMP_Dropdown fileDropdown; // Use TMP_Dropdown
  public DataReader dataReader; // Reference to the DataReader script
  private string folderPath = @"C:\PoiseVideos"; // Your folder path

  void Start()
  {
    PopulateDropdown();
    fileDropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(fileDropdown); });
  }

  void PopulateDropdown()
  {
    if (fileDropdown == null)
    {
      Debug.LogError("Dropdown not assigned.");
      return;
    }

    // Clear existing options
    fileDropdown.ClearOptions();

    // Get all directories in the specified path
    var directories = Directory.GetDirectories(folderPath).ToList();

    if (directories.Count == 0)
    {
      Debug.LogWarning("No directories found in the specified path.");
      return;
    }

    // Add options to the dropdown
    fileDropdown.AddOptions(directories.Select(dir => new TMP_Dropdown.OptionData(Path.GetFileName(dir))).ToList());
  }

  void DropdownItemSelected(TMP_Dropdown dropdown)
  {
    int index = dropdown.value;
    string selectedFolderName = dropdown.options[index].text;
    string selectedFolderPath = Path.Combine(folderPath, selectedFolderName);

    // Pass the selected folder path to the DataReader
    dataReader.selectedSessionFolderPath = selectedFolderPath;
    dataReader.selectedSessionFolderName = selectedFolderName;
    dataReader.LoadData();
  }
}
