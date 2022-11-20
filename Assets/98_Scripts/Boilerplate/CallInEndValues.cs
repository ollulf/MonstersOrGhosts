using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class CallInEndValues : Singleton<CallInEndValues>
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private GameObject endScreen;

    private FishMovment fish;
    private BirdController bird;
    private DeerController deer;
    private BacteriumMovement bacterium;

    private float gameTotalTempretureIncrease, totalCarbonDioxide, moneyGeneratedTotal;

    private int totalAmountOfCollectedAcetate;



   public static void SetFish(FishMovment newFish)
   {
       Instance.photonView.RPC("SetAllFish", RpcTarget.MasterClient, newFish);
   }
   
   public static void SetBird(BirdController newBird)
   {
       Instance.photonView.RPC("SetAllBirds", RpcTarget.MasterClient, newBird);
   }
   
   public static void SetDeer(DeerController newDeer)
   {
       Instance.photonView.RPC("SetAllDeer", RpcTarget.MasterClient, newDeer);
   }
   
   public static void SetBacteria(BacteriumMovement newBacteria)
   {
       Instance.photonView.RPC("SetAllBacteria", RpcTarget.MasterClient, newBacteria);
   }


    public static void SetValues()
    {
        PopulationPair fishValue;
        PopulationPair birdValue;
        PopulationPair deerValue;

        Instance.gameTotalTempretureIncrease = TempretureHandler.Tempreture;
        Instance.totalCarbonDioxide = ShipHandler.CarbonProduced;
        Instance.moneyGeneratedTotal = ShipHandler.Money;

        Instance.totalAmountOfCollectedAcetate = Instance.bacterium != null 
            ? Instance.bacterium.AcetateCount 
            : 0;

        fishValue = Instance.fish != null
            ? new PopulationPair(FirstDataGive.FishStartPopulation, Instance.fish.Population)
            : new PopulationPair(FirstDataGive.FishStartPopulation, FirstDataGive.FishStartPopulation);

        birdValue = Instance.bird != null
            ? new PopulationPair(FirstDataGive.BirdStartPopulation, Instance.bird.BirdPopulation)
            : new PopulationPair(FirstDataGive.BirdStartPopulation, FirstDataGive.BirdStartPopulation);

        deerValue = Instance.deer != null
            ? new PopulationPair(FirstDataGive.DeerStartPopulation, Instance.deer.population)
            : new PopulationPair(FirstDataGive.DeerStartPopulation, FirstDataGive.DeerStartPopulation);

        EndGameValues endGameValues = new EndGameValues(Instance.gameTotalTempretureIncrease, Instance.totalCarbonDioxide, Instance.moneyGeneratedTotal, Instance.totalAmountOfCollectedAcetate, birdValue, fishValue, deerValue);

        Instance.photonView.RPC("ShowEnd", RpcTarget.All, endGameValues);

        Instance.ShowEnd(endGameValues);
    }

    [PunRPC]
    private void SetAllFish(FishMovment newFish)
    {
        fish = newFish;
    }

    [PunRPC]
    private void SetAllBirds(BirdController newBird)
    {
        bird = newBird;
    }

    [PunRPC]
    private void SetAllDeer(DeerController newDeer)
    {
        deer = newDeer;
    }

    [PunRPC]
    private void SetAllBacteria(BacteriumMovement newBacteria)
    {
        bacterium = newBacteria;
    }

    [PunRPC]
    private void ShowEnd(EndGameValues newValues)
    {
        Instance.endScreen.SetActive(true);
        Instance.endScreen.GetComponent<EndScreen>().UpdateGameValues(newValues);
    }
}