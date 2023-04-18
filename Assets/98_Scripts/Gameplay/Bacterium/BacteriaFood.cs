using UnityEngine;

public class BacteriaFood : MonoBehaviour
{
    void Start() => BacteriaFoodHandler.AddToList(gameObject);

    public void RemoveFromList() => BacteriaFoodHandler.RemoveFromList(gameObject);
}
