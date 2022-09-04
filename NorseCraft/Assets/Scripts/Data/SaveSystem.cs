using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    /*
     * Saves the game state to the file specified by the given file index (1, 2, or 3)
     */
    public static void SaveGameState(SaveData saveData, int fileIndex)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/game.dat"; ;
        switch (fileIndex)
        {
            case 1:
                path = Application.persistentDataPath + "/gameOne.dat";
                break;

            case 2:
                path = Application.persistentDataPath + "/gameTwo.dat";
                break;

            case 3:
                path = Application.persistentDataPath + "/gameThree.dat";
                break;

            default:
                break;
        }
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, saveData);

        stream.Close();
    }

    /*
     * Loads the game file specified by the given file index (1, 2, or 3)
     */
    public static SaveData LoadGameState(int fileIndex)
    {
        string path = Application.persistentDataPath + "/game.dat"; ;
        switch (fileIndex)
        {
            case 1:
                path = Application.persistentDataPath + "/gameOne.dat";
                break;

            case 2:
                path = Application.persistentDataPath + "/gameTwo.dat";
                break;

            case 3:
                path = Application.persistentDataPath + "/gameThree.dat";
                break;

            default:
                break;
        }

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Save file not found in: " + path);
            return null;
        }
    }

    /*
     * Checks if there is a valid save file in the target directory (persistant data path)
     */
    public static bool FoundGameState(int fileIndex)
    {
        string path = Application.persistentDataPath + "/game.dat"; ;
        switch (fileIndex)
        {
            case 1:
                path = Application.persistentDataPath + "/gameOne.dat";
                break;

            case 2:
                path = Application.persistentDataPath + "/gameTwo.dat";
                break;

            case 3:
                path = Application.persistentDataPath + "/gameThree.dat";
                break;

            default:
                break;
        }
        return (File.Exists(path));
    }
	
}
