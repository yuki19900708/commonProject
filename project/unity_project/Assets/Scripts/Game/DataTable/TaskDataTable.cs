using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class TaskDataTable : ScriptableObject
{
    public List<TaskData> taskDataTable = new List<TaskData>();
    public Dictionary<int, TaskData> taskDataDic = new Dictionary<int, TaskData>();

    public void SetDatas(object[] obj)
    {
        taskDataTable.Clear();
        foreach (object o in obj)
        {
            taskDataTable.Add(o as TaskData);
        }
    }

    public List<TaskData> GetAllData()
    {
        if (taskDataTable == null || taskDataTable.Count == 0)
        {
            Debug.LogError("TaskDataTable未导入asset");
        }
        return taskDataTable;
    }

    public IList<TaskData> GetAllReadonlyData()
    {
        if (taskDataTable == null || taskDataTable.Count == 0)
        {
            Debug.LogError("TaskDataTable未导入asset");
        }

        ReadOnlyCollection<TaskData> readOnlyTaskData = new ReadOnlyCollection<TaskData>(taskDataTable);

        return readOnlyTaskData ;
    }

    public TaskData GetData(int index)
    {
        if (taskDataTable == null || taskDataTable.Count == 0)
        {
            Debug.LogError("TaskDataTable未导入asset");
        }
        if (taskDataDic.Count == 0)
        {
            ReadOnlyCollection<TaskData> readOnlyTaskData = new ReadOnlyCollection<TaskData>(taskDataTable);
            foreach (TaskData value in readOnlyTaskData)
            {
                if (taskDataDic.ContainsKey(value.id))
                {
                    Debug.LogError("id重复检查数据表"+ value.id);
                }
                //taskDataDic.Add(value.id, value.Clone());
                taskDataDic.Add(value.id, value);
            }
        }
        if (taskDataDic.ContainsKey(index))
        {
            return taskDataDic[index];
        }
        else
        {
            return null;
        }
        
    }
}

