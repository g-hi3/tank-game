using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Serialization.Json;
using UnityEngine;

namespace TankGame.Core.Save
{
    /// <summary>
    /// Represents the serializable game state, specifically the recorded game times.
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        private static readonly string SaveDataPath = Path.Combine(Application.persistentDataPath, "data.json");

        /// <summary>
        /// Contains the attempts at the game.
        /// </summary>
        public Attempt[] attempts;

        /// <summary>
        /// Creates a new instance without any attempts.
        /// </summary>
        /// <remarks>
        /// This constructor is necessary for Unity's serialization. Always use
        /// <see cref="GameSaveData(IEnumerable{Attempt})"/> instead!
        /// </remarks>
        public GameSaveData() : this(Array.Empty<Attempt>())
        {
        }

        /// <summary>
        /// Creates a new instance with the given <paramref name="attempts"/>.
        /// </summary>
        public GameSaveData(IEnumerable<Attempt> attempts)
        {
            this.attempts = attempts?.ToArray() ?? Array.Empty<Attempt>();
        }

        /// <summary>
        /// Saves the game to the default save file location.
        /// </summary>
        /// <param name="data">data to save</param>
        public static void Save(GameSaveData data)
        {
            var fileContent = JsonSerialization.ToJson(data, new JsonSerializationParameters
            {
                DisableSerializedReferences = true
            });
            File.WriteAllText(SaveDataPath, fileContent);
        }

        /// <summary>
        /// Loads save data from the default location.
        /// </summary>
        /// <returns>the data from the save, or an empty game save, if there was no save</returns>
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