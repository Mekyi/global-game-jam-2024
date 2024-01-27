using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayscaleShaderScaler : MonoBehaviour
{
    private Material _material;

    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    public void SetColorScale(float colorScale)
    {
        _material.SetFloat("_ColorScale", Mathf.Clamp(colorScale, 0f, 1f));
    }
}
