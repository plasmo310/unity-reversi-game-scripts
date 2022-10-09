# unity-reversi-game-scripts
Unityで作ったリバーシのスクリプト。</br>5(+3)種類のAIと対戦することができます。
### 改訂履歴
- v1.0.0</br>リリースに向けてUI、広告等のその他機能を実装
- v0.4.0</br>各プレイヤーのキャラクターを作成、選択できるよう実装
- v0.3.0</br>ML-Agentsを用いたAIを実装
- v0.2.0</br>モンテカルロ法で置くAIを実装
- v0.1.0</br>ゲーム基盤、ランダムに置くAI、MiniMaxアルゴリズムで置くAIを実装
### Unityバージョン<br>
- 2021.3.1f1<br>
### 使用アセット・パッケージ<br>
- VContainer<br>https://github.com/hadashiA/VContainer
- UniRx<br>https://github.com/neuecc/UniRx
- UniTask<br>https://github.com/Cysharp/UniTask
- ML-Agents<br>https://github.com/Unity-Technologies/ml-agents
- DOTWeen<br>https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676
- Very Animation<br>https://assetstore.unity.com/packages/tools/animation/very-animation-96826?locale=ja-JP
- Japanese School Classroom<br>https://assetstore.unity.com/packages/3d/environments/japanese-school-classroom-18392
- Universal Toon Shader<br>https://github.com/unity3d-jp/UnityChanToonShaderVer2_Project/releases
- Google Mobile Ads Unity Plugin<br>https://github.com/googleads/googleads-mobile-unity/releases/tag/v7.2.0
- UnmaskForUGUI<br>https://github.com/mob-sakai/UnmaskForUGUI
### 使用フォント<br>
- にしき的フォント<br>https://umihotaru.work
- Noto Sans Japanese<br>https://fonts.google.com
### 全体構成
- シーン設計
  - タイトルシーン<br>モードとキャラクターを選択するシーン
    - VSモード<br>選択したキャラクターとオセロするモード
    - 観戦モード<br>キャラクター同士のオセロを見守るモード
  - ゲームシーン<br>選択したキャラクターとオセロを行うシーン
- ソフトウェア設計<br>
![image](https://user-images.githubusercontent.com/77447256/173171317-a7f7a288-6e54-43e2-b9cd-107d3364601a.png)
