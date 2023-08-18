using System;
using System.IO;
using System.Text;
using UnityEngine;

public class Benchmarking
{
    private float totalNodesSearchedPerSecond;
    private float totalDepthReached;
    private int iterations;
    private readonly string filePath = Path.Combine(Application.dataPath, "Benchmarking", "benchmarking_v1.csv");

    public void StartBenchmarking()
    {
        totalNodesSearchedPerSecond = 0;
        totalDepthReached = 0;
        iterations = 0;
    }

    public void RecordMetrics(int nodesSearched, int depthReached, float maxTime_ms)
    {
        totalNodesSearchedPerSecond += nodesSearched / (maxTime_ms / 1000);
        totalDepthReached += depthReached;
        iterations++;
    }

    public void WriteMetricsToCsv(string versionDescription, float maxTime_ms)
    {
        double averageNodesPerSecond = (double)totalNodesSearchedPerSecond / iterations;
        double averageDepth = (double)totalDepthReached / iterations;

        var csv = new StringBuilder();
        var newLine = string.Format("{0},{1},{2},{3}", Math.Round(averageNodesPerSecond, 3), Math.Round(averageDepth, 3), maxTime_ms, versionDescription);
        csv.AppendLine(newLine);
        File.AppendAllText(filePath, csv.ToString());
    }
}