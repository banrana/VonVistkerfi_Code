﻿using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal linkedPortal;
    List<PortalTraveller> trackedTravellers;

    void Awake()
    {
        trackedTravellers = new List<PortalTraveller>();
    }

    void LateUpdate()
    {
        HandleTravellers();
    }

    void HandleTravellers()
    {
        for (int i = 0; i < trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;
            var m = linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - transform.position;
            int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
            int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));

            if (portalSide != portalSideOld)
            {
                var positionOld = travellerT.position;
                var rotOld = travellerT.rotation;
                traveller.Teleport(transform, linkedPortal.transform, m.GetColumn(3), m.rotation);
                traveller.graphicsClone.transform.SetPositionAndRotation(positionOld, rotOld);
                linkedPortal.OnTravellerEnterPortal(traveller);
                trackedTravellers.RemoveAt(i);
                i--;
            }
            else
            {
                traveller.graphicsClone.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
                traveller.previousOffsetFromPortal = offsetFromPortal;
            }
        }
    }

    void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            traveller.EnterPortalThreshold();
            traveller.previousOffsetFromPortal = traveller.transform.position - transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller)
        {
            OnTravellerEnterPortal(traveller);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);
        }
    }
}
