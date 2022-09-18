using TMPro;

namespace Reversi.Extensions
{
    public static class TextMeshProExtensions
    {
        /// <summary>
        /// TextMeshProのダイナミックフォント設定処理
        /// </summary>
        /// <param name="textMeshPro"></param>
        /// <param name="value"></param>
        public static void SetTextForDynamic(this TextMeshProUGUI textMeshPro, string value)
        {
            // 文字が既に設定済の場合、返却する
            if (textMeshPro.text == value)
            {
                return;
            }

            // 文字が取得できない場合、リセットする
            var fontAsset = textMeshPro.font;

            // 独自対応
            // staticなフォントの場合、fallbackにダイナミックフォントが設定している場合があるため取得し直す
            if (fontAsset.atlasPopulationMode == AtlasPopulationMode.Static)
            {
                fontAsset = fontAsset.fallbackFontAssetTable[0];
            }

            // ダイナミックフォントかつ文字が読み込めない場合、フォントアセットをリセットする
            if (fontAsset != null
                && fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic
                && !fontAsset.TryAddCharacters(value))
            {
                fontAsset.ClearFontAssetData();
                fontAsset.TryAddCharacters(value);
            }

            // 文字を設定
            textMeshPro.text = value;
        }
    }
}
