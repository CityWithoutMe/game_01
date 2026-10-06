using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>Painted Tilemap door: an open door never blocks movement.</summary>
[RequireComponent(typeof(Tilemap), typeof(BoxCollider2D))]
public sealed class SchoolDoor : MonoBehaviour
{
    [SerializeField] private TileBase closedTile;
    [SerializeField] private TileBase openTile;
    [SerializeField] private bool initiallyOpen;
    [SerializeField] private bool initiallyLocked;
    private Tilemap tiles;
    private BoxCollider2D blocker;
    private Transform player;
    public bool IsOpen { get; private set; }
    public bool IsLocked { get; private set; }

    private void Awake()
    {
        IsLocked = initiallyLocked;
        SetOpen(initiallyOpen && !IsLocked);
    }

    public void SetOpen(bool open)
    {
        if (tiles == null) tiles = GetComponent<Tilemap>();
        if (blocker == null) blocker = GetComponent<BoxCollider2D>();
        IsOpen = open;
        tiles.SetTile(Vector3Int.zero, open ? openTile : closedTile);
        blocker.enabled = !open;
    }

    public void SetLocked(bool locked)
    {
        IsLocked = locked;
        if (locked) SetOpen(false);
    }

    private bool InReach()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }
        return player != null && Mathf.Abs(player.position.x - transform.position.x) < 2.6f
            && Mathf.Abs(player.position.y - 2f) < 2.2f;
    }

    private void Update()
    {
        if (!IsLocked && InReach() && Input.GetKeyDown(KeyCode.E))
        {
            // Closing while occupied would push the player into a wall.
            if (IsOpen && Mathf.Abs(player.position.x - transform.position.x) < 1.4f) return;
            SetOpen(!IsOpen);
        }
    }

    private void OnGUI()
    {
        if (!InReach()) return;
        string message = IsLocked ? "LOCKED" : IsOpen ? "E  /  CLOSE" : "E  /  OPEN";
        GUI.Box(new Rect(Screen.width * 0.5f - 85f, Screen.height - 70f, 170f, 34f), message);
    }
}
