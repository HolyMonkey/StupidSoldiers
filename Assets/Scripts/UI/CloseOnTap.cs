using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using Agava.YandexGames.Utility;

public class CloseOnTap : MonoBehaviour
{ 
    //[SerializeField] private GameObject _panelWithControlElements;
    //[SerializeField] private GameObject _tapToPlay;

    private bool mouseIsOver = false;

    private void OnEnable()
    {
        WebEventSystem.current.SetSelectedGameObject(gameObject);
    }

    private void Update()
    {
        PointerEventData pointer = new PointerEventData(WebEventSystem.current);

        if (Input.GetMouseButtonDown(0))
        {
            pointer.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            WebEventSystem.current.RaycastAll(pointer, raycastResults);

            if (raycastResults.Count == 0)
            {
                CloseSelf();
            }
        }
    }

    public void CloseSelf()
    {
        gameObject.SetActive(false);
        //_panelWithControlElements.SetActive(true);
        //_tapToPlay.SetActive(true);
    }

}