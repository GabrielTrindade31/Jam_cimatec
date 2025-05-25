using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BuildType
{
    ATTACK,
    GENERATION,
    DEFENSE,
    CoreBuild
}

public abstract class Build : MonoBehaviour
{
    protected Slider healthBar;
    public GameObject prefab;
    public string buildName;
    public BuildType buildType;
    public int cost;
    public float currentLife;
    public Stat MaxLife;
    public bool canRotate = false;

    public virtual void Start()
    {
        currentLife = MaxLife.Value;
        healthBar = transform.GetComponentInChildren<Slider>(true);
    }

    void Update()
    {
        healthBar.maxValue = MaxLife.Value;
        healthBar.value = currentLife;
        healthBar.fillRect.gameObject.SetActive(currentLife > 0.01);
    }

    public GameObject CreateGhostInstance(float rotation)
    {
        Quaternion rot = canRotate ? Quaternion.Euler(0, 0, rotation) : Quaternion.identity;
        GameObject ghost = Instantiate(prefab, Vector3.zero, rot);

        foreach (var comp in ghost.GetComponentsInChildren<Collider2D>()) //Remove as colisões do fantasma
            Destroy(comp);

        foreach (var spriteRender in ghost.GetComponentsInChildren<SpriteRenderer>())
            spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 0.5f); //Deixa transparente

        foreach (var script in ghost.GetComponentsInChildren<MonoBehaviour>())
            script.enabled = false; //Desliga as lógicas de update

        return ghost;
    }

    public virtual GameObject BuildIn(Vector3 position, float rotation, TowerBuilder tw)
    {
        Quaternion rot = canRotate ? Quaternion.Euler(0, 0, rotation) : Quaternion.identity;
        GameObject newBuild = Instantiate(prefab, position, rot);
        return newBuild;
    }
}

[Serializable]
public class BuildEntry
{
    public BuildType buildType;
    public List<Build> constructions;
}