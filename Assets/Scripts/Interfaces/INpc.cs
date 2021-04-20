using ScriptableObjects;

namespace Interfaces
{
    public interface INpc
    {
        void GiveQuest(Quest quest);
        void Talk();
    }
}