using System.Collections.Generic;
using MessagePack;

[MessagePackObject(true)]
public class SKillpointData
{
    public Dictionary<string, int> Points { get; set; }
}