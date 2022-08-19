using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OilCrater : MonoBehaviourPun
{
    public void BuildOilRig()
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine && RefineryHandler.Money >= RefineryHandler.RefineryCost)
        {
            base.photonView.RequestOwnership();
            PhotonNetwork.Instantiate("MachineGame/seaRefinery", new Vector3(transform.parent.root.position.x , 13.6f, transform.parent.root.position.z), Quaternion.identity);
            RefineryHandler.SetMoney();
            PhotonNetwork.Destroy(photonView.gameObject);
        }
    }

    public void BuildEnvoirment()
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] == Charakter.Machine && RefineryHandler.Money >= RefineryHandler.RefineryCost)
        {
            base.photonView.RequestOwnership();
            PhotonNetwork.Instantiate("MachineGame/military-headquarters", new Vector3(transform.parent.root.position.x, 13.6f, transform.parent.root.position.z), Quaternion.identity);
            RefineryHandler.SetMoney();
            PhotonNetwork.Destroy(photonView.gameObject);
        }
    }
}
