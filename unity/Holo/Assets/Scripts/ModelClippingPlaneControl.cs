﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.Buttons;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.UX;

public class ModelClippingPlaneControl : MonoBehaviour, IClickHandler
{
    public CompoundButton ButtonClippingPlane;
    public CompoundButton ButtonClippingPlaneTranslation;
    public CompoundButton ButtonClippingPlaneRotation;
    public Material ClippingMaterial;
    public GameObject ModelWithPlate;

    BoundingBoxRig clipPlaneQuadBbox;
    HandDraggable HandTranslation;
    
    private enum ClipPlaneState
    {
        Disabled,
        Active,
        Translation,
        Rotation
    }

    ClipPlaneState ClippingPlaneState = ClipPlaneState.Disabled;

    void Start()
    {
        clipPlaneQuadBbox = GetComponent<BoundingBoxRig>();
        HandTranslation = GetComponent<HandDraggable>();
        HandTranslation.enabled = false;

        ResetState();
    }

    public void Click(GameObject clickObj)
    {
        Debug.Log("Clicked obj: " + clickObj.name);
        switch (clickObj.name)
        {
            case "ButtonClipping":
                ClickedClipPlane();
                break;

            case "ButtonClippingTranslation":
                ChangeButtonsState(ClipPlaneState.Translation);
                break;

            case "ButtonClippingRotation":
                ChangeButtonsState(ClipPlaneState.Rotation);
                break;
        }

    }

    void ClickedClipPlane()
    {
        ModelWithPlate modelWithPlate = ModelWithPlate.GetComponent<ModelWithPlate>();
        if (!modelWithPlate.Instance)
        {
            Debug.LogWarning("No model loaded for clipping plane");
            ClippingPlaneState = ClipPlaneState.Disabled;
            return;
        }

        SkinnedMeshRenderer modelRenderer = modelWithPlate.Instance.GetComponent<SkinnedMeshRenderer>();

        ClippingPlaneState = ClippingPlaneState == ClipPlaneState.Disabled ? ClipPlaneState.Active : ClipPlaneState.Disabled;

        if (ClippingPlaneState == ClipPlaneState.Active)
        {
            modelRenderer.material = ClippingMaterial;
            
            ButtonClippingPlaneTranslation.gameObject.SetActive(true);
            ButtonClippingPlaneRotation.gameObject.SetActive(true);
            modelWithPlate.SetButtonState(ButtonClippingPlane, true); 
        }
        else
        {
            modelRenderer.material = modelWithPlate.MaterialNonPreview;
            clipPlaneQuadBbox.Deactivate();
            ButtonClippingPlaneTranslation.gameObject.SetActive(false);
            ButtonClippingPlaneRotation.gameObject.SetActive(false);
            modelWithPlate.SetButtonState(ButtonClippingPlane, false);
        }
    }

    void ChangeButtonsState(ClipPlaneState newState)
    {
        ModelWithPlate modelWithPlate = ModelWithPlate.GetComponent<ModelWithPlate>();
        if (newState == ClippingPlaneState)
        {
            // clicking again on the same sidebar button just toggles it off
            newState = ClipPlaneState.Active;
        }
        ClippingPlaneState = newState;

        modelWithPlate.SetButtonState(ButtonClippingPlaneTranslation, newState == ClipPlaneState.Translation);
        modelWithPlate.SetButtonState(ButtonClippingPlaneRotation, newState == ClipPlaneState.Rotation);

        HandTranslation.enabled = newState == ClipPlaneState.Translation;

        if (newState == ClipPlaneState.Active || newState == ClipPlaneState.Disabled)
        {
            clipPlaneQuadBbox.Deactivate();
        }
        else
        {
            clipPlaneQuadBbox.Activate();
        }        
    }


    public void ResetState()
    {
        ClippingPlaneState = ClipPlaneState.Disabled;
        ButtonClippingPlaneTranslation.gameObject.SetActive(false);
        ButtonClippingPlaneRotation.gameObject.SetActive(false);
        clipPlaneQuadBbox.Deactivate();
    }
}
