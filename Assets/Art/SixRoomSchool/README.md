# Six-room school map

打开 main，在 Grid 下可以看到 01—06 六个房间，各自包含背景、地板、地基和墙体 Tilemap。
A/D 移动，K 跳跃，走近门按 E 开关。进入教学房间后关门，完成移动和跳跃落地后出口自动打开。
第一个房间从高空坠落入场，生命归零或掉出地图会重新坠落复活。
第四房间的吊灯和光束会摇摆，第五房间随前进逐渐变亮，第六房间为明亮天台。
当前未制作 Boss，可先走完整张地图。接入 Boss 后勾选 Level Flow 的 Boss Encounter Enabled，并在死亡事件调用 CompleteBossFight()。

All backgrounds, terrain and doors are Tilemaps in Assets/Scenes/main.unity.
Room groups live under Grid. World range is x=0..196, ground surface y=1.
Room one has a tall shaft; player starts at (12,27) and falls into view.

Rooms: 01 Rebirth (0..24), 02 Tutorial (24..56), 03 Boss classroom (56..92),
04 Night corridor (92..128), 05 Dawn classroom (128..160), 06 Rooftop (160..196).

A/D move, K jump, E opens/closes a nearby unlocked door.
The tutorial closes its entrance and opens its exit after walking then jumping and landing.
No boss has been created. The boss room is traversable; enable Boss Encounter Enabled on
Level Flow when a boss exists, then call CompleteBossFight() on defeat.

Doors swap Door_Closed/Door_Open Tile assets; only closed doors enable BoxCollider2D.
Open Tile assets have Collider Type None. Do not add a TilemapCollider2D to door layers.
All terrain uses Grid collision. Editable Tile assets are in Tiles/.

Room five brightens as the player crosses; room six has bright paper daylight.
Night Hall Swaying Lamp objects animate the hanging light and its soft light cone.
Artwork came from built-in imagegen; source prompts: Docs/ArtGenerationPrompts.md.
The PNGs are unmodified outputs; floor and door margins are trimmed by Sprite importer rectangles.
