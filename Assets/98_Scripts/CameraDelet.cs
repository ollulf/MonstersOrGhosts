using Photon.Pun;

public class CameraDelet : MonoBehaviourPun
{
    void Start()
    {
        if(!photonView.IsMine)
        {
            Destroy(gameObject);
        }
    }
}