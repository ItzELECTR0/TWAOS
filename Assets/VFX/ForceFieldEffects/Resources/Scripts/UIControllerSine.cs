using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControllerSine : MonoBehaviour
{
    public Transform prefabHolder;
    public float openSpeed = 1f;
    public bool openAnimation = true;
    public AnimationCurve openCurve;

    private Transform[] prefabs;
    private List<Transform> lt;
    private int activeNumber = 0;
    private ForceFieldController ffc;
    private float openCloseValue;
    private float openCloseCurve;
    private GameObject activeGameObject;

    void Start()
    {

        lt = new List<Transform>();
        prefabs = prefabHolder.GetComponentsInChildren<Transform>(true);

        foreach (Transform tran in prefabs)
        {
            if (tran.parent == prefabHolder)
            {
                lt.Add(tran);
            }
        }

        prefabs = lt.ToArray();
        EnableActive();
    }

    // Turn On active VFX Prefab
    public void EnableActive()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            if (i == activeNumber)
            {
                prefabs[i].gameObject.SetActive(true);
                activeGameObject = prefabs[i].gameObject;
            }
            else
            {
                prefabs[i].gameObject.SetActive(false);
            }
        }
    }

    // Change active VFX
    public void ChangeEffect(bool bo)
    {
        activeGameObject.GetComponent<ForceFieldController>().SetOpenCloseValue(0f);
        
        if (bo == true)
        {
            activeNumber++;
            if (activeNumber == prefabs.Length)
            {
                activeNumber = 0;
            }
        }
        else
        {
            activeNumber--;
            if (activeNumber == -1)
            {
                activeNumber = prefabs.Length - 1;
            }
        }

        EnableActive();
    }

    // TEMP CONTROL
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeEffect(true);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeEffect(false);
        }
    }
}
