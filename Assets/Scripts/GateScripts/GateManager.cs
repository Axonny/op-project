using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GateScripts
{
    public class GateManager : MonoBehaviour
    {
        public List<ColorBunch> hasKeys;
    
        public void TryOpenGate(Gate gate)
        {
            if (hasKeys.Any(key => key == gate.color))
            {
                gate.Open();
            }
        }

        public void GetKey(Key key)
        {
            hasKeys.Add(key.color);
        }
    }
}