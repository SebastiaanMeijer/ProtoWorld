using UnityEngine;
using System.Collections;
using System.Linq;

public class TicketBarrierControl : MonoBehaviour
{
    private SEStarSmartObject smartObject;
    private SEStar SEStarControl;
    private Transform blockObject;
    private Vector3 initialScale;
    private bool isOpen;

    void Start()
    {
        Debug.Log("Exit barrier created");

        smartObject = this.GetComponent<SEStarSmartObject>();
        SEStarControl = GameObject.FindObjectOfType<SEStar>();

        var blockObject = transform.FindChild("Block");

        if (blockObject == null)
            Debug.LogError("The block object of the Ticketbarriers cannot be null");
        else
            ChangeBarrier(true); // Open by default
    }

    public void ChangeBarrier(bool open)
    {
        if (blockObject == null)
        {
            blockObject = transform.FindChild("Block");
            initialScale = blockObject.localScale;
        }

        if (open)
        {
            blockObject.gameObject.SetActive(false); // blockObject.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            isOpen = true;
        }
        else
        {
            blockObject.gameObject.SetActive(true); // blockObject.localScale = initialScale;
            isOpen = false;
        }
    }

    void OnMouseOver()
    {
        // Left button changes its state
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Exit barrier " + smartObject.objectId + " changing state");

            if (isOpen)
                SEStarControl.ChangeSEStarObjectState(smartObject.objectId, "Blocked");
            else
                SEStarControl.ChangeSEStarObjectState(smartObject.objectId, "Normal");

            ChangeBarrier(!isOpen);

        }
    }
}