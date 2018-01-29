using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crevice.Core
{
    /**
     * Todo: キーボードと組み合わせる関係で、
     * SendInput関係に、recapture = false 的な機能が必要っぽい
     * 例えばあるマウスボタンをCtrlとして使うようなシーンで、Ctrlをジェスチャに組み込む場合、
     * アプリケーション側でReCaptureしないといけない
     * 
     * Todo: 現在のコンテキストを表示するようなビュー。コールバックだけあればユーザーサイドでやれる？
     * 
     * Todo: カーソルの軌跡的なビュー。これもジェスチャ状態だけ貰えればあとは定期的にポーリングすればいける
     *          あるいはストローク情報に開始終了位置を含むとか
     * 
     * Todo: 複数のプロファイルを読み込むような機能。これはAppで実装できるかな
     *          複数のアプリを実行するより、優先度の管理がしやすい
     *              DeclareProfile("ProfileName");
     *              SubProfile("ProfileName");
     *              Using(new Profile("ProfileName")) {}
     *              NextProfile();
     *              
     *       複数でも、OR的なのと、無関係に並列っぽいのがありえる
     *              SubProfile
     *              ParallelProfile
     *                  として実装はできそう
     *              SubProfile(); とするよりは、 SubProfile(() => {  }); としたほうがいい？
     *                  →微妙
     *              
     * Todo: ベンチマーク
     * 
     * Todo: On() と If() を区別するのは？ SingleThrowとStrokeはIfのほうがよいような…
     *          →これだと互換性アリにできる？
     *              →DoubleThrowが常にOnな点が違う
     * 
     * Todo: WhenやOn, Ifに文字列で記述を追加したい
     *          Description("hoge", ja="")
     * 
     * Todo: プロファイルのグラフ出力。ブラウザに出せばいいのでそのように
     * 
     */

    namespace Misc
    {
        interface IIsDisposed
        {
            bool IsDisposed { get; }
        }
    }
}
