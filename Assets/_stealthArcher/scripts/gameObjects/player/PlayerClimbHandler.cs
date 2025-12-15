using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.input;
using UnityEngine;

namespace _stealthArcher.scripts.gameObjects.player {
public class PlayerClimbHandler : MonoBehaviour {

    private GameObject canJumpIndicatorObj;
    private Collider colliderToClimb;

    
    void Start() {
        canJumpIndicatorObj = Instantiate(GameObjects.Cursor);
        
        GetComponent<OnTapHandler>().OnTapCallback = () => {
            Debug.Log("OnTap called in Player!");
            if (canJumpIndicatorObj.activeSelf) {
                Debug.Log("Jumping!");
                
                CharacterController cc = GetComponent<CharacterController>();
                cc.Move(Vector3.up * 5f);
            } else {
                Debug.Log("Can't jump");
            }
        };
    }

    void Update() {
        float radius = 1;
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in hits) {
            if (col.name.Contains(Constants.WALL)) {
                canJumpIndicatorObj.SetActive(true);
                canJumpIndicatorObj.transform.position = new Vector3(transform.position.x, 3, transform.position.z);
                colliderToClimb = col;
                return;
            }
        }
        canJumpIndicatorObj.SetActive(false);
        return;
    }

}
}