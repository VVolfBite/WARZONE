using System;
using System.Web.Script.Serialization;

namespace Warzone.Application.Save
{
    public sealed class JsonSaveGameSerializer : ISaveGameSerializer
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();

        public JsonSaveGameSerializer()
        {
            _serializer.MaxJsonLength = int.MaxValue;
        }

        public string Serialize(SaveGameSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return null;
            }

            return _serializer.Serialize(snapshot);
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
                snapshot = _serializer.Deserialize<SaveGameSnapshot>(data);
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
