using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

[UnitCategory("_Lilith")]
public class ScriptFragment : Unit
{
    [DoNotSerialize]
    public ControlInput inputTrigger;

    [DoNotSerialize]
    public ControlOutput outputTrigger;

    [DoNotSerialize]
    public ValueInput fragmentInput;

    [DoNotSerialize]
    public ValueOutput result;

    private bool resultValue;

    protected override void Definition()
    {

        inputTrigger = ControlInput("inputTrigger", (flow) =>
        {
            resultValue = PlayFragment(flow.GetValue<DialogueDataBank>(fragmentInput));
            return outputTrigger;
        });
        outputTrigger = ControlOutput("outputTrigger");

        fragmentInput = ValueInput<DialogueDataBank>("ScriptFragment", null);
        result = ValueOutput<bool>("result", (flow) => resultValue);

        Requirement(fragmentInput, inputTrigger);
        Succession(inputTrigger, outputTrigger);
        Assignment(inputTrigger, result);
    }

    public bool PlayFragment(DialogueDataBank dataBank)
    {
        if(dataBank == null)
        {
            return false;
        }
        
        //Seems hacky, might need a different way to do this if it messes things up
        Game.EventBarn.StartCoroutine(WaitForTime(dataBank));
        return true;
    }

    private IEnumerator WaitForTime(DialogueDataBank dataBank)
    {
        // TODO: Time goes into data bank asset. Replace foreach
        foreach (KeyValuePair<string, DialogueData> lineData in dataBank.lineDictionary)
        {
            Game.EventBarn.m_onNewStoryText.Dispatch(lineData.Value);
            yield return new WaitForSeconds(lineData.Value.m_duration);
        }
        Game.EventBarn.m_onDialogueEnded.Dispatch();
    }
}

