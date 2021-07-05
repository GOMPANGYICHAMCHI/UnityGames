using UnityEngine;
using UnityEngine.UI;

public class CameraMove : MonoBehaviour 
{
    public float UpDownSpeed;

    public int z_max;
    public int z_min;
    public int y_max;
    public int y_min;
    public int x_max;
    public int x_min;

	void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector3 movepos = new Vector3(transform.position.x,transform.position.y,transform.position.z +1);
            if(movepos.z > z_max )
            {
                movepos.z = z_max;
            }
            transform.position = movepos;
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector3 movepos = new Vector3(transform.position.x,transform.position.y,transform.position.z -1);
            if(movepos.z < z_min )
            {
                movepos.z = z_min;
            }
            transform.position = movepos;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3 movepos = new Vector3(transform.position.x-1,transform.position.y,transform.position.z);
            if(movepos.x < x_min )
            {
                movepos.x = x_min;
            }
            transform.position = movepos;
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 movepos = new Vector3(transform.position.x+1,transform.position.y,transform.position.z);
            if(movepos.x > x_max )
            {
                movepos.x = x_max;
            }
            transform.position = movepos;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Vector3 movepos = new Vector3(transform.position.x,transform.position.y-UpDownSpeed,transform.position.z);
            if(movepos.y < y_min )
            {
                movepos.y = y_min;
            }
            transform.position = movepos;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Vector3 movepos = new Vector3(transform.position.x,transform.position.y+UpDownSpeed,transform.position.z);
            if(movepos.y > y_max )
            {
                movepos.y = y_max;
            }
            transform.position = movepos;
        }
    }
}
