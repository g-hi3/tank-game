using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace TankGame.Core.Save
{
    [Serializable]
    public class GameSaveData
    {
        private static readonly string SaveDataPath = Path.Combine(Application.persistentDataPath, "data.json");

        public GameSaveData() : this(Array.Empty<Attempt>())
        {
        }

        public GameSaveData(IEnumerable<Attempt> attempts)
        {
            Attempts = attempts?.ToArray() ?? Array.Empty<Attempt>();
        }
        
        public Attempt[] Attempts { get; set; }

        public static void Save(GameSaveData data)
        {
            var fileContent = JsonUtility.ToJson(data);
            File.WriteAllText(SaveDataPath, fileContent);
        }

        public static GameSaveData Load()
        {
            if (!File.Exists(SaveDataPath))
                return new();

            var fileContent = File.ReadAllText(SaveDataPath);
            return JsonUtility.FromJson<GameSaveData>(fileContent) ?? new();
        }
    }
}