using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgrammerBullet : BulletBase
{
    [SerializeField]
    private List<string> oneliners;
    [SerializeField]
    private TextMeshProUGUI tmp;
    public override void Shoot(Vector3 direction, Vector3 pos)
    {
        tmp.text = GetRandomOneliner();
        base.Shoot(direction, pos);
        if (direction.x > 0)
            transform.right = direction;
        else
            transform.right = -direction;
    }    
    public string GetRandomOneliner()
    {
        int randomNum = Random.Range(0, oneliners.Count);
        return oneliners[randomNum];
    }
}
