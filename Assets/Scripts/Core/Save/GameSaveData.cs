using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Serialization.Json;
using UnityEngine;

namespace TankGame.Core.Save
{
    [Serializable]
    public class GameSaveData
    {
        private static readonly string SaveDataPath = Path.Combine(Application.persistentDataPath, "data.json");
        public Attempt[] Attempts;

        public GameSaveData() : this(Array.Empty<Attempt>())
        {
        }

        public GameSaveData(IEnumerable<Attempt> attempts)
        {
            Attempts = attempts?.ToArray() ?? Array.Empty<Attempt>();
        }
        
        public static void Save(GameSaveData data)
        {
            var fileContent = JsonSerialization.ToJson(data, new JsonSerializationParameters
            {
                DisableSerializedReferences = true
            });
            File.WriteAllText(SaveDataPath, fileContent);
        }

        public static GameSaveData Load()
        {
            if (!File.Exists(SaveDataPath))
                return new();

            var fileContent = File.ReadAllText(SaveDataPath);
            return JsonSerialization.FromJson<GameSaveData>(fileContent, new JsonSerializationParameters
            {
                DisableSerializedReferences = true
            }) ?? new();
        }
    }
}