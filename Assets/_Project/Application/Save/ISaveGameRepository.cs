namespace Warzone.Application.Save
{
    public interface ISaveGameRepository
    {
        bool Save(string slotId, string data, out string reason);

        bool TryLoad(string slotId, out string data, out string reason);
    }
}
