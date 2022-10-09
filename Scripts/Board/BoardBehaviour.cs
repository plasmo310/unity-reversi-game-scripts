using UnityEngine;

namespace Reversi.Board
{
    /// <summary>
    /// ボードクラス
    /// </summary>
    public class BoardBehaviour : MonoBehaviour
    {
        /// <summary>
        /// ボードのセルPrefab
        /// </summary>
        [SerializeField] private GameObject boardCellPrefab;

        /// <summary>
        /// 土台となるオブジェクト
        /// </summary>
        private GameObject _boardCellsBase;

        /// <summary>
        /// セルの位置配列
        /// </summary>
        private Vector3[,] _cellPositions;

        /// <summary>
        /// 一辺あたりのセル数
        /// </summary>
        public static int CellSideCount => 8;

        private void Awake()
        {
            // 土台となるオブジェクトを生成
            _boardCellsBase = new GameObject("BoardCells");
            _boardCellsBase.transform.SetParent(transform);
            _boardCellsBase.transform.position = transform.position;
            _boardCellsBase.transform.localScale = Vector3.one;

            // ボード生成
            GenerateBoard();
        }

        /// <summary>
        /// ボード生成処理
        /// </summary>
        private void GenerateBoard()
        {
            _cellPositions = new Vector3[CellSideCount, CellSideCount];
            for (var x = 0; x < CellSideCount; x++)
            {
                for (var z = 0; z < CellSideCount; z++)
                {
                    // セル生成
                    var cell = Instantiate(boardCellPrefab, _boardCellsBase.gameObject.transform);
                    cell.transform.localPosition = new Vector3(x, 0.4f, z);
                    cell.transform.localScale = boardCellPrefab.transform.localScale;

                    // 位置を保持
                    _cellPositions[x, z] = cell.transform.localPosition;
                }
            }
        }

        /// <summary>
        /// 指定セルの位置を取得
        /// </summary>
        public Vector3 GetCellPosition(int x, int z)
        {
            return _cellPositions[x, z];
        }
    }
}
