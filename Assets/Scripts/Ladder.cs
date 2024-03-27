using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Ladder : MonoBehaviour, IInteractable
{
    private SpriteRenderer sr;
    GameObject player;
    bool goingUp;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void Interact(){
        Vector3 pos = player.transform.position;
        pos.x = transform.position.x;
        player.transform.position = pos;
        goingUp = pos.y < transform.position.y ? true : false;
        if(goingUp){
            //create vector3 based on top of the Ladder game object minus 1/2 the height of the player
            Vector3 topPosition = transform.position + Vector3.Scale(transform.up, sr.bounds.size) - new Vector3(0f, player.GetComponent<SpriteRenderer>().bounds.extents.y*2, 0f);
            player.GetComponent<CharacterControl>().MoveUpLadder(topPosition);
        }else{
            //create vector3 based on the bottom of the Ladder game object minus 1/2 the height of the player
           Vector3 bottomPosition = transform.position - new Vector3(0f, sr.bounds.extents.y - player.GetComponent<SpriteRenderer>().bounds.extents.y/2, 0f);
           player.GetComponent<CharacterControl>().MoveDownLadder(bottomPosition);
        }
    }
    private void OnTriggerExit2D(Collider2D collision){
        if(collision.CompareTag("Player")){

        }
    }

}

