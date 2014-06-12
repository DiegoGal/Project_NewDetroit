using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flying : MonoBehaviour 
{
    private class FlyingUnit
    {
        public CStateUnit cs;
        public Rigidbody rigidbody = null;
        public NavMeshAgent navMeshAgent;
        public Transform transform;
        public float lastPosY = -1;
        public int contTrapped = 0;
        public bool goingDown = false;
    }

    // a list of characters that are flying
    private static List<FlyingUnit> ccList = new List<FlyingUnit>();
    private static bool emptyList = true;
    private static UnitController.State currentState = UnitController.State.Idle;
	
	// Update is called once per frame
	void Update ()
    {
        if (!emptyList)
        {
            foreach (FlyingUnit c in ccList)
            {
                CStateUnit cStateU = c.cs;
                Rigidbody cRigidbody = c.rigidbody;
                NavMeshAgent cNavMeshAgent = c.navMeshAgent;
                float cPosY = c.transform.position.y;
                float cLastPosY = c.lastPosY;
                int cContTrapped = c.contTrapped;
                bool cGoingDown = c.goingDown;
                float delta = 0.01f;

                if (cStateU.currentState != UnitController.State.Dying)
                {
                    int maxTrapped = 20;
                    if (!cGoingDown)
                    {
                        if (cRigidbody)
                        {
                            cRigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                            //cRigidbody.freezeRotation = true;
                        }
                        cContTrapped = 0;
                        if (cPosY > cLastPosY)
                        {
                            c.lastPosY = cPosY;
                        }
                        else
                            c.goingDown = true;
                    }
                    else if ( ( (cPosY + delta >= cLastPosY) && (cPosY - delta <= cLastPosY)
                        || (cLastPosY < 0.0f)) && cGoingDown)
                    {
                        if (cContTrapped == maxTrapped)
                        {
                            if (cNavMeshAgent && cNavMeshAgent.enabled)
                            {
                                cNavMeshAgent.Resume();
                                cNavMeshAgent.ResetPath();
                            }
                            if (cRigidbody)
                                Destroy(cRigidbody);
                            //currentState = UnitController.State.Idle;
                            //cStateU.currentState = currentState;
                            //if (cStateU.lastState == UnitController.State.GoingTo)
                            //    GoTo(cStateU.destiny);
                            //else
                            //{
                                currentState = cStateU.lastState;
                                cStateU.currentState = currentState;
                            //}
                            c.lastPosY = -1.0f;
                            c.goingDown = false;
                            c.contTrapped = 0;
                            // to change the emptyList attribute if ccList is empty
                            if (ccList.Count == 0)
                                emptyList = true;
                        }
                        else
                            c.contTrapped++;
                        c.lastPosY = cPosY;
                    }
                    else
                        c.lastPosY = cPosY;
                }
            }
        }
	}

    public static void InsertToList(CStateUnit cs, Rigidbody rb, NavMeshAgent nm, Transform tr)
    {
        FlyingUnit fU = new FlyingUnit();

        fU.cs = cs;
        fU.rigidbody = rb;
        fU.navMeshAgent = nm;
        fU.transform = tr;
        fU.lastPosY = -1;
        fU.contTrapped = 0;
        fU.goingDown = false;

        if (!ccList.Contains(fU))
        {
            ccList.Add(fU);
            if (emptyList)
                emptyList = false;
        }
    }

}
