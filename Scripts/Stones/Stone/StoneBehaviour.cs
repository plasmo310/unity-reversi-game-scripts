using UnityEngine;

namespace Reversi.Stones.Stone
{
    /// <summary>
    /// ストーン コアクラス
    /// </summary>
    public partial class StoneBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject stoneObject;
        [SerializeField] private GameObject focusArea;

        public static readonly string TagName = "Stone";

        /// <summary>
        /// 表示中の状態
        /// </summary>
        private StoneState _viewState;

        /// <summary>
        /// 配列に対応するインデックス
        /// </summary>
        public StoneIndex Index { get; private set; }

        private void Awake()
        {
            // 最初は非表示にしておく
            stoneObject.SetActive(false);
            focusArea.SetActive(false);
        }

        /// <summary>
        /// ストーンの状態を変更する
        /// </summary>
        /// <param name="changeState">変更するストーン状態</param>
        /// <param name="putStoneIndex">状態変更時に置かれたストーン位置(最初の設定時のみnull)</param>
        public void ChangeViewState(StoneState changeState, StoneIndex putStoneIndex = null)
        {
            // 状態が変わらない場合は何も行わない
            if (_viewState == changeState) return;

            // 状態によって表示を切り替える
            switch (changeState)
            {
                // Emptyの場合は非表示
                case StoneState.Empty:
                    stoneObject.SetActive(false);
                    break;
                // 色が指定されたら表示
                case StoneState.White:
                case StoneState.Black:
                    stoneObject.SetActive(true);
                    // 最終的に設定する位置、回転
                    var targetPosition = Vector3.zero;
                    var targetRotation = new Vector3(0.0f, 0.0f, changeState == StoneState.White ? 0.0f : 180.0f);
                    if (_viewState == StoneState.Empty)
                    {
                        // 初めて置かれる場合
                        stoneObject.transform.localEulerAngles = targetRotation;
                        if (putStoneIndex != null) StartPutEffect();
                    }
                    else
                    {
                        // 色が変わる場合
                        StartTurnAnimation(putStoneIndex, () =>
                        {
                            // 微妙なずれを防ぐためアニメーション完了時に直接指定する
                            stoneObject.transform.localPosition = targetPosition;
                            stoneObject.transform.localEulerAngles = targetRotation;
                        });
                    }
                    break;
            }

            // 状態の設定
            _viewState = changeState;
        }

        /// <summary>
        /// ストーンのボード内でのインデックスを設定する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public void SetIndex(int x, int z)
        {
            Index = new StoneIndex(x, z);
        }

        /// <summary>
        /// フォーカス状態を切り替える
        /// </summary>
        /// <param name="value"></param>
        public void SetIsFocus(bool value)
        {
            focusArea.SetActive(value);
        }
    }
}
