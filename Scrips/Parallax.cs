using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    private float startPos, length;
    public GameObject cam;
    public float parallaxEffect; //Càng tịnh tiến về 1 thì sẽ đứng im và ngược lại 

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Biến tạm lưu giá trị vị trí của camera
        float temp = (cam.transform.position.x * (1 - parallaxEffect));

        //Khoàng cách giữa camera và hiệu ứng
        float distance = (cam.transform.position.x * parallaxEffect);

        //
        transform.position = new Vector3 (startPos + distance, transform.position.y, transform.position.y);

        //Nếu mà camera đã di chuyển đến cuối của background thì tiến hành dịch chuyển ảnh lên và ngược lại
        if(temp > startPos + length)
        {
            startPos += length;
        }
        else if(temp < startPos - length) 
        {
            startPos -= length;
        }
    }
}
