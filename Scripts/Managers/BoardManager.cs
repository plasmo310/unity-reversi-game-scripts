using Reversi.Board;
using Reversi.Services;
using UnityEngine;
using VContainer;

namespace Reversi.Managers
{
    /// <summary>
    /// ボード管理クラス
    /// </summary>
    public class BoardManager
    {
        /// <summary>
        /// 土台となるオブジェクト
        /// </summary>
        private readonly GameObject _boardCellsBase;

        /// <summary>
        /// ボードのセルPrefab
        /// </summary>
        private readonly GameObject _boardCellPrefab;

        /// <summary>
        /// セルの位置配列
        /// </summary>
        private Vector3[,] _cellPositions;

        [Inject]
        public BoardManager(IAssetsService assetsService, BoardBehaviour boardBehaviour)
        {
            // 土台となるオブジェクトを生成
            _boardCellsBase = new GameObject("BoardCells");
            _boardCellsBase.transform.SetParent(boardBehaviour.transform);
            _boardCellsBase.transform.position = boardBehaviour.transform.position;

            // Prefab読み込み
            _boardCellPrefab = assetsService.LoadAssets("BoardCell");
        }

        /// <summary>
        /// 一辺あたりのセル数
        /// </summary>
        public static readonly int CellSideCount = 8;

        /// <summary>
        /// ボード生成処理
        /// </summary>
        public void GenerateBoard()
        {
            _cellPositions = new Vector3[CellSideCount, CellSideCount];
            for (var x = 0; x < CellSideCount; x++)
            {
                for (var z = 0; z < CellSideCount; z++)
                {
                    // セル生成
                    var cell = Object.Instantiate(_boardCellPrefab, _boardCellsBase.gameObject.transform);
                    cell.transform.localPosition = new Vector3(x, 0.4f, z);

                    // 位置を保持
                    _cellPositions[x, z] = cell.transform.localPosition;
                }
            }
        }

        /// <summary>
        /// 指定セルの位置を取得
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Vector3 GetCellPosition(int x, int z)
        {
            return _cellPositions[x, z];
        }
    }
}
