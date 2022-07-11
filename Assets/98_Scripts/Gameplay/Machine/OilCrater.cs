using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OilCrater : MonoBehaviourPun
{
    private void OnMouseDown()
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine && RefineryHandler.Money >= RefineryHandler.RefineryCost)
        {
            base.photonView.RequestOwnership();
            PhotonNetwork.Instantiate("MachineGame/seaRefinery", new Vector3(transform.position.x, 13.6f, transform.position.z), Quaternion.identity);
            RefineryHandler.SetMoney();
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
