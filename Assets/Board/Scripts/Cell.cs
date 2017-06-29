﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    private CameraScript.Element type = CameraScript.Element.NOTHING;
    private GameObject baseObj = null;
    private GameObject overlayObj = null;

    public Cell(CameraScript.Element element, GameObject block, Material mat)
    {
        this.type = element;
        this.baseObj = block;
        Renderer renderer = block.GetComponent<Renderer>();
        renderer.material = mat;
        block.transform.Rotate(new Vector3(0, 90 * Random.Range(0, 4), 0));
    }


    public void addCrate(GameObject obj)
    {
        this.overlayObj = obj;
        type = CameraScript.Element.CRATE;
        //obj.transform.Rotate(new Vector3(0, 90 * Random.Range(0, 4), 0));
    }



    public CameraScript.Element getType()
    {
        return type;
    }


    public void setType(CameraScript.Element type)
    {
        this.type = type;
    }

    public void smashCrate()
    {
        if (type != CameraScript.Element.CRATE) return;
        if (overlayObj == null) return;

        CrateScript crate = (CrateScript)overlayObj.GetComponent(typeof(CrateScript));
        crate.detonate(overlayObj);
        type = CameraScript.Element.FLOOR;
        overlayObj = null;
    }


    public void destroyObjects()
    {
        if (baseObj != null) Object.Destroy(baseObj);
        if (overlayObj != null)
        {
            if (!overlayObj.CompareTag("Finish")) Object.Destroy(overlayObj);
        }
    }
}
