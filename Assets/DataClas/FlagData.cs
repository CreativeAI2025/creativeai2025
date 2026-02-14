using System.Collections.Generic;
using MessagePack;

[MessagePackObject(true)]
public class FlagData
{
    public Dictionary<string, bool> Flags { get; set; }
}