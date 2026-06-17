using UnityEngine;

public class Pickup : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;


    public float throwForce = 500f; //force at which the object is thrown at
    public float pickUpRange = 5f; //how far the player can pickup the object from
    public KeyCode throwKey = KeyCode.G; // Key used to throw the object (G by default, to avoid conflict with R-rotation in building mode)

    private GameObject trash;
    private Rigidbody trashRb;
    private bool canDrop = true;
    private int LayerNumber;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) //pickup and drop object with E key
        {
            if (trash == null) 
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    if (hit.transform.gameObject.tag == "canPickUP")
                    {
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping(); //prevents object from clipping through walls
                    DropObject();
                }
            }
        }
        if (trash != null) //if player is holding object
        {
            HoldPosition(); //keep object position at holdPos
            if (Input.GetKeyDown(throwKey) && canDrop == true) //used to throw (configurable via inspector)
            {
                StopClipping();
                ThrowObject();
            }

        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            trash = pickUpObj; 
            trashRb = pickUpObj.GetComponent<Rigidbody>(); 
            trashRb.isKinematic = true;
            trashRb.transform.parent = holdPos.transform;
            trash.layer = LayerNumber;

            Physics.IgnoreCollision(trash.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }
    void DropObject()
    {
        //re-enable collision with player
        Physics.IgnoreCollision(trash.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        trash.layer = 0; //object assigned back to default layer
        trashRb.isKinematic = false;
        trash.transform.parent = null; //unparent object
        trash = null; //undefine game object
    }
    void HoldPosition()
    {
        trash.transform.position = holdPos.transform.position;
    }
    void ThrowObject()
    {
        //same as drop function, but add force to object before undefining it
        Physics.IgnoreCollision(trash.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        trash.layer = 0;
        trashRb.isKinematic = false;
        trash.transform.parent = null;
        trashRb.AddForce(transform.forward * throwForce);
        trash = null;
    }
    void StopClipping() //function only called when dropping/throwing
    {
        var clipRange = Vector3.Distance(trash.transform.position, transform.position); //distance from holdPos to the camera
        //have to use RaycastAll as object blocks raycast in center screen
        //RaycastAll returns array of all colliders hit within the cliprange
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //if the array length is greater than 1, meaning it has hit more than just the object we are carrying
        if (hits.Length > 1)
        {
            //change object position to camera position 
            trash.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //offset slightly downward to stop object dropping above player 
            //if your player is small, change the -0.5f to a smaller number (in magnitude) ie: -0.1f
        }
    }

}
