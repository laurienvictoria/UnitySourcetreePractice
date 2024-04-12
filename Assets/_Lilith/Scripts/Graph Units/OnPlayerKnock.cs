using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[UnitTitle("On Player Knock Event")]
[UnitCategory("Events\\_Lilith")]
public class OnPlayerKnock : EventUnit<int>
{
    [DoNotSerialize]
    public ValueOutput result { get; private set; }// The event output data to return when the event is triggered.
    protected override bool register => true;

    // Adding an EventHook with the name of the event to the list of visual scripting events.
    public override EventHook GetHook(GraphReference reference)
    {
        return new EventHook(Constants.Events.ON_KNOCK_EVENT_NAME);
    }
    protected override void Definition()
    {
        base.Definition();
        // Setting the value on our port.
        result = ValueOutput<int>(nameof(result));
    }
    // Setting the value on our port.
    protected override void AssignArguments(Flow flow, int data)
    {
        flow.SetValue(result, data);
    }
}
