using System;

namespace Model
{
    [Serializable]
    public class TouchDurationRapidTestResult
    {
        /// <summary>
        /// タッチテスト回数
        /// </summary>
        public string TestCount;

        /// <summary>
        /// タッチ間隔(millisec)
        /// </summary>
        public string TestDuration;

        /// <summary>
        /// 認識したタッチ回数
        /// </summary>
        public string DetectTouchCount;

        /// <summary>
        /// タッチ認識精度
        /// </summary>
        public string Accuracy;
    }
}