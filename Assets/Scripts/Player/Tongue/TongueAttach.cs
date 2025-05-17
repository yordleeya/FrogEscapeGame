using UnityEngine;

public class TongueAttach : MonoBehaviour
{
    [SerializeField]
    RopeAction rope;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            switch(collision.tag)
            {
                case "Platform":
                    rope.ConnectSpringJoint();
                    rope.IsAttached = true;
<<<<<<< HEAD

                    transform.parent = collision.transform;

=======
>>>>>>> Won_Branch
                    rope.ResetTongue();
                    break;
                case "Ground":
                    rope.ResetRopeState();
                    break;
                case "Object":
                    break;
            
                
            }
        }
    }
<<<<<<< HEAD

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(transform.parent != null)
        {
            transform.parent = null;
        }
    }



=======
>>>>>>> Won_Branch
}
