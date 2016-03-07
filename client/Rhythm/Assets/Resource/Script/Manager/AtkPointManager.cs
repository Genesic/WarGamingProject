using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtkPointManager : ObjectPools<AtkPointManager, AtkPointBasic> {

    public Dictionary<int, AtkPointBasic> atkList;
    protected override Transform ContainerTs { get { return this.transform; } }

    protected override void Awake() { base.Awake(); }

    protected override void OnDestroy() { base.OnDestroy(); }
    //public GameObject defPointPrefab;
    
    public static void Retrieve(AtkPointBasic obj)
    {
        instance.InsRetrieve(obj);
    }
    
    public override AtkPointBasic CreateNew(string id)
    {
        // var path = string.Format("Prefab/DefendPoint");
        // var prefab = Resources.Load<GameObject>(path);
        var dp = MainManager.dataCenter.dpGroup.getDefendPointByString(id);        
        var atkPointGo = Instantiate(dp.prefab);
        var atkPointTs = atkPointGo.transform;
        atkPointTs.SetParent(ContainerTs);
        
        AtkPointBasic setting = atkPointGo.GetComponent<AtkPointBasic>();
        setting.setParam(id);        
        return setting;
    }
}
