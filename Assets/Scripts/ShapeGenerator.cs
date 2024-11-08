using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class ShapeGenerator
{
    ShapeSettings settings;
    NoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;
    public void UpdateSettings(ShapeSettings settings)
    {
        this.settings = settings;
        noiseFilters = new NoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = new NoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
        elevationMinMax = new MinMax();
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float elevation = 0;
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
            elevation += noiseFilters[i].Evaluate(pointOnUnitSphere); 
            }
        }
        elevation = (float)Math.Floor((settings.planetRadius) * (1 + (double)(elevation)));
        elevationMinMax.AddValue(elevation);
        return pointOnUnitSphere * elevation;
    }
}
