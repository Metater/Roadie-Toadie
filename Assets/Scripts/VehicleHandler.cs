using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleHandler : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sprite;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Mathf.Abs(rb.transform.position.x) > GameManager.TilemapWidth) Destroy(gameObject);
    }

    public void SetVelocityX(float speed)
    {
        rb.velocity = new Vector2(speed, 0);
    }
    public void SetRotationZ(float angle)
    {
        sprite.gameObject.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}
