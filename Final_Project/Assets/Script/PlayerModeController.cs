using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModeController : MonoBehaviour
{
    private int now_mode;
    void Start()
    {
        now_mode = 0;
    }

    public int getMode()
    {
        return now_mode;
    }

    public void setMpde(int mode)
    {
        now_mode = mode;
    }
}
