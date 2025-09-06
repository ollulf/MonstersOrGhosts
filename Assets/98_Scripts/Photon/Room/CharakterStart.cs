using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharakterStart : MonoBehaviourPunCallbacks, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Charakter _charakter;
    [SerializeField] private Color _color, _clicked;

    private Image _image;
    private int index;
    private Color _originalBackground;
    private ExitGames.Client.Photon.Hashtable _myCharakter = new ExitGames.Client.Photon.Hashtable();

    public void OnPointerClick(PointerEventData eventData)
    {
        _image.color = _clicked;
        _myCharakter["PlayerCharakter"] = _charakter;
        PhotonNetwork.LoadLevel(1);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _myCharakter["PlayerCharakter"] = _charakter;
        _image.color = _color;
        SetProperties();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = _originalBackground;
    }

    void Start()
    {
        _image = GetComponent<Image>();
        _originalBackground = _image.color;
    }

    public void SetProperties()
    {
        _myCharakter["PlayerCharakter"] = _charakter;
        PhotonNetwork.SetPlayerCustomProperties(_myCharakter);

        Debug.LogWarning(PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"]);

    }

}
