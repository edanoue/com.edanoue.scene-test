#nullable enable

namespace Edanoue.TestAPI
{
    public enum Status
    {
        // 一度もRunnerにより実行されていない 生成直後 の状態
        Created,
        // 現在 テストが 実行中
        Running,
        // 成功して終了
        Succeed,
        // 失敗して終了
        Failed,
        // キャンセルされて終了
        Canceled,
    }
}
