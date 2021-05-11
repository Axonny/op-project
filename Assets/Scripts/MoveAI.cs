using UnityEngine;


public class MoveAI : MonoBehaviour
{
    public float distanceRaycast = 5f;
    public float speed = 3f;

    private new Rigidbody2D rigidbody;
    private bool isDetectedPlayer;
    [SerializeField] private LayerMask layerMask;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
    }
    
    public void UpdateAI(GameField[,] map, Player player)
    {
        if (isDetectedPlayer || TryDetectPlayer(player, out isDetectedPlayer))
            MoveToPlayer(map, player);
    }
    
    private void MoveBFSOnTilemapToPoint(GameField[,] map, Vector2 point)
    {
        //at some time in the future....
    }

    private void MoveToPlayer(GameField[,] map, Player player)
    {
        if (TryThrowRayCast(transform, player.transform, distanceRaycast))
        {
            rigidbody.velocity = (player.transform.position - transform.position).normalized * speed;
        }
        else
        {
            MoveBFSOnTilemapToPoint(map, player.transform.position);
        }
    }

    private bool TryDetectPlayer(Player player, out bool result)
    {
        result = TryThrowRayCast(transform, player.transform, distanceRaycast);
        return result;
    }

    private bool TryThrowRayCast(Transform from, Transform to, float distance)
    {
        var position = from.position;
        var hit = Physics2D.Raycast(position, to.position - position, distance, layerMask);
        return hit.transform == to;
    } 
}
