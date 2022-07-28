using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[UnitTitle("On Player Eavesdrop Event")]
[UnitCategory("Events\\_Lilith")]
public class OnPlayerEavesdrop : EventUnit<string>
{
    [DoNotSerialize]
    public ValueInput triggerInput;
    [DoNotSerialize]
    public ValueOutput result { get; private set; }// The event output data to return when the event is triggered.
    protected override bool register => true;

    private string dispatchedID;
    private string listeningForID;

    // Adding an EventHook with the name of the event to the list of visual scripting events.
    public override EventHook GetHook(GraphReference reference)
    {
        return new EventHook(Constants.Events.ON_EAVESDROP_EVENT_NAME);
    }
    
    protected override void Definition()
    {
        base.Definition();

        triggerInput = ValueInput<string>("Trigger ID", null);
        
        // Setting the value on our port.
        result = ValueOutput<string>(nameof(result));
    }
    // Setting the value on our port.
    protected override void AssignArguments(Flow flow, string data)
    {
        flow.SetValue(result, data);
    }
    //public override void StartListening(GraphStack stack)
    //{
    //    if(dispatchedID == null || listeningForID == null || dispatchedID != listeningForID)
    //    {
    //        Debug.Log("Invalid Trigger ID");
    //        return;
    //    }

    //    base.StartListening(stack);
    //}

    protected override bool ShouldTrigger(Flow flow, string args)
    {
        listeningForID = flow.GetValue<string>(triggerInput);
        dispatchedID = args;
        if (string.IsNullOrEmpty(dispatchedID) || string.IsNullOrEmpty(listeningForID) || dispatchedID != listeningForID)
        {
            return false;
        }

        return true;
    }


}
