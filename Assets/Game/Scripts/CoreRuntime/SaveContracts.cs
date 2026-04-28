using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.CoreRuntime
{
    [Serializable]
    public sealed class SaveEnvelope
    {
        public int Version;
        public List<SaveChunk> Chunks = new();
    }

    [Serializable]
    public sealed class SaveChunk
    {
        public string Id;
        public string JsonPayload;
    }

    public interface ISaveDataProvider
    {
        string SaveId { get; }
        object CaptureState();
        void RestoreState(string json);
    }

    public interface ISaveMigrationStep
    {
        int FromVersion { get; }
        int ToVersion { get; }
        SaveEnvelope Migrate(SaveEnvelope sourceEnvelope);
    }

    public sealed class SaveService
    {
        public const int CurrentVersion = 1;

        private readonly List<ISaveDataProvider> providers = new();
        private readonly List<ISaveMigrationStep> migrations = new();

        public void RegisterProvider(ISaveDataProvider provider)
        {
            if (provider != null && !providers.Contains(provider))
            {
                providers.Add(provider);
            }
        }

        public void RegisterMigration(ISaveMigrationStep migrationStep)
        {
            if (migrationStep != null)
            {
                migrations.Add(migrationStep);
            }
        }

        public string BuildSaveJson()
        {
            SaveEnvelope envelope = new SaveEnvelope { Version = CurrentVersion };
            for (int i = 0; i < providers.Count; i++)
            {
                ISaveDataProvider provider = providers[i];
                object state = provider.CaptureState();
                envelope.Chunks.Add(new SaveChunk
                {
                    Id = provider.SaveId,
                    JsonPayload = JsonUtility.ToJson(state)
                });
            }

            return JsonUtility.ToJson(envelope, true);
        }

        public void RestoreFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            SaveEnvelope envelope = JsonUtility.FromJson<SaveEnvelope>(json);
            if (envelope == null)
            {
                return;
            }

            envelope = ApplyMigrations(envelope);

            for (int i = 0; i < envelope.Chunks.Count; i++)
            {
                SaveChunk chunk = envelope.Chunks[i];
                for (int p = 0; p < providers.Count; p++)
                {
                    if (providers[p].SaveId == chunk.Id)
                    {
                        providers[p].RestoreState(chunk.JsonPayload);
                        break;
                    }
                }
            }
        }

        private SaveEnvelope ApplyMigrations(SaveEnvelope envelope)
        {
            int current = envelope.Version;
            while (current < CurrentVersion)
            {
                ISaveMigrationStep step = migrations.Find(m => m.FromVersion == current);
                if (step == null)
                {
                    break;
                }

                envelope = step.Migrate(envelope);
                current = envelope.Version;
            }

            return envelope;
        }
    }
}
