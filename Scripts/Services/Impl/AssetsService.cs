using System.Collections.Generic;
using UnityEngine;

namespace Reversi.Services.Impl
{
    public class AssetsService : IAssetsService
    {
        /// <summary>
        /// キャッシュしたAssets
        /// key: ファイル名
        /// </summary>
        private readonly IDictionary<string, GameObject> _cachedAssetsDictionary = new Dictionary<string, GameObject>();

        /// <summary>
        /// Assetsファイルの読み込み
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <returns>Asset(GameObject)</returns>
        public GameObject LoadAssets(string fileName)
        {
            // ファイル名をキーとしてキャッシュする
            if (!_cachedAssetsDictionary.ContainsKey(fileName))
            {
                var asset = Resources.Load(fileName) as GameObject;
                _cachedAssetsDictionary.Add(fileName, asset);
            }
            return _cachedAssetsDictionary[fileName];
        }
    }
}
