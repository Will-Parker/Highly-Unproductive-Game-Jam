using UnityEngine;
public class Entity : MonoBehaviour, IEntity
{
    //protected Animator anim;
    protected float maxHealth;
    protected float health;
    /*public void UpdateAnim(bool isMoving, Vector2 moveDir)
    {
        Debug.Log("Update Anim");
        if (isMoving)
        {
            anim.SetFloat("Horizontal", moveDir.x);
            anim.SetFloat("Vertical", moveDir.y);
        }
        anim.SetBool("isMoving", isMoving);
    }

    public void UpdateAnim(bool isMoving)
    {
        Debug.Log("Update Anim");
        anim.SetBool("isMoving", isMoving);
    }*/

    public void TakeDamage(float damage) 
    {
        throw new System.NotImplementedException("TakeDamage method Not filled out in subclass");
    }
}
