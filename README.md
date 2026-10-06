# game_01

黑白手绘校园题材的 2D 银河城原型，使用 Unity 2022.3.62f1c1 与 URP 2D。

## 打开项目

通过 Unity Hub 添加此仓库目录，使用 Unity 2022.3.62f1 系列编辑器打开，等待资源导入。
打开 `Assets/Scenes/offline.unity` 后点击 Play，可从主菜单进入游戏。
直接打开 `Assets/Scenes/main.unity` 可以检查关卡。

## 当前内容

- offline：开始游戏、Settings 亮度/对比度/伽马调整、ABOUT US 名单。
- cg：播放 `Assets/cg/valid.mp4`，结束后进入 main。
- main：六个以门连接的 Tilemap 房间，整体从黑暗走向明亮。

| 房间 | 内容 |
| --- | --- |
| 01 | 高空坠落入场与复活区 |
| 02 | 移动和跳跃教学，进入后关门、完成后开启出口 |
| 03 | 昏暗的 Boss 教室，预留 Boss 接口 |
| 04 | 夜晚走廊、摇曳吊灯与光束 |
| 05 | 随前进逐渐变亮的教室 |
| 06 | 明亮的学校天台 |

## 操作和调整

- **A / D**：左右移动；**K**：跳跃；**E**：开关附近的门。
- 开门切换贴图并关闭碰撞，关门恢复碰撞。
- `player` 的 `PlayerAttributes` 集中管理速度、跳跃、生命和战斗基础数值。
- main 的 `Grid` 下有 01—06 房间，每个房间含背景、地面、地基和墙体 Tilemap。
- 当前没有 Boss 实体，Boss 房可通行。添加 Boss 后启用 `Level Flow / Boss Encounter Enabled`，并在击败 Boss 时调用 `CompleteBossFight()`。

新地图资源位于 `Assets/Art/SixRoomSchool`，美术生成提示词记录于 `Docs/ArtGenerationPrompts.md`。
Unity 缓存、IDE 临时文件和本地验证副本不进入版本库。
