using UnityEngine;

public static class Physics
{
    public static Collider2D[] FindColliders(Vector3 position, float radius, LayerMask layers)
    {
        return Physics2D.OverlapCircleAll(position, radius, layers);
    }
    
    public static Collider2D FindCollider(Vector3 position, float radius, LayerMask layers)
    {
        return Physics2D.OverlapCircle(position, radius, layers);
    }

    public static float GetAngleToMouse(Camera mainCamera, Vector3 position)
    {
        var vector = mainCamera.ScreenToWorldPoint(InputSystem.Instance.Input.Mouse.Move.ReadValue<Vector2>()) - position;
        var angle = Mathf.Atan2(vector.y, vector.x);
        return angle * Mathf.Rad2Deg - 90f;
    }
}