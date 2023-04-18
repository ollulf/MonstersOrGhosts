using UnityEngine;
using Photon.Pun;

public class SimpelMoveScript : MonoBehaviourPun
{
    [SerializeField] private float movementSpeed;
    private Vector2 movement;

    void Update()
    {
        if (base.photonView.IsMine)
        {
            GetAxis();
        }
    }

    private void FixedUpdate()
    {
        if (base.photonView.IsMine)
        {
            MovePlayer();
        }
    }

    private void GetAxis()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }
    private void MovePlayer() => transform.position = new Vector3(transform.position.x + movement.x * movementSpeed * Time.fixedDeltaTime, transform.position.y, transform.position.z + movement.y * movementSpeed * Time.deltaTime);
}
