using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycodeController : MonoBehaviour
{
    public ControlDevice Robot;
    public GameObject Dog;
    public GameObject Dragon1;
    public GameObject Dragon2;
    private GameObject nowObject;
    public string FrontKey;
    public string BackKey;
    public string RightKey;
    public string LeftKey;
    // Start is called before the first frame update
    void Start()
    {
        nowObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        checkDevice();
        if (Robot != ControlDevice.None)
        {
            if (nowObject.GetComponent<MonsteroomRobot_ctl>().nowType == MonsteroomRobot_ctl.type.Control)
            {
                Ctl();
            }
        }
    }
    public enum ControlDevice
    {
        None,
        Dog,
        Dragon1,
        Dragon2
    }
    void checkDevice()
    {
        switch (Robot)
        {
            case ControlDevice.None:
                nowObject = null;
                break;
            case ControlDevice.Dog:
                nowObject = Dog;
                break;
            case ControlDevice.Dragon1:
                nowObject = Dragon1;
                break;
            case ControlDevice.Dragon2:
                nowObject = Dragon2;
                break;
        }
    }
    void Ctl()
    {
        if (Input.GetKey(FrontKey))
        {
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobR = nowObject.GetComponent<MonsteroomRobot_ctl>().Front_Rspeed;
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobL = nowObject.GetComponent<MonsteroomRobot_ctl>().Front_Lspeed;
        }
        else if (Input.GetKey(BackKey))
        {
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobR = nowObject.GetComponent<MonsteroomRobot_ctl>().Back_Rspeed;
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobL = nowObject.GetComponent<MonsteroomRobot_ctl>().Back_Lspeed;
        }
        else if (Input.GetKey(RightKey))
        {
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobR = nowObject.GetComponent<MonsteroomRobot_ctl>().Back_Rspeed;
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobL = nowObject.GetComponent<MonsteroomRobot_ctl>().Front_Lspeed;
        }
        else if (Input.GetKey(LeftKey))
        {
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobR = nowObject.GetComponent<MonsteroomRobot_ctl>().Front_Rspeed;
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobL = nowObject.GetComponent<MonsteroomRobot_ctl>().Back_Lspeed;
        }
        else
        {
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobR = 0;
            nowObject.GetComponent<MonsteroomRobot_ctl>().RobL = 0;
        }
        nowObject.GetComponent<MonsteroomRobot_ctl>().RobR = Mathf.Abs(nowObject.GetComponent<MonsteroomRobot_ctl>().RobR) > nowObject.GetComponent<MonsteroomRobot_ctl>().MaxSpeed ? nowObject.GetComponent<MonsteroomRobot_ctl>().MaxSpeed * (nowObject.GetComponent<MonsteroomRobot_ctl>().RobR / Mathf.Abs(nowObject.GetComponent<MonsteroomRobot_ctl>().RobR)) : nowObject.GetComponent<MonsteroomRobot_ctl>().RobR;
        nowObject.GetComponent<MonsteroomRobot_ctl>().RobL = Mathf.Abs(nowObject.GetComponent<MonsteroomRobot_ctl>().RobL) > nowObject.GetComponent<MonsteroomRobot_ctl>().MaxSpeed ? nowObject.GetComponent<MonsteroomRobot_ctl>().MaxSpeed * (nowObject.GetComponent<MonsteroomRobot_ctl>().RobL / Mathf.Abs(nowObject.GetComponent<MonsteroomRobot_ctl>().RobL)) : nowObject.GetComponent<MonsteroomRobot_ctl>().RobL;
    }
}
