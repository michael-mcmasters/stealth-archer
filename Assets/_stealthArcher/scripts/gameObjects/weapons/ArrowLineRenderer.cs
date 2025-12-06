using _stealthArcher.scripts.constants;
using UnityEngine;

namespace _stealthArcher.scripts.weaponPrefabs {
public class ArrowLineRenderer : MonoBehaviour {
    public float maxLength = 5f;         // how long the wind trail gets
    public float growSpeed = 10f;        // how fast the trail expands
    public float shrinkSpeed = 15f;      // how fast it disappears on hit

    private LineRenderer lr;
    private float currentLength = 0f;
    private bool hit = false;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = 2;

        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        // Start with no visible line
        currentLength = 0f;
        UpdateLinePositions();
    }

    void Update() {
        if (!hit) {
            // Arrow is flying → wind trail extends
            currentLength = Mathf.MoveTowards(currentLength, maxLength, growSpeed * Time.deltaTime);
        }
        else {
            // Arrow has hit → retract the wind trail
            currentLength = Mathf.MoveTowards(currentLength, 0f, shrinkSpeed * Time.deltaTime);

            // Optional: auto-disable when done
            if (currentLength <= 0.01f)
                gameObject.SetActive(false);
        }

        UpdateLinePositions();
    }

    void UpdateLinePositions() {
        // Tail stays behind the arrow
        lr.SetPosition(0, new Vector3(0, 0, -currentLength));

        // Front stays at the arrow base
        lr.SetPosition(1, new Vector3(0, 0, 0));
    }

    void OnCollisionEnter(Collision collision) {
        hit = true;
        // Destroy(lr);
        // this.enabled = false;
    }
}
}