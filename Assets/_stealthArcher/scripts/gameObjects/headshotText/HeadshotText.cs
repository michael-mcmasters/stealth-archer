using System.Collections;
using _stealthArcher.scripts.constants;
using _stealthArcher.scripts.util;
using UnityEngine;

namespace _stealthArcher.scripts.gameObjects.headshotText {
public class HeadshotText {

    private static HeadshotText instance;
    public static HeadshotText Instance => (instance == null) ? new HeadshotText() : instance;

    private HeadshotText() {
        // this.HeadshotTextObj = FindGam
    }
    
    public void Activate(Enemy enemy) {
        GameObject headshotTextObj = GameObjects.HeadshotTextObj;
        headshotTextObj.transform.position = new Vector3(enemy.transform.position.x, 3, enemy.transform.position.z);
        headshotTextObj.transform.LookAt(headshotTextObj.transform.position + Camera.main.transform.forward);
        CoroutineRunner.Run(MoveUpCoroutine());
    }
    
    private IEnumerator MoveUpCoroutine() {
        float speed = 4;
        GameObject headshotTextObj = GameObjects.HeadshotTextObj;
        
        // Keep moving while the global y position is less than 100
        while (headshotTextObj.transform.position.y < 100f)
        {
            // Move upward along global Y
            headshotTextObj.transform.position += Vector3.up * speed * Time.deltaTime;

            // Optional: clamp to 100
            if (headshotTextObj.transform.position.y > 100f)
            {
                Vector3 clamped = headshotTextObj.transform.position;
                clamped.y = 100f;
                headshotTextObj.transform.position = clamped;
            }

            yield return null; // wait for next frame
        }
    }
    
}
}