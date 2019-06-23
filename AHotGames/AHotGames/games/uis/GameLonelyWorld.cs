using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class GameLonelyWorld : AHotBase
{
    Transform Role;
    protected override void InitComponents()
    {
        var btnQuit = FindWidget<Button>("btnQuit");
        btnQuit.onClick.AddListener(() =>
        {
            UnloadThis();

            LoadAnotherUI<UIMain>();
        });

        Role = UStaticFuncs.FindChild(gameObj.transform, "Role");

        var LegL = UStaticFuncs.FindChild(gameObj.transform, "LegL");
        var LegR = UStaticFuncs.FindChild(gameObj.transform, "LegR");

        InitObjs();

        const float rotateSpeed = 20;
        const float moveSpeed = 2;
        var updater = gameObj.AddComponent<UUpdater>();
        updater.onUpdate += () =>
        {
            if (Input.GetKey(KeyCode.A))
            {
                Role.transform.Rotate(Vector3.down * Time.deltaTime * rotateSpeed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                Role.transform.Rotate(Vector3.up * Time.deltaTime * rotateSpeed);
            }
            if (Input.GetKey(KeyCode.W))
            {
                Role.transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                Role.transform.Translate(Vector3.back * Time.deltaTime * moveSpeed);
            }
        };
    }

    private void InitObjs()
    {
        var Objs = UStaticFuncs.FindChild(gameObj.transform, "Objs");
        var aobjs = new List<Transform>();
        for (var i = 0; i < Objs.childCount; i++)
        {
            aobjs.Add(Objs.GetChild(i));
        }

        var ixcount = UnityEngine.Random.Range(20, 30);
        var iycount = UnityEngine.Random.Range(20, 30);
        for (var i = 1; i <= ixcount; i++)
        {
            for (var j = 1; j <= iycount; j++)
            {
                var obj = aobjs[UnityEngine.Random.Range(0, aobjs.Count)];
                var x = (i - 10) * 10 + UnityEngine.Random.Range(0, 10);
                var z = (j - 10) * 10 + UnityEngine.Random.Range(0, 10);
                obj = GameObject.Instantiate(obj, Role.parent);
                obj.localPosition = new Vector3(x, obj.localPosition.y, z);
            }
        }
    }
}

