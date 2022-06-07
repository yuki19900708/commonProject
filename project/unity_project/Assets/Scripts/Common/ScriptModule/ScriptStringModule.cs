using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptStringModule
{
    public static string fieldFormat = "    /// <summary>{0}</summary>\r\n    public {1} {2};\r\n";
    public static string classModule = "using UnityEngine;\r\nusing System.Collections.Generic; \r\n[System.Serializable]\r\npublic class #ScriptName#\r\n{\r\n#Content#\r\n}\r\n";
    public static string eventFormat = "        TableDataMgr.Event_Load#Content# += ResMgr.Load<#Content#>;";
}
