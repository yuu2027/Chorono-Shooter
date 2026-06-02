# Chrono Shooter

Chrono Shooter は、時間を遅くするスローゲージを使いながら敵弾を避けて戦う 2D シューティングゲームです。

通常敵を倒しながら進行し、最後に登場するボスを撃破するとゲームクリアになります。結果画面ではスコア、撃破数、被弾数、プレイ時間を確認できます。

## Features

- スローゲージを消費して敵の移動や弾速を遅くする時間操作システム
- 通常敵フェーズからボス戦へ進むステージ構成
- HP、スコア、ボスHP、スローゲージなどのゲーム中UI
- Game Over / Game Clear 画面でのリザルト表示
- BGM、SE、エフェクトを管理するキュー方式のデータ構成

## Requirements

- Unity `6000.4.3f1`
- Unity Input System

## Scenes

Build Settings には以下のシーンが登録されています。

| Scene | Description |
| --- | --- |
| `Assets/Scenes/TitleScene.unity` | タイトル画面 |
| `Assets/Scenes/GameScene.unity` | メインゲーム |
| `Assets/Scenes/GameOverScene.unity` | ゲームオーバー画面 |
| `Assets/Scenes/GameClearScene.unity` | ゲームクリア画面 |

## How to Play

Unity Editor でプロジェクトを開き、`TitleScene` または `GameScene` を再生してください。

| Action | Keyboard / Mouse | Gamepad |
| --- | --- | --- |
| Move | `WASD` / Arrow keys | Left Stick |
| Shoot | `Space` / Left Click | South Button |
| Slow | `Left Shift` | Left Trigger |
| Pause | `Esc` | Start |

## Project Structure

| Path | Description |
| --- | --- |
| `Assets/Scenes` | ゲームで使用するUnityシーン |
| `Assets/Scripts` | プレイヤー、敵、ボス、弾、UI、音声、エフェクトなどのゲームロジック |
| `Assets/Arts` | ゲーム内で使用する画像、プレハブ、音声素材 |
| `Assets/Data` | ステージ、音声、エフェクトなどの ScriptableObject データ |
| `Assets/AssetStore` | Unity Asset Store からインポートした素材 |
| `Packages` | Unity Package Manager の依存関係 |
| `ProjectSettings` | Unity プロジェクト設定 |

## Assets and Credits

### Original / Generated Assets

- プレイヤー画像、ボス画像、弾画像は ChatGPT で生成した画像を使用しています。
- ゲームロジック、シーン構成、ゲームルールは本プロジェクト用に制作しています。

### Unity Asset Store Assets

| Asset | Publisher / Author | Used For | License / Notes |
| --- | --- | --- | --- |
| [2D Space Kit](https://assetstore.unity.com/packages/2d/environments/2d-space-kit-27662) | Brett Gregory | 背景、小惑星、宇宙系素材 | Standard Unity Asset Store EULA |
| [Tiny Space Ships](https://gamecontentshopper.com/asset/all-assets/tiny-space-ships/2025/08/21/) | Disruptor Art | 敵機スプライト | プロジェクト内 `.meta` で `packageName: Tiny Space Ships` を確認済み。公式UnityページURLは要確認 |
| [Free 2D Impact FX](https://marketplace.unity.com/packages/vfx/particles/fire-explosions/free-2d-impact-fx-201222) | Inguz Media | `Impact01` - `Impact03` などのヒット/爆発エフェクト | Standard Unity Asset Store EULA |
| [Space Game GUI kit](https://assetstore.unity.com/packages/2d/gui/icons/space-game-gui-kit-298577) | DanProps | UI素材、アイコン、フォント素材 | Standard Unity Asset Store EULA |
| [Sci-fi GUI skin](https://assetstore.unity.com/packages/2d/gui/sci-fi-gui-skin-15606) | 3d.rina | GUIスキン、ボタン、バー素材 | Standard Unity Asset Store EULA |
| [War FX](https://assetstore.unity.com/packages/vfx/particles/war-fx-5669) | Jean Moreno | 爆発、煙、戦闘エフェクト | Standard Unity Asset Store EULA |
| [FREE Power Music for Awesome Games](https://assetstore.unity.com/packages/audio/music/electronic/free-power-music-for-awesome-games-166487) | Aural Space | BGM素材 | Standard Unity Asset Store EULA |

### Fonts

- `Asap` / `Righteous` は SIL Open Font License のフォント素材として使用しています。
- TextMesh Pro 付属フォントやリソースは Unity / TextMesh Pro の提供物として扱います。

### Assets to Confirm

以下の素材はプロジェクト内の `.meta` からインポート元の名前は確認できますが、正式な配布ページ、作者名、またはライセンス表記は追加確認が必要です。

| Asset / Source Name | Used For | Notes |
| --- | --- | --- |
| `Grenade Sound FX` | 爆発、撃破、破壊系SE | `Assets/Grenade Sound FX/Grenade/...` 由来の音声を使用 |
| `Music Packs/Ambient Sci-Fi` | タイトル、ゲーム中、ゲームオーバー、ゲームクリアなどのBGM | `Assets/Music Packs/Ambient Sci-Fi/...` 由来の音声を使用 |
| その他一部SE | ボタン、射撃、スロー、ポーズ、警告など | 正式な素材名と配布元は要確認 |

## License Notes

このリポジトリ自体のライセンスは未設定です。ソースコードや自作素材の利用条件を公開する場合は、別途ライセンスを追加してください。

Unity Asset Store 由来の素材は、それぞれのアセットのライセンスおよび Unity Asset Store EULA に従ってください。
