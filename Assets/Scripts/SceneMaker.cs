using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMaker : MonoBehaviour
{
    public enum SceneType {Ground, Swamp, Ice, Cave, Arena}
    [Header("Params")]
    public int SizeX;
    private Vector2[] EnemyPos;
    public Vector2[] GetEnemyPos()
    {
        return EnemyPos;
    }
    public Vector2[] GetLongEnemyPos()
    {
        Vector2 Pos = new Vector2((Player.active.transform.position.x > 0 ? -1 : 1) * SizeX * 0.5f, 0);
        return new Vector2[2] { Pos, Pos };
    }
    private GroundInfo nowGround;
    [Header("Object")]
    public Transform Ground;
    public Transform Lava;
    public GameObject[] Loot;
    public List<GameObject> FarBackGround;
    public List<GameObject> BackGround;
    public List<GameObject> SceneObjects;
    [Header("Components")]
    public SpriteRenderer Renderer;

    public void MakeScene(int SizeX, int scene)
    {
        ClearScene();

        this.SizeX = SizeX;
        Ground.localScale = new Vector3(SizeX, 5, 1);
        Lava.localScale = new Vector3(SizeX + 100, 30f, 1);

        GameData.NowGround = scene;
        nowGround = GameData.active.Ground[GameData.NowGround];
        Renderer.sprite = nowGround.Ground;
        Level.active.Ui_Game_BackGround.color = nowGround.UiBackColor;
        GameData.Speed = nowGround.Speed;
        GameData.Acceleration = nowGround.Acceleration;

        EnemyPos = new Vector2[2] { new Vector2(-SizeX / 3, 0), new Vector2(SizeX / 3, 0) };

        //Scene Object
        for (int x = -SizeX + 10; x < SizeX - 10; x += 15)
        {
            if (Random.Range(0, 2) == 0)
                continue;
            Vector3 Pos = new Vector3(x + Random.Range(-5, 5), 1, 0);
            int ObjRand = Random.Range(0, nowGround.SceneObject.Length);
            GameObject Obj = Instantiate(nowGround.SceneObject[ObjRand], Pos, Quaternion.identity);
            Obj.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
            SceneObjects.Add(Obj);
        }

        //BackGround Object
        for (int x = -SizeX + 10; x < SizeX - 10; x += 5)
        {
            if (Random.Range(0, 5) == 0)
                continue;
            Vector3 Pos = new Vector3(x + Random.Range(-2, 2), 0, 0);
            int ObjRand = Random.Range(0, nowGround.BackGround.Length);
            GameObject Obj = Instantiate(nowGround.BackGround[ObjRand], Pos, Quaternion.identity);
            Obj.transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
            BackGround.Add(Obj);
        }
    }
    public void MakeScene(int SizeX, SceneType type)
    {
        MakeScene(SizeX, (int)type);
    }
    public void MakeRandomScene(int SizeX)
    {
        MakeScene(SizeX, Random.Range(0, GameData.active.Ground.Length));
    }
    public void MakeEmptyScene(int SizeX)
    {
        ClearScene();

        this.SizeX = SizeX;
        Ground.localScale = new Vector3(SizeX, 5, 1);
        Lava.localScale = new Vector3(SizeX + 100, 30f, 1);

        GameData.NowGround = 0;
        nowGround = GameData.active.Ground[GameData.NowGround];
        Renderer.sprite = nowGround.Ground;
        GameData.Speed = nowGround.Speed;
        GameData.Acceleration = nowGround.Acceleration;

        EnemyPos = new Vector2[2] { new Vector2(-SizeX / 3, 0), new Vector2(SizeX / 3, 0) };
    }
    public void ClearScene()
    {
        for(int i = 0; i < BackGround.Count; i++)
        {
            GameObject obj = BackGround[i].gameObject;
            Destroy(obj);
        }
        for (int i = 0; i < SceneObjects.Count; i++)
        {
            GameObject obj = SceneObjects[i].gameObject;
            Destroy(obj);
        }

        BackGround.Clear();
        SceneObjects.Clear();
    }

    public void SpawnBox(Vector2 Pos, Vector2 Impulse)
    {
        GameObject Obj = Instantiate(Loot[Random.Range(0, Loot.Length)], Pos, Quaternion.identity, null);
        SceneObject Script = Obj.GetComponent<SceneObject>();
        Script.Fly = true;
        Script.Rig.velocity += Impulse;
        Script.Rig.angularVelocity += Random.Range(-360, 360);
        SceneObjects.Add(Obj);
    }
    public void SpawnBuff(Vector2 Pos, Vector2 Impulse)
    {
        Buff Obj = Instantiate(GameData.active.GetRandomBuff(), Pos, Quaternion.identity, null);
        Obj.Rig.velocity += Impulse;
        Obj.Rig.angularVelocity += Random.Range(-360, 360);
    }
    public void SpawnRandObject(Vector2 Pos, Vector2 Impulse)
    {
        GameObject Obj = Instantiate(nowGround.SceneObject[Random.Range(0, nowGround.SceneObject.Length)], Pos, Quaternion.identity, null);
        SceneObject Script = Obj.GetComponent<SceneObject>();
        Script.Fly = true;
        Script.Rig.velocity += Impulse;
        Script.Rig.angularVelocity += Random.Range(-360, 360);
        SceneObjects.Add(Obj);
    }
    
}
public struct SceneData
{
    public GameObject[] BackGround;
    public GameObject[] SceneObject;
    public GroundInfo Ground;

    public SceneData(GameObject[] backGround, GameObject[] sceneObject, GroundInfo ground)
    {
        BackGround = backGround;
        SceneObject = sceneObject;
        Ground = ground;
    }
}