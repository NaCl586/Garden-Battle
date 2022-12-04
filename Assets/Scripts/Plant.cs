﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] private Sprite[] GrowthPhase;
    [SerializeField] private Sprite requireWaterIcon;
    [SerializeField] private Sprite requireFertilizerIcon;
    [SerializeField] private Sprite infectedIcon;

    public Color dullColor;

    private SpriteRenderer sr;
    private SpriteRenderer indicator;
    private SpriteRenderer infected;

    private float phaseTimer;
    private float waterFertilizerTimer;
    private bool requireWater;
    private bool requireFertilizer;

    private bool infectedWithPest;
    private float harvestTimer;
    private bool pesticideEffects;
    private float pesticideTimer;
    

    private bool plantIsAlive;

    private Phases currentPhase;

    private int waterFertilizerStartTime = 30;

    private enum Phases{
        none,
        seed,
        stem,
        leaves,
        fruit
    };

    private void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        indicator = this.transform.GetChild(0).GetComponent<SpriteRenderer>();
        infected = this.transform.GetChild(1).GetComponent<SpriteRenderer>();
        currentPhase = Phases.none;
        plantIsAlive = false;
    }


    //public methods bisa dipanggil buat player actions nya
    public void PlantSeed()
    {
        initPlant();
    }

    public void HarvestFruit()
    {
        if (currentPhase != Phases.fruit) return;
        initLeaves();
        //bkin player jadi bawa buah
        //...
    }

    public void KasihAir()
    {
        requireWater = false;
    }

    public void KasihFertilizer()
    {
        requireFertilizer = false;
    }

    public void KasihHama()
    {
        if (pesticideEffects) return;
        infectedWithPest = true;
        waterFertilizerStartTime = 15;
    }

    public void KasihPesticide()
    {
        infectedWithPest = false;
        waterFertilizerStartTime = 30;

        pesticideEffects = true;
        pesticideTimer = 60;
    }


    //darisini kebawah udh private method, internal dari taneman nya.. jgn dikotak katik lagi
    IEnumerator randomizeNeedWaterFertilizer()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
        if (Random.Range(1, 10) <= 3)
        {
            if (Random.Range(1, 100) % 2 == 0)
                requireFertilizer = true;
            else
                requireWater = true;

            waterFertilizerTimer = 30;
        }
        else
        {
            requireFertilizer = requireWater = false;
        }
    }

    private void initPlant()
    {
        if (plantIsAlive) return;
        plantIsAlive = true;

        sr.sprite = GrowthPhase[1];
        currentPhase = Phases.seed;
        phaseTimer = Random.Range(10, 20);

        StartCoroutine(randomizeNeedWaterFertilizer());
    }

    private void initStem()
    {
        sr.sprite = GrowthPhase[2];
        currentPhase = Phases.stem;
        phaseTimer = Random.Range(10, 20);

        StartCoroutine(randomizeNeedWaterFertilizer());
    }

    private void initLeaves()
    {
        sr.sprite = GrowthPhase[3];
        currentPhase = Phases.leaves;
        phaseTimer = Random.Range(10, 20);

        StartCoroutine(randomizeNeedWaterFertilizer());
    }

    private void initFruit()
    {
        sr.sprite = GrowthPhase[4];
        currentPhase = Phases.fruit;

        if (infectedWithPest)
            harvestTimer = 30;
    }

    private void killPlant()
    {
        plantIsAlive = false;
        sr.color = Color.white;
        sr.sprite = GrowthPhase[0];
        currentPhase = Phases.none;
        requireFertilizer = requireWater = infectedWithPest = false;
    }

    // Update is called once per frame
    void Update()
    {
        //return if plant is not alive
        if (!plantIsAlive)
        {
            indicator.sprite = infected.sprite = null;
            return;
        }

        //icons
        if (requireFertilizer) indicator.sprite = requireFertilizerIcon;
        else if (requireWater) indicator.sprite = requireWaterIcon;
        else indicator.sprite = null;

        infected.sprite = (infectedWithPest) ? infectedIcon : null;

        //water/fertilizer timers
        if (requireFertilizer || requireWater)
        {
            float colorSaturation = waterFertilizerTimer / waterFertilizerStartTime;
            sr.color = new Color(Mathf.Lerp(1, dullColor.r, 1-colorSaturation),
                                Mathf.Lerp(1, dullColor.g, 1-colorSaturation),
                                Mathf.Lerp(1, dullColor.b, 1-colorSaturation),
                                1
            );
            waterFertilizerTimer -= Time.deltaTime;
            if (waterFertilizerTimer <= 0) killPlant();
        }
        else
        {
            sr.color = Color.white;
        }

        //pesticide Timers
        if (pesticideEffects)
        {
            pesticideTimer -= Time.deltaTime;
            if (pesticideTimer <= 0) pesticideEffects = false;
        }

        //regular phase change timers
        phaseTimer -= Time.deltaTime;
        
        //fruit hama thingy
        if(infectedWithPest && currentPhase == Phases.fruit)
        {
            float colorSaturation = harvestTimer / 30;
            sr.color = new Color(Mathf.Lerp(1, dullColor.r, 1 - colorSaturation),
                                Mathf.Lerp(1, dullColor.g, 1 - colorSaturation),
                                Mathf.Lerp(1, dullColor.b, 1 - colorSaturation),
                                1
            );

            harvestTimer -= Time.deltaTime;
            if (harvestTimer == 0) killPlant();
        }
        else if(!infectedWithPest && currentPhase == Phases.fruit)
        {
            sr.color = Color.white;
        }

        //change phase ready
        if (phaseTimer <= 0)
        {
            phaseTimer = 0;

            //change phase if only plant isn't needing water/fertilizer
            if (requireFertilizer || requireWater) return;

            //change phase
            if (currentPhase == Phases.seed) initStem();
            else if (currentPhase == Phases.stem) initLeaves();
            else if (currentPhase == Phases.leaves) initFruit();
        }
    }
}
