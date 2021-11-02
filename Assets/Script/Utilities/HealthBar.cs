using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    GameObject healthClone;
    float ScreenWidthHalf;
    float ScreenHeightHalf;
    float CameraWidthHalf;
    float CameraHeightHalf;
    public float tinggiPosisi;
    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<Enemy>().isServer)
        {
            healthClone = Instantiate(Resources.Load("Prefab/UI/CanvasHealthBar") as GameObject);
            ScreenWidthHalf = healthClone.GetComponent<CanvasScaler>().referenceResolution.x / 2;
            ScreenHeightHalf = healthClone.GetComponent<CanvasScaler>().referenceResolution.y / 2;
            CameraHeightHalf = GameObject.Find("Main Camera").GetComponent<Camera>().orthographicSize;
            CameraWidthHalf = CameraHeightHalf * ScreenWidthHalf / ScreenHeightHalf;
            healthClone.transform.SetParent(this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<Enemy>().isServer)
        {
            healthClone.transform.Find("HealthBar").transform.localPosition = new Vector2((transform.position.x - GameObject.Find("Main Camera").transform.position.x) * ScreenWidthHalf / CameraWidthHalf, (transform.position.y - GameObject.Find("Main Camera").transform.position.y + tinggiPosisi) * ScreenHeightHalf / CameraHeightHalf);
            healthClone.transform.Find("HealthBar").GetComponent<Slider>().value = GetComponent<Enemy>().health/GetComponent<Enemy>().maxHealth;
        }
    }
}
