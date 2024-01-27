using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextBullet : BulletBase
{
    [SerializeField]
    private List<string> Texts;
    [SerializeField]
    private TextMeshProUGUI tmp;
    public override void Shoot(Vector3 direction, Vector3 pos)
    {
        tmp.text = GetRandomTexts();
        base.Shoot(direction, pos);
        if (direction.x > 0)
            transform.right = direction;
        else
            transform.right = -direction;
    }    
    public string GetRandomTexts()
    {
        int randomNum = Random.Range(0, Texts.Count);
        return Texts[randomNum];
    }
}
