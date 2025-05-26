using System;
using System.Collections.Generic;
using TMPro;
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
    protected TowerBuilder tw;
    protected Slider healthBar;
    protected TextMeshProUGUI nameTxt;
    public GameObject prefab;
    public string buildName;
    public BuildType buildType;
    public long cost;
    public float currentLife;
    public Stat MaxLife;
    public bool canRotate = false;

    public virtual void Start()
    {
        currentLife = MaxLife.Value;
        healthBar = transform.GetComponentInChildren<Slider>(true);
        nameTxt = transform.GetComponentInChildren<TextMeshProUGUI>(true);
        nameTxt.text = buildName;
    }

    public void Initialize(TowerBuilder tw)
    {
        this.tw = tw;
    }

    public void DeleteBuild()
    {
        // Joga aqui um animator de morte
        tw.RemoveBuildFromMap(tw.World2Grid(transform.position));
    }

    void Update()
    {
        healthBar.maxValue = MaxLife.Value;
        healthBar.value = currentLife;
        healthBar.fillRect.gameObject.SetActive(currentLife > 0.01);
    }

    public void TakeDamage(float amount)
    {
        currentLife -= amount;
        if (currentLife <= 0) DestroyBuild();
    }

    void DestroyBuild()
    {
        //Implementar alguma coisa aqui pra ela morrer
        if (TryGetComponent<Collider2D>(out var x))
            x.enabled = false;
        
        Destroy(gameObject, 20f);
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

    public GameObject BuildIn(Vector3 position, float rotation, TowerBuilder tw)
    {
        Quaternion rot = canRotate ? Quaternion.Euler(0, 0, rotation) : Quaternion.identity;
        GameObject newBuild = Instantiate(prefab, position, rot);
        newBuild.GetComponent<Build>().Initialize(tw);
        return newBuild;
    }
}

[Serializable]
public class BuildEntry
{
    public BuildType buildType;
    public List<Build> constructions;
}