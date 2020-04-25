// =======================================================================================
// Created and maintained by Fhiz
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// =======================================================================================

using UnityEngine;
using Mirror;
using System.Collections;

// =======================================================================================
// SHARED INSTANCE PORTAL
// =======================================================================================
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(NetworkIdentity))]
public class UCE_SharedInstancePortal : NetworkBehaviour
{
    [Header("[INSTANCE PORTAL]")]
    public UCE_UI_SharedInstance ui;

    public int instanceCategory;

    [Header("[Editor]")]
    public Color gizmoColor = new Color(0, 1, 1, 0.25f);
    public Color gizmoWireColor = new Color(1, 1, 1, 0.8f);

    // -----------------------------------------------------------------------------------
    // OnDrawGizmos
    // @Editor
    // -----------------------------------------------------------------------------------
    private void OnDrawGizmos()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();

        // we need to set the gizmo matrix for proper scale & rotation
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(collider.offset, collider.size);
        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireCube(collider.offset, collider.size);
        Gizmos.matrix = Matrix4x4.identity;
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerExit
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    private void OnTriggerEnter2D(Collider2D co)
    {
        Player player = co.GetComponentInParent<Player>();

        if (!player || !player.isAlive || player != Player.localPlayer) return;

        if (!ui)
            ui = FindObjectOfType<UCE_UI_SharedInstance>();

        ui.Show(instanceCategory);
    }

    // -----------------------------------------------------------------------------------
    // OnTriggerExit
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    private void OnTriggerExit2D(Collider2D co)
    {
        Player player = co.GetComponentInParent<Player>();

        if (!player || !player.isAlive || player != Player.localPlayer) return;

        if (!ui)
            ui = FindObjectOfType<UCE_UI_SharedInstance>();

        ui.Hide();
    }

    // -----------------------------------------------------------------------------------
}