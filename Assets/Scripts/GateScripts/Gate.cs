using System.Collections;
using PlayerScripts;
using UnityEngine;

namespace GateScripts
{
    public class Gate : MonoBehaviour
    {
        public SpriteRenderer gateSprite;
        public SpriteRenderer lockSprite;
        public TypeGate typeGate;
        public ColorBunch color;

        public void Open()
        {
            GameManager.Instance.RemoveGateFromTilemap(this);
            StartCoroutine(DeleteGate());
        }

        public void ReOrderLayer()
        {
            var playerY = Player.Instance.transform.position.y;
            var gateY = transform.position.y;
            gateSprite.sortingOrder = playerY > gateY ? 15 : -15;
            lockSprite.sortingOrder = playerY > gateY ? 16 : -14;
        }

        private IEnumerator DeleteGate()
        {
            var gateColorStart = gateSprite.color;
            var lockColorStart = lockSprite.color;
            var gateColorEnd = gateColorStart;
            var lockColorEnd = lockColorStart;
            gateColorEnd.a = 0;
            lockColorEnd.a = 0;

            var t = 0f;
            while (t < 1)
            {
                gateSprite.color = Color.Lerp(gateColorStart, gateColorEnd, t);
                lockSprite.color = Color.Lerp(lockColorStart, lockColorEnd, t);
                t += Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}