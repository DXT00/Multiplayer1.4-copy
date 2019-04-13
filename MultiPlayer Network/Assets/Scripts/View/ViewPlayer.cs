using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ViewPlayer : MonoBehaviour
{
   

    GameObject player_instance;
    public int connectID;//与Player的connectID相同；


    readonly float speed = 50f;
    readonly float rotate_speed = 5f;
    float Mygravity = 2f;
    public GameObject BulletPrefabs;
    public Transform bulletSpawn;

    Rigidbody PlayerRigidbody;
    List<CustomSyncMsg> msg_list;

    int touchTerrianMask;
    readonly float max_y = 100f;


    //Move
    float z = 0;
    float y = 0;
    float x = 0;

    //Rotate
    float rot_x = 0;
    float rot_y = 0;

    int key_x1 = 0;
    int key_x2 = 0;
    readonly float camRayLength = 100f;
    Vector3 playerToMouse;

    //Shoot
    int shootableMask;
    Ray shootRay = new Ray();
    RaycastHit shootHit;
    float max_shootDistance = 500f;
    bool has_shoot = false;

    //Health
  
    int startingHealth = 1000;
    public int currentHealth=1000;
    bool isDead = false;

    //Damage
    int DamagePerHit = 10;




    Transform Transform;
    CharacterController characterController;
    public CameraFollow camera;

    //Animation
    public Animator Animator;
    string lastAnim = "Idle";  


    public void Start()
    {
        msg_list = new List<CustomSyncMsg>();
        PlayerRigidbody = GetComponent<Rigidbody>();
        touchTerrianMask = LayerMask.GetMask("touchTerrian");
        Transform = gameObject.GetComponent<Transform>();
        PlayerRigidbody.freezeRotation = true;
        characterController = gameObject.GetComponent<CharacterController>();//MY
        Animator = GetComponent<Animator>();//MY
        startingHealth = 1000;
        shootableMask = LayerMask.GetMask("shootable");

        //currentHealth = startingHealth;
       // healthSlider.value = startingHealth;

    }


    public void FixedUpdate()
    {
       
        PlayerRigidbody.AddForce(Physics.gravity * Mygravity, ForceMode.Acceleration);

        x += Input.GetAxis("Horizontal");
        z += Input.GetAxis("Vertical");


        //Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //RaycastHit floorHit;

        //if (Physics.Raycast(camRay, out floorHit, camRayLength, touchTerrianMask))
        //{
        //    playerToMouse = floorHit.point - transform.position;//player到Ray撞到floor的点
        //    playerToMouse.y = 0f;

        //}

        if (Input.GetKeyDown(KeyCode.A))
        {
            key_x1++;
            rot_x = 0;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            key_x2++;
            rot_x = 0;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            rot_x += key_x1 * (-rotate_speed);
            key_x1 = 0;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            rot_x += key_x2 * (rotate_speed);
            key_x2 = 0;
        }

        if (Input.GetMouseButton(0))
        {
            has_shoot = true;
            
            shootRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("************************************** shootRay.origin. =" + "(" + shootRay.origin.x + "," + shootRay.origin.y + "," + shootRay.origin.z + ")");
            Debug.DrawRay(shootRay.origin, shootRay.direction * 100, Color.yellow);
        }
        else
        {
            has_shoot = false;
           
        }

        if (isDead)
        {
            Death();
        }
    }

    public List<CustomSyncMsg> get_local_input()
    {
        msg_list.Clear();
       
        CustomSyncMsg input_msg = new InputMessage(connectID, new Vector3(x, 0, z));
        msg_list.Add(input_msg);
        x = 0;
        z = 0;
      
        if (rot_x != 0f || rot_y != 0f)
        {
            CustomSyncMsg rot_msg = new RotateMessage(connectID, new Vector2(rot_x, rot_y));
            msg_list.Add(rot_msg);
            rot_x = 0;
            rot_y = 0;
        }
        if (has_shoot)
        {
            CustomSyncMsg shoot_msg = new ShootMessage(connectID, shootRay.origin, shootRay.direction);
            msg_list.Add(shoot_msg);
            has_shoot = false;

        }

        return msg_list;
    }
    public void Move(float horizontal, float vertical)//, float y
    {
        if (horizontal == 0 && vertical == 0)
        {
            changeAnim("Idle");
            return;
        }


        // Animator.SetBool("isRuning", true);//MY
        changeAnim("Run");                                  //  PlayerRigidbody.AddForce((Transform.right) * horizontal * speed);

        PlayerRigidbody.AddForce(Transform.forward * vertical * speed);

        // PlayerRigidbody.AddForce(-Transform.up * Mygravity);
    }

    public void Rotate(float delta_x, float delta_y)
    {
        //绕自己的y轴旋转
        //  Animator.SetBool("isRuning", false);//MY
        Transform.Rotate(Vector3.up, delta_x, Space.Self);

        Quaternion targetRotation = new Quaternion(0, Mathf.Sin(delta_x / 2.0f), 0, Mathf.Cos(delta_x / 2.0f));//将角度转化为四元素





        //Vector3 playerToMouse_ = new Vector3(playerToMouse_x, 0, playerToMouse_z);
        //Quaternion targetRotation = Quaternion.LookRotation(playerToMouse_);

        Quaternion newRotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.timeScale * 20.0f);
        PlayerRigidbody.MoveRotation(newRotation);
    }


    public void Shoot()
    {


        changeAnim("Shoot");

        //GameObject bullet = Instantiate(BulletPrefabs, bulletSpawn.position, bulletSpawn.rotation);
        //bullet.GetComponent<Rigidbody>().velocity = transform.forward * 6f;


        //NetworkServer.Spawn(bullet);

        //Destroy(bullet, 2f);
    }
    public void TakeDamage()
    {
        currentHealth -= DamagePerHit;
        Tool.Print("************************************************************----------currentHealth = " + currentHealth+"clientID = "+connectID);
        if (currentHealth < 0)
        {
            isDead = true;
        }
        else
        {
          
           
          //  healthSlider.value = currentHealth;
        }
    }
    public void Death()
    {
        Tool.Print("YOU ARE DEAD!");

    }

    void changeAnim(string nextAnim)
    {
        Animator.SetBool(lastAnim, false);

        Animator.SetBool(nextAnim, true);

        lastAnim = nextAnim;

    }


    public float move_on_the_ground()
    {
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, max_y, touchTerrianMask))
        {
            return hit.point.y + 1f;
        }
        else
            return -101;
    }
    public void set_player_instance(GameObject instance)
    {
        player_instance = instance;
    }
    public float get_speed()
    {
        return speed;
    }
    public Rigidbody get_Rigidbody()
    {
        return PlayerRigidbody;
    }

    public void bind_cameraFollow(CameraFollow cameraFollow)
    {
        camera = cameraFollow;
    }
    public void bind_playerInstance(GameObject Instance)
    {
        player_instance = Instance;
    }
   

}

