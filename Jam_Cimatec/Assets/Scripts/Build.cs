using System;
using UnityEngine;

[Serializable]
public abstract class Build : MonoBehaviour
{
    public GameObject prefab;
    public string buildName;
    public int cost;
    public int life;
    public int maxLife;

    public virtual void Initialize(GameObject instance) { }

    public virtual GameObject CreateGhost()
    {
        GameObject ghost = Instantiate(prefab);

        foreach (var comp in ghost.GetComponentsInChildren<Collider2D>()) //Remove as colisões do fantasma
            Destroy(comp);

        foreach (var spriteRender in ghost.GetComponentsInChildren<SpriteRenderer>())
            spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 0.5f); //Deixa transparente

        foreach (var script in ghost.GetComponentsInChildren<MonoBehaviour>())
            script.enabled = false; //Desliga as lógicas de update

        return ghost;
    }

    public virtual GameObject BuildIn(Vector3 position)
    {
        GameObject newBuild = Instantiate(prefab, position, Quaternion.identity);
        Initialize(newBuild);
        return newBuild;

    }
}
