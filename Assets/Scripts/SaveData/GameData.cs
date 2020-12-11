using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Text;
using System.Xml.Serialization;

public class GameData
{
    [Serializable]
    private class SerializableGameData
    {
        public SerializableHashMap<string, int> levelData = new SerializableHashMap<string, int>();
    }

    private static SerializableGameData gameData = new SerializableGameData();
    private const string SAVE_FOLDER = "saves";
    private const string SAVE_FILE = "data.sav";

    static GameData()
    {
        load();
    }

    private static FileStream openFile(FileMode mode)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, SAVE_FOLDER);
        if (!Directory.Exists (folderPath)) Directory.CreateDirectory (folderPath);
        string filePath = Path.Combine(folderPath, SAVE_FILE);
        try {
            return File.Open(filePath, mode);
        }
        catch (Exception e) {
            return null;
        }
    }

    public static void save(string level, int score)
    {
        int prevScore;
        bool found = gameData.levelData.TryGetValue(level, out prevScore);
        if (score>prevScore || !found) {
            gameData.levelData.Add(level, score);
            save();
        }
    }

    public static void unlockLevel(string level) {
        save(level, 0);
    }

    public static bool isLevelUnlocked(string level) {
        return gameData.levelData.ContainsKey(level);
    }

    public static int getLevelScore(string level) {
        int value;
        if (gameData.levelData.TryGetValue(level, out value)) return value;
        return 0;
    }

    public static int getTotalScore() {
        int totalScore = 0;
        foreach (int score in gameData.levelData.Values) totalScore+=score;
        return totalScore;
    }

    private static void save()
    {
        using (FileStream file = openFile(FileMode.OpenOrCreate))
            new BinaryFormatter().Serialize (file, gameData);
    }

    private static void load()
    {
        using (FileStream file = openFile(FileMode.Open))
            gameData = file!=null ? (SerializableGameData)new BinaryFormatter().Deserialize (file) : gameData;
    }
}
