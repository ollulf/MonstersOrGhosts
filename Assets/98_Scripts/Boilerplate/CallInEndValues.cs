using UnityEngine;
using Photon.Pun;


public class CallInEndValues : Singleton<CallInEndValues>
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private GameObject endScreen, enviButtons;
    [SerializeField] private float timeToWaitForLoad;

    private FishMovment fish;
    private BirdController bird;
    private DeerController deer;
    private BacteriumMovement bacterium;
    private Timer timer;

    private float gameTotalTempretureIncrease, totalCarbonDioxide, moneyGeneratedTotal;

    private int totalAmountOfCollectedAcetate;

    private bool startEnd = false;
    private bool loadLevel = false;

    public static void SetFish(FishMovment newFish) => Instance.photonView.RPC("SetAllFish", RpcTarget.MasterClient, newFish);

    public static void SetBird(BirdController newBird) => Instance.photonView.RPC("SetAllBirds", RpcTarget.MasterClient, newBird);

    public static void SetDeer(DeerController newDeer) => Instance.photonView.RPC("SetAllDeer", RpcTarget.MasterClient, newDeer);

    public static void SetBacteria(BacteriumMovement newBacteria) => Instance.photonView.RPC("SetAllBacteria", RpcTarget.MasterClient, newBacteria);

    public static void SetValues()
    {
        if (!Instance.startEnd)
        {
            Instance.startEnd = true;

            PopulationPair fishValue;
            PopulationPair birdValue;
            PopulationPair deerValue;

            Instance.timer = new Timer(Instance.timeToWaitForLoad, true);

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

            if (Instance.photonView.Owner.IsMasterClient)
            {
                int tempBirdValueStart, tempBirdValueEnd, tempFishValueStart, tempFishValueEnd, tempDeerValueStart, tempDeerValueEnd;

                if (Instance.fish != null)
                {
                    tempFishValueStart = FirstDataGive.FishStartPopulation;
                    tempFishValueEnd = Instance.fish.Population;
                }
                else
                {
                    tempFishValueStart = FirstDataGive.FishStartPopulation;
                    tempFishValueEnd = FirstDataGive.FishStartPopulation;
                }
                if (Instance.bird != null)
                {
                    tempBirdValueStart = FirstDataGive.BirdStartPopulation;
                    tempBirdValueEnd = Instance.bird.BirdPopulation;
                }
                else
                {
                    tempBirdValueStart = FirstDataGive.BirdStartPopulation;
                    tempBirdValueEnd = FirstDataGive.BirdStartPopulation;
                }
                if (Instance.deer != null)
                {
                    tempDeerValueStart = FirstDataGive.DeerStartPopulation;
                    tempDeerValueEnd = Instance.deer.population;
                }
                else
                {
                    tempDeerValueStart = FirstDataGive.DeerStartPopulation;
                    tempDeerValueEnd = FirstDataGive.DeerStartPopulation;
                }

                Instance.photonView.RPC("ShowEnd", RpcTarget.All, Instance.gameTotalTempretureIncrease, Instance.totalCarbonDioxide, Instance.moneyGeneratedTotal, Instance.totalAmountOfCollectedAcetate, tempBirdValueStart, tempBirdValueEnd, tempFishValueStart, tempFishValueEnd, tempDeerValueStart, tempDeerValueEnd);

            }
        }
    }

    [PunRPC]
    private void SetAllFish(FishMovment newFish) => fish = newFish;

    [PunRPC]
    private void SetAllBirds(BirdController newBird) => bird = newBird;

    [PunRPC]
    private void SetAllDeer(DeerController newDeer) => deer = newDeer;

    [PunRPC]
    private void SetAllBacteria(BacteriumMovement newBacteria) => bacterium = newBacteria;

    [PunRPC]
    private void ShowEnd(float TotalTempIncrease, float TotalCarbDiox, float totalMoney, int totalCollectedAce, int newtempBirdValueStart, int newtempBirdValueEnd, int newtempFishValueStart, int newtempFishValueEnd, int newtempDeerValueStart, int newtempDeerValueEnd)
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] != Charakter.Observer)
        {
            PopulationPair fishValue;
            PopulationPair birdValue;
            PopulationPair deerValue;

            fishValue = new PopulationPair(newtempFishValueStart, newtempFishValueEnd);
            birdValue = new PopulationPair(newtempBirdValueStart, newtempBirdValueEnd);
            deerValue = new PopulationPair(newtempDeerValueStart, newtempDeerValueEnd);

            EndGameValues endGameValues = new EndGameValues(TotalTempIncrease, TotalCarbDiox, totalMoney, totalCollectedAce, birdValue, fishValue, deerValue);
            Instance.endScreen.SetActive(true);
            Instance.enviButtons.SetActive(false);
            Instance.endScreen.GetComponent<EndScreen>().UpdateGameValues(endGameValues);
        }
    }

    private void FixedUpdate()
    {
        if (timer != null)
        {
            if (photonView.Owner.IsMasterClient)
            {
                timer.Tick();
                if (timer.CurrentTime <= 0 && !loadLevel)
                {
                    loadLevel = true;
                    PhotonNetwork.LoadLevel(1);
                }
            }
        }
    }
}