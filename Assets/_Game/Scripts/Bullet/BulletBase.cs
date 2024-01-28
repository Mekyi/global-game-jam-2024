using UnityEngine;

public class BulletBase : MonoBehaviour
{
    [SerializeField] private int dmg;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float range;
    [SerializeField] private string targetTag;
    private Vector3 velocity;
    private Rigidbody2D rBody;
    private Vector3 startPos;
    
    private void Awake()
    {
        rBody = GetComponent<Rigidbody2D>();
    }
    public virtual void Shoot(Vector3 direction, Vector3 pos)
    {
        velocity = Random.Range(minSpeed, maxSpeed) * direction;
        transform.position = startPos = pos;
    }
    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, startPos) > range)
        {
            Destroy(gameObject);
            return;
        }
        rBody.velocity = velocity;
    }

    private void CheckHit(Collider2D other)
    {
        Debug.Log("hit " + other.name + ", tag: " + other.tag);
        if (!other.CompareTag(targetTag))
            return;
        
        Debug.Log("Target is right");
        var charBase = other.GetComponent<CharacterBase>();
        if (charBase == null)
            return;
        Debug.Log("Got characterbase");
        if(charBase.DealDamage(dmg))
            Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckHit(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        CheckHit(other);
    }
}
