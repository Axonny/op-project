using UnityEngine;

namespace GateScripts
{
    public class Gate : MonoBehaviour
    {
        public TypeGate typeGate;
        public ColorBunch color;

        public void Open()
        {
            GameManager.Instance.RemoveGateFromTilemap(this);
        }
    }
}