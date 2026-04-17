using System;
using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;
    public Transform openState;
    public Transform closedState;
    public enum DoorState { Open, Opening, Closing, Closed }
    [Header("Debug")]
    public DoorState doorState = DoorState.Closed;
    public float doorTime = 0f;
    public float doorDuration = 1f;

    private Coroutine openDoorCoroutine;
    private Coroutine closeDoorCoroutine;

    
    public void OpenDoor()
    {
        if (doorState == DoorState.Open || doorState == DoorState.Opening)
        {
            return;
        }

        if (doorState == DoorState.Closing)
        {
            StopCoroutine(closeDoorCoroutine);
        }

        doorState = DoorState.Opening;

        openDoorCoroutine = StartCoroutine(OpenDoorCoroutine());
    }

    private IEnumerator OpenDoorCoroutine()
    {
        while (doorTime < doorDuration)
        {
            doorTime += Time.deltaTime;
            var factor = AnimationHelper.EaseInOutCirc(doorTime / doorDuration);
            door.SetPositionAndRotation(
                Vector3.Lerp(closedState.position, openState.position, factor),
                Quaternion.Slerp(closedState.rotation, openState.rotation, factor)
            );
            yield return null;
        }

        doorTime = doorDuration;
        doorState = DoorState.Open;
    }

    public void CloseDoor()
    {
        if (doorState == DoorState.Closed || doorState == DoorState.Closing)
        {
            return;
        }

        if (doorState == DoorState.Opening)
        {
            StopCoroutine(openDoorCoroutine);
        }

        doorState = DoorState.Closing;

        closeDoorCoroutine = StartCoroutine(CloseDoorCoroutine());
    }

    private IEnumerator CloseDoorCoroutine()
    {
        while (doorTime > 0f)
        {
            doorTime -= Time.deltaTime;
            var factor = AnimationHelper.EaseInOutCirc(doorTime / doorDuration);
            door.SetPositionAndRotation(
                Vector3.Lerp(closedState.position, openState.position, factor),
                Quaternion.Slerp(closedState.rotation, openState.rotation, factor)
            );
            yield return null;
        }
        doorTime = 0f;
        doorState = DoorState.Closed;
    }

    public void ToggleDoor()
    {
        if (doorState == DoorState.Open)
        {
            CloseDoor();
        }
        else if (doorState == DoorState.Closed)
        {
            OpenDoor();
        }
    }
}
