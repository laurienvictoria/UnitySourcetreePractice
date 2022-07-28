using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static class Geometry
    {
        public static readonly Vector3 FORWARD_VECTOR = Vector3.forward;
        public static readonly Vector3 RIGHT_VECTOR = Vector3.right;
        public static readonly Vector2 RIGHT_VECTOR2 = Vector2.right;
        public static readonly Vector3 UP_VECTOR = Vector3.up;
        public static readonly Vector2 UP_VECTOR2 = Vector2.up;
        public static readonly Vector3 ZERO_VECTOR = Vector3.zero;
        public static readonly Vector2 ZERO_VECTOR2 = Vector2.zero;
        public static readonly Vector3 ONE_VECTOR = Vector3.one;
        public static readonly Vector2 ONE_VECTOR2 = Vector2.one;

        public static readonly Quaternion IDENTITY_QUATERNION = Quaternion.identity;
    }

    public static class Paths
    {
        public const string PATH_TO_INK_FILES = "Assets/_Lilith/Writing/";
        public const string PATH_TO_DIALOGUE_DATA = "Assets/_Lilith/Data/DialogueData/";
        public const string PATH_TO_AUDIO_FILES = "Assets/_Lilith/Audio/";      //Todo will be removed eventually to integrate Fmod

        public const string ASSET_EXTENSION = ".asset";
        public const string INK_EXTENSION = ".ink";
        public const string JSON_EXTENSION = ".json";
        public const string AUDIO_EXTENSION = ".mp3";

    }

    public static class Events
    {
        public const string ON_KNOCK_EVENT_NAME = "OnPlayerKnock";
        public const string ON_EAVESDROP_EVENT_NAME = "OnPlayerEavesdrop";
    }

    public static class Syntax
    {
        public const string INK_TAG_PREFIX = "#";
        public const string AUDIO_SYNTAX = @"(?<tag>audio-(?<dialogueID>\d{4}))";
        public const string AUDIO_TAG_SYNTAX = INK_TAG_PREFIX + AUDIO_SYNTAX;
    }
}
