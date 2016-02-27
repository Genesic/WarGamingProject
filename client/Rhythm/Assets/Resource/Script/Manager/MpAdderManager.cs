using UnityEngine;
using System.Collections;

public class MpAdderManager : ObjectPools<MpAdderManager, MpAdderBasic>
{

    protected override Transform ContainerTs { get { return this.transform; } }

    protected override void Awake() { base.Awake(); }

    protected override void OnDestroy() { base.OnDestroy(); }
    public GameObject mpAdderPrefab;
    public GameObject rivalmpAdderPrefab;

    public static void Retrieve(MpAdderBasic obj)
    {
        instance.InsRetrieve(obj);
    }

    public override MpAdderBasic CreateNew(string id)
    {
        // var path = string.Format("Prefab/DefendPoint");
        // var prefab = Resources.Load<GameObject>(path);
        var prefab = mpAdderPrefab;       
        if (id == "rival")        
            prefab = rivalmpAdderPrefab;
        
        var go = Instantiate(prefab);
        var ts = go.transform;
        ts.SetParent(ContainerTs);

        MpAdderBasic setting = go.GetComponent<MpAdderBasic>();
        setting.setParam(id);
        return setting;
    }
}
