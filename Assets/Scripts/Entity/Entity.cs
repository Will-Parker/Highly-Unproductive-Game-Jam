using UnityEngine;
public class Entity : MonoBehaviour
{
    protected Animator anim;
    [SerializeField] protected Healthbar healthbar = null;
    [SerializeField] protected float MaxHealth;
    public float Health { get; protected set; }
    [SerializeField] protected float Attack;
    public Vector2 facingDirection;
    public void UpdateAnim(bool isMoving, Vector2 moveDir)
    {
        anim.SetBool("isMoving", isMoving);
        anim.SetFloat("Horizontal", moveDir.x);
        anim.SetFloat("Vertical", moveDir.y);
        facingDirection = moveDir;
    }

    public void UpdateAnim(bool isMoving)
    {
        anim.SetBool("isMoving", isMoving);
    }
    public void UpdateAnim(Vector2 faceDir)
    {
        anim.SetFloat("Horizontal", faceDir.x);
        anim.SetFloat("Vertical", faceDir.y);
        facingDirection = faceDir;
    }

    public void TakeDamage(float damage) 
    {
        throw new System.NotImplementedException("TakeDamage method Not filled out in subclass");
    }
}
