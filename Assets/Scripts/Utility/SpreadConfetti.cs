using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadConfetti : MonoBehaviour
{
    [Header("Use Object")]
    [SerializeField] private GameObject confettiContainer;
    [SerializeField] private GameObject confettiObject;

    [HideInInspector] private List<GameObject> cloneConfettiObjects;

    [Header("System Config")]
    [SerializeField] private GameObject spreadOriginalPos;
    [SerializeField] private Vector3 spreadCustomPos = Vector3.zero;
    [SerializeField] private float spreadRange = 4f;
    [SerializeField] private float spreadForce = 64f;
    [SerializeField] private int spreadNum = 100;
    [SerializeField] private float voidPosY = 0f;

    // Unity

    void Awake()
    {
        cloneConfettiObjects = new List<GameObject>();
    }

    void Update()
    {
        cloneConfettiObjects = RemoveConfettiObject(cloneConfettiObjects);
    }

    // Custom Function

    public void StartSpread()
    {
        for (int i = 0; i < spreadNum; i++)
        {
            GameObject cloneConfettiObject = UniversalFunction.SetCloneObject(confettiObject, confettiContainer);

            cloneConfettiObjects.Add(cloneConfettiObject);

            cloneConfettiObject.transform.position =
            (
                spreadOriginalPos != null ?
                spreadOriginalPos.transform.position + spreadCustomPos + UniversalFunction.GenerateRandomRange(spreadRange * -1f, spreadRange) :
                spreadCustomPos + UniversalFunction.GenerateRandomRange(spreadRange * -1f, spreadRange)
            );
            cloneConfettiObject.transform.rotation = Quaternion.Euler(UniversalFunction.GenerateRandomRange(0f, 360f));
            cloneConfettiObject.GetComponent<Renderer>().material.color = UniversalFunction.GetColor(-1f, 0.5f, 1f, -1f);

            Rigidbody cloneConfettiRigidbody = cloneConfettiObject.GetComponent<Rigidbody>();
            cloneConfettiRigidbody.useGravity = true;
            cloneConfettiRigidbody.AddForce(UniversalFunction.GenerateRandomRange(spreadForce * -1f, spreadForce));
            cloneConfettiRigidbody.AddRelativeTorque(UniversalFunction.GenerateRandomRange(spreadForce * -1f * 100f, spreadForce * 100f));
        }
    }

    public void StopSpread()
    {
        foreach (GameObject cloneConfettiObject in cloneConfettiObjects) GameObject.Destroy(cloneConfettiObject);

        cloneConfettiObjects = new List<GameObject>();
    }

    // Specific Function

    List<GameObject> RemoveConfettiObject(List<GameObject> gos)
    {
        List<GameObject> newList = new List<GameObject>();
        List<GameObject> destroyList = new List<GameObject>();

        foreach (GameObject go in gos)
        {
            if (go.transform.position.y > voidPosY)
            {
                newList.Add(go);
            }
            else
            {
                destroyList.Add(go);
            }
        }

        foreach (GameObject go in destroyList) GameObject.Destroy(go);

        return newList;
    }
}
