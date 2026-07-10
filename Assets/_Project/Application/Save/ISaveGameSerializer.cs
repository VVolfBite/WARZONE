namespace Warzone.Application.Save
{
    public interface ISaveGameSerializer
    {
        string Serialize(SaveGameSnapshot snapshot);

        bool TryDeserialize(string data, out SaveGameSnapshot snapshot, out string reason);
    }
}
