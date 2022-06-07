using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CachePool  {

    //public static MonoBehaviourPool<CollectProgressbar> CollectProgressbarPool;
    //public static MonoBehaviourPool<DerivativeProgressbar> DerivativeProgressbarPool;
    public static void RegistrationCachePool()
    {
        //CollectProgressbarPool = new MonoBehaviourPool<CollectProgressbar>(ResMgr.Load<CollectProgressbar>("CollectProgressbar"),CommonObjectMgr.poolParent);
        //DerivativeProgressbarPool = new MonoBehaviourPool<DerivativeProgressbar>(ResMgr.Load<DerivativeProgressbar>("DerivativeProgressbar"), CommonObjectMgr.poolParent);
    }

}
