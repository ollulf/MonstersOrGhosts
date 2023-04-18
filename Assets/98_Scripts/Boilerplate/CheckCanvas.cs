using Photon.Pun;

public class CheckCanvas : MonoBehaviourPun
{

    private void Start()
    {
        if(!photonView.IsMine)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
    }

}
