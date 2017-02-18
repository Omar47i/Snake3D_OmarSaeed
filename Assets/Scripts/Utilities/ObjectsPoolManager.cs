using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsPoolManager : MonoBehaviour 
{
    [System.Serializable]
    public class ObjectsPoolProperties
    {
        public GameObject ObjectPrefab;
        public int InitialCount = 1;
        //public ObjectPrefabType ObjectType;
    }

    public static ObjectsPoolManager Instance;

    public List<ObjectsPoolProperties> ObjectsPoolPropertiesList;
    private Dictionary<string, ObjectsPoolProperties> HashedObjectsPoolPropertiesList;

    private List<GameObject> NeedToCallOnActivate;
    private List<GameObject> NeedToCallOnDeactivate;

    private HashSet<GameObject> CurrentlyUsedObjects;
    //private Dictionary<ObjectPrefabType, List<GameObject>> ObjectsPool;
    private Dictionary<string, List<GameObject>> ObjectsPool;

    void Awake()
    {
        Instance = this;

        HashedObjectsPoolPropertiesList = new Dictionary<string, ObjectsPoolProperties>();
        //ObjectsPool = new Dictionary<ObjectPrefabType, List<GameObject>>();
        ObjectsPool = new Dictionary<string, List<GameObject>>();

        NeedToCallOnActivate = new List<GameObject>();
        NeedToCallOnDeactivate = new List<GameObject>();

        CurrentlyUsedObjects = new HashSet<GameObject>();

        InitPool();

    }
	
    void InitPool()
    {
        for (int i = 0; i < ObjectsPoolPropertiesList.Count; i++)
        {
            string prefabName = GetPooledObjectPrefabName(ObjectsPoolPropertiesList[i].ObjectPrefab);

            HashedObjectsPoolPropertiesList.Add(prefabName, ObjectsPoolPropertiesList[i]);

            List<GameObject> objPoolList = new List<GameObject>();
            ObjectsPool.Add(prefabName, objPoolList);

            for (int j = 0; j < ObjectsPoolPropertiesList[i].InitialCount; j++)
            {
                //Create the pooled object
                GameObject obj = GameObject.Instantiate(ObjectsPoolPropertiesList[i].ObjectPrefab);
                obj.SetActive(false);

                //Store it in the pool dictionary
                objPoolList.Add(obj);
            }
        }
    }

    private string GetPooledObjectPrefabName(GameObject obj)
    {
        return string.Format("{0}(Clone)", obj.name);
    }

    public GameObject GetPooledObject(GameObject gameObjectPrefab)
    {        
        string prefabName = GetPooledObjectPrefabName(gameObjectPrefab);

        if(!HashedObjectsPoolPropertiesList.ContainsKey(prefabName))
        {
            GameObject newObj = GameObject.Instantiate(gameObjectPrefab);
            return newObj;
        }

        List<GameObject> pooledObjects = ObjectsPool[prefabName];

        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if(!CurrentlyUsedObjects.Contains(pooledObjects[i]))
            {
                CurrentlyUsedObjects.Add(pooledObjects[i]);
                pooledObjects[i].SetActive(true);
                RegisterOnPooledObjectActivated(pooledObjects[i]);
                return pooledObjects[i];
            }
        }

        //Create a new pooled object
        GameObject obj = GameObject.Instantiate(HashedObjectsPoolPropertiesList[prefabName].ObjectPrefab);

        CurrentlyUsedObjects.Add(obj);
        pooledObjects.Add(obj);
        RegisterOnPooledObjectActivated(obj);

        return obj;
    }

    public GameObject GetPooledObject(GameObject gameObjectPrefab, Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetPooledObject(gameObjectPrefab);

        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }

        return obj;
    }

    private void RegisterOnPooledObjectActivated(GameObject obj)
    {
        if (NeedToCallOnDeactivate.Contains(obj))
        {
            OnPooledObjectDeactivated(obj);
            NeedToCallOnDeactivate.Remove(obj);
        }
        
        NeedToCallOnActivate.Add(obj);
    }

    private void OnPooledObjectActivated(GameObject obj)
    {
        PooledObjectScriptBase[] poolScripts = obj.GetComponents<PooledObjectScriptBase>();

        for (int i = 0; i < poolScripts.Length; i++)
        {
            poolScripts[i].OnPooledObjectActivated();
        }
    }

    private void RegisterOnPooledObjectDeactivated(GameObject obj)
    {/*
        if (NeedToCallOnActivate.Contains(obj))
        {
            OnPooledObjectActivated(obj);
            NeedToCallOnActivate.Remove(obj);
        }
*/
        NeedToCallOnDeactivate.Add(obj);
    }

    private void OnPooledObjectDeactivated(GameObject obj)
    {
        PooledObjectScriptBase[] poolScripts = obj.GetComponents<PooledObjectScriptBase>();

        for (int i = 0; i < poolScripts.Length; i++)
        {
            poolScripts[i].OnPooledObjectDeactivated();
        }

        CurrentlyUsedObjects.Remove(obj);
    }

    public bool IsPooledGameOject(GameObject obj)
    {
        //Create a new pooled object
        if (obj.GetComponent<PooledObject>() == null)
            return false;

        return true;
    }

    public void DestroyPooledGameObject(GameObject obj)
    {
        //Destroy(obj);
        //return;
        
        //Check to see if the object is registered
        if(IsPooledGameOject(obj))
        {
            if (CurrentlyUsedObjects.Contains(obj))
            {
                obj.gameObject.SetActive(false);
                RegisterOnPooledObjectDeactivated(obj);
            }
            return;
        }

        DestroyObject(obj);
    }

    public void DestroyPooledGameObject(GameObject obj, float delay)
    {
        StartCoroutine(DestroyPooledGameObjectCoroutine(obj, delay));
    }

    private IEnumerator DestroyPooledGameObjectCoroutine(object obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        DestroyPooledGameObject((GameObject)obj);
    }

    public void DestroyGameObjectWithPooledChildren(GameObject obj)
    {
        //Destroy(obj);
        //return;

        PooledObject[] children = obj.GetComponentsInChildren<PooledObject>();

        for (int i = 0; i < children.Length; i++)
        {
            children[i].gameObject.transform.SetParent(null);

            if (CurrentlyUsedObjects.Contains(children[i].gameObject))
            {
                children[i].gameObject.SetActive(false);
                RegisterOnPooledObjectDeactivated(children[i].gameObject);
            }
        }

        DestroyPooledGameObject(obj);
    }

    void LateUpdate()
    {
        if (NeedToCallOnActivate.Count > 0)
        {
            OnPooledObjectActivated(NeedToCallOnActivate[0]);

            NeedToCallOnActivate.RemoveAt(0);
        }

        if (NeedToCallOnDeactivate.Count > 0)
        {
            OnPooledObjectDeactivated(NeedToCallOnDeactivate[0]);

            NeedToCallOnDeactivate.RemoveAt(0);
        }
    }
}
