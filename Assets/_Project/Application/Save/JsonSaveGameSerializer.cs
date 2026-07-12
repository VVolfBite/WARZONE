using System;
using System.Text.Json;

namespace Warzone.Application.Save
{
    public sealed class JsonSaveGameSerializer : ISaveGameSerializer
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNameCaseInsensitive = true
        };

        public JsonSaveGameSerializer()
        {
        }

        public string Serialize(SaveGameSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return null;
            }

            return JsonSerializer.Serialize(snapshot, SerializerOptions);
        }

        public bool TryDeserialize(string data, out SaveGameSnapshot snapshot, out string reason)
        {
            snapshot = null;
            reason = null;

            if (string.IsNullOrWhiteSpace(data))
            {
                reason = "Save data is empty.";
                return false;
            }

            try
            {
                snapshot = JsonSerializer.Deserialize<SaveGameSnapshot>(data, SerializerOptions);
                if (snapshot == null)
                {
                    reason = "Save data could not be parsed.";
                    return false;
                }

                if (snapshot.Metadata == null)
                {
                    snapshot.Metadata = new SaveGameMetadata();
                }

                if (snapshot.Campaign == null)
                {
                    snapshot.Campaign = new CampaignSaveData();
                }

                return true;
            }
            catch (Exception exception)
            {
                reason = exception.Message;
                return false;
            }
        }
    }
}
