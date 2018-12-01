using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UEventListener : Singleton<UEventListener>
{
    public delegate void EventCallback(EventBase eb);

    private Dictionary<string, List<EventCallback>> registedCallbacks = new Dictionary<string, List<EventCallback>>();
    private Dictionary<string, List<EventCallback>> registedCallbacksPending = new Dictionary<string, List<EventCallback>>();
    private List<EventBase> lPendingEvents = new List<EventBase>();



    public void RegistEventListener(string sEventName, EventCallback eventCallback)
    {
        lock (this)
        {
            if (!registedCallbacks.ContainsKey(sEventName))
            {
                registedCallbacks.Add(sEventName, new List<EventCallback>());
            }

            if (isEnuming)
            {
                if (!registedCallbacksPending.ContainsKey(sEventName))
                {
                    registedCallbacksPending.Add(sEventName, new List<EventCallback>());
                }
                registedCallbacksPending[sEventName].Add(eventCallback);
                return;
            }

            registedCallbacks[sEventName].Add(eventCallback);
        }
    }

    public void UnregistEventListener(string sEventName, EventCallback eventCallback)
    {
        lock (this)
        {
            if (!registedCallbacks.ContainsKey(sEventName))
            {
                return;
            }

            if (isEnuming)
            {
                return;
            }

            registedCallbacks[sEventName].Remove(eventCallback);
        }
    }

    List<EventBase> lEvents = new List<EventBase>();

    public void DispatchEvent<T>(T eventInstance)
        where T : EventBase
    {
        lock (this)
        {
            if (!registedCallbacks.ContainsKey(eventInstance.sEventName))
            {
                return;
            }

            if (isEnuming)
            {
                lPendingEvents.Add(eventInstance);
                return;
            }

            for (int i = 0; i < lPendingEvents.Count; i++)
            {
                lEvents.Add(lPendingEvents[i]);
            }
            lPendingEvents.Clear();

            lEvents.Add(eventInstance);
        }
    }

    public void DispatchEvent(string eventName, object eventValue)
    {
        //		MyDebug.Log ("DispatchEvent......."+eventName);
        lock (this)
        {
            if (!registedCallbacks.ContainsKey(eventName))
            {
                return;
            }

            if (isEnuming)
            {
                lPendingEvents.Add(new EventBase(eventName, eventValue));
                return;
            }

            lEvents.Add(new EventBase(eventName, eventValue));
        }
    }

    private void testPendingEvents()
    {
		if(lPendingEvents.Count>0)
		{
			for (int i = 0; i < lPendingEvents.Count; i++)
			{
				lEvents.Add(lPendingEvents[i]);
			}
			lPendingEvents.Clear();
		}
    }

    public static bool isEnuming = false;
    public static int eventCount = 0;
    
    public void OnTick()
    {
        lock (this)
        {
            if (lEvents.Count == 0)
            {
				if(registedCallbacksPending.Keys.Count>0)
				{
					foreach(string el in registedCallbacksPending.Keys)
					{
						for (int j = 0; j < registedCallbacksPending[el].Count; j++)
						{
							RegistEventListener(el, registedCallbacksPending[el][j]);
						}
					}
					
					registedCallbacksPending.Clear();
				}
               

                testPendingEvents();
                return;
            }

            isEnuming = true;

            for (int j = 0; j < lEvents.Count; j++)
            {
                for (int i = 0; i < registedCallbacks[lEvents[j].sEventName].Count; i++)
                {
                    EventCallback ecb = registedCallbacks[lEvents[j].sEventName][i];
                    if (ecb.Target == null)
                    {
                        Debug.Log("event-call's target is null");
                    }
                    Component go = ecb.Target as Component;
                    if (null == go)
                    {
                        continue;
                    }
                    if (go != null && !go.gameObject.activeInHierarchy)
                    {
                        continue;
                    }
                    if (ecb == null)
                    {
                        continue;
                    }
                    ecb(lEvents[j]);
                    eventCount++;
                }
            }
            lEvents.Clear();
        }
        isEnuming = false;
    }
}

public class EventBase
{
    public string sEventName;
    public object eventValue;
	public object[] eventValues
	{
		get
		{
			return eventValue as object[];
		}
	}

	public EventBase()
    {
        sEventName = this.GetType().FullName;
    }

    public EventBase(string eventName, object ev)
    {
        eventValue = ev;
        sEventName = eventName;
    }
}

