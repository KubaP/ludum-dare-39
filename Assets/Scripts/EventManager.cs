using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour {


    [Header("Panel")]
    public GameObject EventPanel;
    public Text eventName;
    public Text eventDescription;
    [Space]

    private float timeToNextEvent = 45f;

    public List<Event> eventList = new List<Event>();

    private void Update()
    {
        if (!GameObject.FindGameObjectWithTag("GameController").GetComponent<BuildingManager>().inMinigame && !GameObject.FindGameObjectWithTag("GameController").GetComponent<BuildingManager>().gameOver) {
            if (timeToNextEvent <= 0) {
                CauseEvent();
                timeToNextEvent = 30f + Random.Range(0f, 30f);
            }
            timeToNextEvent -= Time.deltaTime;
        }
    }

    public void CauseEvent()
    {
        if (eventList.Count > 0) {
            int chosenEvent = Random.Range(0, eventList.Count);
            eventName.text = eventList[chosenEvent].eventName;
            eventDescription.text = eventList[chosenEvent].description;
            StartCoroutine(OpenEventBox());
            eventList[chosenEvent].DoOperation();
        }
    }

    void CloseEventBox()
    {
        EventPanel.GetComponent<Animator>().Play("eventPanelClose");
    }

    IEnumerator OpenEventBox()
    {
        EventPanel.GetComponent<Animator>().Play("eventPanelOpen");
        yield return new WaitForSeconds(5);
        CloseEventBox();
    }

}
