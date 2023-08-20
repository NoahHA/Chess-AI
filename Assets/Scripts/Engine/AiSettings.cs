using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AiSettings
{
    public enum Search
    {
        Time, Depth
    }

    public static Search SearchMode = Search.Time;
    public static int SearchDepth = 3;
    public static float TimeLimit_ms = 1000;
    public static bool Benchmarking = false;
    public static int MaxComputerTurns = 10;
    public static string VersionDescription = "v1";
}